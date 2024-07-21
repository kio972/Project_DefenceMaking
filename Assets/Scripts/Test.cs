using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    private List<System.Func<bool>> events = new List<System.Func<bool>>();

    private bool Event1()
    {
        print("Event1");
        return false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            events.Add(Event1);

        if (Input.GetKeyDown(KeyCode.W))
            events.Remove(Event1);

        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var item in events)
                item.Invoke();
        }
    }
}
