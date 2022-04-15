using System;
using System.Collections.Generic;
using UnityEngine;

public class CarpetTree : MonoBehaviour
{
    [SerializeField] private GameObject carpetPrefab;

    //The Solution Tree
    public Node<Carpet.RefNode> SolutionTree { private set; get; }
    public Node<Carpet> CurrentTree { private set; get; }

    public Carpet[] Carpets { private set; get; }

    public int RolledOutCounter { private set; get; } = 0;
    public int CorrectNodeCount { private set; get; } = 0;

    public bool IsUnrolledCarpetsInOrder { private set; get; } = true;

    public Action OnFullyMatchedSolution;

    public CarpetRoller[] CarpetRollers { get; private set; }

    public CarpetTreeController CarpetTreeController { get; private set; }

    public Action<bool /*hintFlag*/> TreeStateChanged = delegate { };

    public Bounds Bounds { private set; get; }


    private void Awake()
    {
        CarpetTreeController = GetComponent<CarpetTreeController>();
    }

    public void GenerateTree(LevelSO levelSO)
    {
        int n = levelSO.carpetSOs.Count;

        Carpets = new Carpet[n];

        CarpetRollers = new CarpetRoller[n];

        for (int i = 0; i < n; i++)
        {
            Carpets[i] = Instantiate(carpetPrefab, transform).GetComponent<Carpet>().Init(levelSO.carpetSOs[i]);
            Carpets[i].carpetRoller.RollStateChanged += HandleCarpetRollStateChanged;
            Carpets[i].carpetRoller.BeforeRollIn += HandleBeforeRollIn;
            Carpets[i].carpetRoller.BeforeRollOut += HandleBeforeRollOut;
            Carpets[i].InGameIndex = -1;
            Carpets[i].carpetMeshCreator.MeshRebuiltCallback += Carpets[i].carpetRoller.HandleCarpetMeshRebuilt;
            Carpets[i].carpetMeshCreator.RebuildMesh();

            CarpetRollers[i] = Carpets[i].carpetRoller;
        }

        RolledOutCounter = 0;

        var rootCarpet = Carpet.CreateRootNode();

        SolutionTree = Carpet.BuildRefTree(rootCarpet.obj, Carpets);

        CurrentTree = rootCarpet;

        float left = float.MaxValue, right = float.MinValue, top = float.MinValue, bottom = float.MaxValue;
        for (int i = 0; i < n; i++)
        {
            var c = Carpets[i];

            c.SetY(c.GetCorrectHeight());

            if (c.carpetMeshCreator.Bounds.x < left) { left = c.carpetMeshCreator.Bounds.x; }
            if (c.carpetMeshCreator.Bounds.y > right) { right = c.carpetMeshCreator.Bounds.y; }
            if (c.carpetMeshCreator.Bounds.z > top) { top = c.carpetMeshCreator.Bounds.z; }
            if (c.carpetMeshCreator.Bounds.w < bottom) { bottom = c.carpetMeshCreator.Bounds.w; }
        }
        Bounds = new Bounds()
        {
            center = new Vector3((left + right) / 2, 0, (top + bottom) / 2),
            size = new Vector3((right - left), 0, (top - bottom))
        };

        //Carpet.DisplayRefTree(SolutionTree);

        CarpetTreeController.Reset();

        Vector3 origin = /*new Vector3(levelSO.Position.x, 0, levelSO.Position.y)- */-Bounds.center;

        transform.localPosition = origin + new Vector3(0, transform.localPosition.y, 0);
    }

    public void CleanUp()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        CarpetRollers = null;

        CorrectNodeCount = 0;

    }

    private void HandleBeforeRollIn(Carpet carpet)
    {

    }

    private void HandleBeforeRollOut(Carpet carpet)
    {
        AddToTree(carpet);

        carpet.SetY(carpet.GetHeight());
    }
    private void HandleCarpetRollStateChanged(bool rolled, GameObject carpet)
    {
        var c = carpet.GetComponent<Carpet>();

        if (rolled)
        {
            RemoveFromTree(c);

            RolledOutCounter--;
        }
        else
        {
            RolledOutCounter++;
        }

        var correctNodes = new List<Node<Carpet>>();

        IsUnrolledCarpetsInOrder = CurrentTree.obj.CheckWithRefTree(SolutionTree, correctNodes);

        CountCorrectNode();

        TreeStateChanged?.Invoke(c.hintFlag);

        if (IsUnrolledCarpetsInOrder && RolledOutCounter >= Carpets.Length)
        {
            OnPuzzleSolved();
        }

        //CurrentTree.obj.DisplayTree();

        //Carpet.DisplayRefTree(SolutionTree);

        Debug.Log((IsUnrolledCarpetsInOrder ? "InOrder " : "Not in order ") + "correctNodeCount " + CorrectNodeCount);
    }

    private int CountCorrectNode()
    {

        CorrectNodeCount = 0;

        for (int i = 0; i < Carpets.Length; i++)
        {
            if (Carpets[i].correctFlag)
            {
                CorrectNodeCount++;
            }
        }

        return CorrectNodeCount;
    }

    private void AddToTree(Carpet carpet)
    {
        CurrentTree.obj.AddToTree(carpet.node);
    }

    private void RemoveFromTree(Carpet current)
    {
        current.Unset();
    }

    private void OnPuzzleSolved()
    {
        CarpetTreeController.enabled = false;

        OnFullyMatchedSolution?.Invoke();
    }

    public CarpetRoller GetNextRoller()
    {
        if (CarpetRollers != null)
        {
            foreach (var roller in CarpetRollers)
            {
                if (roller.RolledIn && !roller.rollingOut)
                {
                    return roller;
                }
            }
        }
        return null;
    }

    public int GetRemainingNodeCount()
    {
        if(Carpets != null)
        {
            return Carpets.Length - CorrectNodeCount;
        }
        return 0;
    }

    //what the hell am I doing here.. fucking wasting time

    public Node<Carpet> GetNextNodeToRollOut()
    {
        Carpet.RefNode lowestNodeThatNotROlledOut = null;

        int minOrder = int.MaxValue;

        SolutionTree.TraverseWithTermination((sNode) =>
        {
            if (sNode.obj.parentCount == 0) return false;

            bool inCurrentTree = false;

            var refNode = sNode.obj;

            CurrentTree.TraverseWithTermination((cNode) =>
            {
                var carpet = cNode.obj;

                if (carpet.parents.Count == 0)
                {
                    return false;
                }

                //find the node of the same order
                if (carpet.carpetSO.Order == refNode.carpetSO.Order &&
                    carpet.parents.Count == refNode.parentCount)
                {
                    inCurrentTree = true;

                    return true;
                }

                return false;
            });

            if (!inCurrentTree)
            {
                if (refNode.carpetSO.Order < minOrder)
                {
                    minOrder = refNode.carpetSO.Order;

                    lowestNodeThatNotROlledOut = refNode;
                }
            }
            return false;
        });

        if (lowestNodeThatNotROlledOut != null)
        {
            return lowestNodeThatNotROlledOut.correctCarpet;
        }
        else
        {
            return null;
        }
    }

}
//private void OnBeforeUnroll(GameObject carpet)
//{
//    var c = carpet.GetComponent<Carpet>();

//    c.SetY(((float)c.Level + (float)c.IndexInParent * 0.5f) * 0.001f);

//    AddToTree(c);

//    Debug.Log("level " + c.Level + " index " + c.IndexInParent);
//}

//private IEnumerator OnBeforeRoll(GameObject carpet)
//{
//    var c = carpet.GetComponent<Carpet>();

//    if (c.node.children.Count > 0)
//    {
//        for (int i = 1; i < c.node.children.Count; i++)
//        {
//            var roller = c.node.children[i].obj.carpetRoller;

//            StartCoroutine(roller.Roll());
//        }

//        var roller0 = c.node.children[0].obj.carpetRoller;

//        yield return StartCoroutine(roller0.Roll());
//    }
//    else
//    {
//        yield return null;
//    }
//}
