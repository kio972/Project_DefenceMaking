using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) { ScriptManager.Instance.EnqueueScript("Dan000"); }
    }
}
