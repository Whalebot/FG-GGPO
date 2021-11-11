using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Sirenix.OdinInspector;

public class AffectAllMoves : MonoBehaviour
{
    public List<Move> allMoves;
    // Start is called before the first frame update
    void Start()
    {

    }

    [Button]
    void LoadItemSO()
    {
        //string[] assetNames = AssetDatabase.FindAssets("t:Move", new[] { "Assets/Moveset" });


        //foreach (string SOName in assetNames)
        //{
        //    var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
        //    var item = AssetDatabase.LoadAssetAtPath<Move>(SOpath);
        //    allMoves.Add(item);
        //}
    }

    [Button]
    void DoubleMeterGain()
    {
        foreach (var item in allMoves)
        {
            //foreach (var t in item.attacks)
            //{
            //    if (t.groundHitProperty.meterGain < 10) t.groundHitProperty.meterGain *= 2;
            //    if (t.groundBlockProperty.meterGain < 10) t.groundBlockProperty.meterGain *= 2;
            //    if (t.groundCounterhitProperty.meterGain < 10) t.groundCounterhitProperty.meterGain *= 2;
            //    if (t.airHitProperty.meterGain < 10) t.airHitProperty.meterGain *= 2;
            //    if (t.airBlockProperty.meterGain < 10) t.airBlockProperty.meterGain *= 2;
            //    if (t.airCounterhitProperty.meterGain < 10) t.airCounterhitProperty.meterGain *= 2;

            //}

            //EditorUtility.SetDirty(item);
        }
    }
}
