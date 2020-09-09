using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public bool toggle;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (toggle)
        {
            var x = GetComponent<LineRenderer>();
            if (x != null)
                Debug.Log("GetComp"+ true);
            else
                Debug.Log("GetComo " + false);
        }
        else
        {
            if (TryGetComponent(out LineRenderer lr))
            {
                Debug.Log(true);
            }
            else
                Debug.Log(false);
        }
    }
}