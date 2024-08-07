using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;


public class Test : MonoBehaviour
{

    private void Awake()
    {
        Test2.Instance.tests.ObserveAdd().Select(_ => true).Subscribe(_ => print("Added"));
        Test2.Instance.tests.ObserveRemove().Select(_ => true).Subscribe(_ => print("Removed"));
    }

    
}
