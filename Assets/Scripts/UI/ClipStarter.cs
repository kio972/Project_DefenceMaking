using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipStarter : MonoBehaviour
{
    public AudioClip clip;
    public float volume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (clip == null)
            return;
        AudioManager.Instance.PlayBackground(clip, volume * SettingManager.Instance.bgmVolume);
    }

}
