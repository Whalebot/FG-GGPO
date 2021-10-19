using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    TreeInstance[] trees;

    // Start is called before the first frame update
    void Start()
    {
        var terrain = GetComponent<Terrain>();
        trees = terrain.terrainData.treeInstances;
        //print(trees.Length);
        foreach (TreeInstance tree in trees)
        {
            //tree.GetComponent <>
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
