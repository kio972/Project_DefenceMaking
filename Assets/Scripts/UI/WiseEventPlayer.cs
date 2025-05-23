using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiseEventPlayer : MonoBehaviour
{
    [SerializeField]
    FMODUnity.EventReference targetEvent;

    public void PlayWWiseEvent()
    {
        FMODUnity.RuntimeManager.PlayOneShot(targetEvent);
    }
}
