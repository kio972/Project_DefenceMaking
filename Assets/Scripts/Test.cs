using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public NotificationControl noti;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) { StoryManager.Instance.EnqueueScript("Dan100"); }

        if (Input.GetKeyDown(KeyCode.T)) { noti.SetMesseage("test"); }
    }
}
