using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiseEventPlayer : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event targetEvent;

    public void PlayWWiseEvent()
    {
        targetEvent.Post(gameObject);
    }
}
