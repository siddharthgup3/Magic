using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class OtherTest : SerializedMonoBehaviour
{
    public HashSet<GameObject> objectHash;
    public Transform playerTransform;

    [ReadOnly] private List<KeyValuePair<GameObject,float>> orderedByDistance;
    public Dictionary<GameObject, int> origDict;
    public List<GameObject> finalList;
    [Button("Log current Distances")]
    public void Bleh()
    {
        foreach (var VARIABLE in objectHash)
        {
            Debug.Log(
                $"Distance to player = {Vector3.Distance(VARIABLE.transform.position, playerTransform.position)}");
        }
    }
    
    [Button("YO")]
    public void OtherBleh()
    {

        finalList = new List<GameObject>();


        foreach (var kvp in origDict.OrderBy(p=>Vector3.Distance(p.Key.transform.position,playerTransform.position)))
        {
            finalList.Add(kvp.Key);
        }

        
    }
}