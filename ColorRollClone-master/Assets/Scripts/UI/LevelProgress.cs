using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private GameObject orangeBlock;
    [SerializeField] private GameObject grayBlock;



    void Start()
    {
        orangeBlock.SetActive(false);
        grayBlock.SetActive(false);
    }

    public void Init(int level, int totalLevel)
    {
        foreach (Transform child in transform)
        {
            if (child != orangeBlock.transform && child != grayBlock.transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < totalLevel; i++)
        {
            var newBlock = Instantiate(i < (totalLevel-level) ? grayBlock : orangeBlock);

            newBlock.transform.parent = transform;
            newBlock.transform.localScale = Vector3.one;

            newBlock.SetActive(true);
        }
    }
}
