using JetBrains.Annotations;
using SkiaDemo1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class CarpetSOComparer : IComparer<CarpetSO>
{
    public int Compare(CarpetSO a, CarpetSO b)
    {
        if (a.Order > b.Order) return 1;
        if (a.Order < b.Order) return -1;
        return 0;
    }
}
public class CarpetComparer : IComparer<Carpet>
{
    public int Compare(Carpet a, Carpet b)
    {
        if (a.carpetSO.Order > b.carpetSO.Order) return 1;
        if (a.carpetSO.Order < b.carpetSO.Order) return -1;
        return 0;
    }
}

public class Carpet : MonoBehaviour
{
    public class RefNode
    {
        public int parentCount = 0;

        public CarpetSO carpetSO = null;

        public Node<Carpet> correctCarpet = null;
    }

    public class ParentNode
    {
        public Node<Carpet> node;

        public int IndexInParent;
    }

    public int InGameIndex = -1;

    public int Level = 1;

    //Some quick access to other components to support the MainGame
    //This class doesn't care about these:
    public CarpetMeshCreator carpetMeshCreator { private set; get; }
    public CarpetRoller carpetRoller { private set; get; }
    public CarpetSO carpetSO { private set; get; }

    public Node<Carpet> node;

    public Node<RefNode> refNode;

    public List<ParentNode> parents { private set; get; }

    [NonSerialized] public bool correctFlag  = false;

    [NonSerialized] public bool hintFlag = false;


    public Carpet Init(CarpetSO carpetSO)
    {
        carpetMeshCreator = GetComponent<CarpetMeshCreator>();

        carpetRoller = GetComponent<CarpetRoller>();

        carpetMeshCreator.carpetSO = carpetSO;

        this.carpetSO = carpetSO;

        node = new Node<Carpet>(this);

        parents = new List<ParentNode>();

        Level = 1;

        return this;
    }

    public void SetY(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public bool AddToTree(Node<Carpet> node, int level = 0)
    {
        if (this.node == node) return true;

        var collidedWithChildren = false;

        foreach (var child in this.node.children)
        {
            if (child.obj.AddToTree(node, level + 1))
            {
                collidedWithChildren = true;
            }
        }

        if (!collidedWithChildren)
        {
            if (Utils.Instance.PolygonCollision(
                carpetSO.Polygon, node.obj.carpetSO.Polygon,
                carpetSO.Position, node.obj.carpetSO.Position,
                carpetSO.PoleOfInaccessibility, node.obj.carpetSO.PoleOfInaccessibility))
            {
                node.obj.parents.Add(new ParentNode() { node = this.node, IndexInParent = this.node.children.Count });

                node.obj.Level += this.node.obj.Level;

                this.node.AddChild(node);

                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    public float GetHeight()
    {
        return (Level - 2) * 0.001f /*+ carpetMeshCreator.Thickness * 0.5f*/;
    }
    public float GetCorrectHeight()
    {
        return (refNode.obj.carpetSO.Order) * 0.001f /*+ carpetMeshCreator.Thickness * 0.5f*/;
    }

    public void Unset()
    {
        foreach (var child in node.children)
        {
            child.obj.Unset();
        }

        node.children.Clear();

        foreach (var parentRef in parents)
        {
            parentRef.node.children[parentRef.IndexInParent] = parentRef.node.children[parentRef.node.children.Count - 1];

            var n = parentRef.node.children[parentRef.IndexInParent].obj;

            ParentNode nParentRef = null;

            foreach (var nP in n.parents)
            {
                if (nP.node == parentRef.node)
                {
                    nParentRef = nP;

                    break;
                }
            }
            if (nParentRef != null)
            {
                nParentRef.IndexInParent = parentRef.IndexInParent;
            }

            parentRef.node.children.RemoveAt(parentRef.node.children.Count - 1);
        }

        Level = 1;

        correctFlag = false;

        parents.Clear();
    }

    public bool CheckWithRefTree(Node<RefNode> root,List<Node<Carpet>> correctNodes)
    {
        if (node.obj.carpetSO.Order != root.obj.carpetSO.Order)
        {
            node.obj.correctFlag = false;

            return false;
        }
        node.obj.correctFlag = true;

        if (node.children.Count > root.children.Count)
        {
            return false;
        }
        

        for (int i = 0; i < node.children.Count; i++)
        {
            var n = node.children[i];

            //find the node of the same order
            Node<RefNode> refNode = null;

            for (int j = 0; j < root.children.Count; j++)
            {
                if (n.obj.carpetSO.Order == root.children[j].obj.carpetSO.Order &&
                    n.obj.parents.Count == root.children[j].obj.parentCount)
                {
                    refNode = root.children[j];

                    break;
                }
            }
            if (refNode == null)
            {
                return false;
            }

            if (!n.obj.CheckWithRefTree(refNode, correctNodes))
            {
                return false;
            }
            //from here the RefNode is matched with the current node 
        }


        return true;
    }

    public static Node<Carpet> CreateRootNode()
    {
        Carpet rootCarpet = new Carpet() { carpetSO = new CarpetSO() { Order = -1 }, parents = new List<ParentNode>() };

        rootCarpet.carpetSO.Polygon.Add(new Vector2(float.MinValue, float.MinValue));
        rootCarpet.carpetSO.Polygon.Add(new Vector2(float.MinValue, float.MaxValue));
        rootCarpet.carpetSO.Polygon.Add(new Vector2(float.MaxValue, float.MaxValue));
        rootCarpet.carpetSO.Polygon.Add(new Vector2(float.MaxValue, float.MinValue));

        rootCarpet.node = new Node<Carpet>(rootCarpet);

        return rootCarpet.node;
    }

    public static Node<RefNode> BuildRefTree(Carpet rootCarpet, Carpet[] carpets)
    {
        var refTreeRoot = new Node<RefNode>(new RefNode() { carpetSO = rootCarpet.carpetSO, parentCount = 0 });

        for (int i = 0; i < carpets.Length; i++)
        {
            var refNode = new Node<RefNode>(
                new RefNode()
                {
                    carpetSO = carpets[i].carpetSO,
                    parentCount = 0,
                    correctCarpet = carpets[i].node
                }
            );

            carpets[i].refNode = refNode;

            AddToRefTree(refTreeRoot, refNode);
        }

        return refTreeRoot;
    }

    public static bool AddToRefTree(Node<RefNode> root, Node<RefNode> node, int level = 0)
    {
        if (root == node) return true;

        bool collidedWithChildren = false;

        foreach (var child in root.children)
        {
            if (AddToRefTree(child, node, level + 1))
            {
                collidedWithChildren = true;
            }
        }
        if (!collidedWithChildren)
        {
            if (Utils.Instance.PolygonCollision(
                root.obj.carpetSO.Polygon, node.obj.carpetSO.Polygon,
                root.obj.carpetSO.Position, node.obj.carpetSO.Position,
                root.obj.carpetSO.PoleOfInaccessibility, node.obj.carpetSO.PoleOfInaccessibility))
            {
                root.AddChild(node);

                node.obj.parentCount++;// s.Add(root);

                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }



    public static Node<Carpet> BuildTree(Carpet[] carpets)
    {
        var root = CreateRootNode();

        for (int i = 0; i < carpets.Length; i++)
        {
            root.obj.AddToTree(carpets[i].node);
        }
        return root;
    }

    public static void DisplayRefTree(Node<RefNode> root)
    {
        Debug.Log("DisplayRefTree");

        root.Traverse((r) =>
        {
            Debug.Log("Order: " + r.carpetSO.Order);
        });

        Debug.Log("End of DisplayRefTree");
    }

    public void DisplayTree()
    {
        Debug.Log("DisplayTree");

        node.Traverse((c) =>
        {
            Debug.Log("Order: " + c.carpetSO.Order + " Level: " + c.Level);
        });

        Debug.Log("End of DisplayTree");
    }
}


//root.Traverse((c) =>
//{
//    Debug.Log("Order " + c.carpetSO.Order);
//});


//Carpet temp = (new Carpet(-1)).Init(new CarpetSO() { Order = -1 });
//Carpet temp1 = (new Carpet(0)).Init(new CarpetSO() { Order = 0 });
//Carpet temp2 = (new Carpet(1)).Init(new CarpetSO() { Order = 1 });
//Carpet temp3 = (new Carpet(2)).Init(new CarpetSO() { Order = 2 });

//temp.node.AddChild(temp1.node);
//temp.node.AddChild(temp2.node);

//temp.node.AddChild(temp3.node);

//Debug.Log("IsSubset" + temp.node.IsSubset(rootCarpet.node,new CarpetComparer()));

//var node4 = new Node<Carpet>(new Carpet(4));

//var node2 = new Node<Carpet>(new Carpet(2));
//node2.AddChild(node4);

//var node1 = new Node<Carpet>(new Carpet(1));
//node1.AddChild(node4);

//node.AddChild(node1);
//node.AddChild(node2);
//node.AddChild(new Node<Carpet>(new Carpet(3)));

//node.Traverse((carpet) =>
//{
//    Debug.Log("node->" + carpet.InGameIndex);
//});