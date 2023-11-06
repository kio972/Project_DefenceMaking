using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) { StoryManager.Instance.EnqueueScript("Dan000"); }
    }
}
