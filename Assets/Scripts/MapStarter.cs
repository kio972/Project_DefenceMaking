using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStarter : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.Init();
    }
}
