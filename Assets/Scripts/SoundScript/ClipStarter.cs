using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipStarter : MonoBehaviour
{
    public AudioClip clip;
    public float volume = 1f;

    public FMODUnity.EventReference bgmEvent;
    // Start is called before the first frame update
    void Start()
    {
        //if (clip != null)
        //{
        //    AudioManager.Instance.PlayBackground(clip, volume * SettingManager.Instance._BGMVolume);
        //}


        if(!bgmEvent.IsNull)
        {
            AudioManager.Instance.PlayBackground(bgmEvent);
        }
    }

}
