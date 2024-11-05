using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSound : MonoBehaviour
{
    [SerializeField]
    AudioClip clickSound;
    [SerializeField]
    AudioClip rotateSound;
    [SerializeField]
    AudioClip moveStartSound;
    [SerializeField]
    AudioClip moveSuccessSound;
    [SerializeField]
    AudioClip moveCancelSound;

    public void PlayClickSound() => AudioManager.Instance.Play2DSound(clickSound, SettingManager.Instance._FxVolume);

    public void PlayRotateSound() => AudioManager.Instance.Play2DSound(rotateSound, SettingManager.Instance._FxVolume);

    public void PlayMoveStartSound() => AudioManager.Instance.Play2DSound(moveStartSound, SettingManager.Instance._FxVolume);

    public void PlayMoveEndSound(bool isSuccess)
    {
        AudioClip targetClip = isSuccess ? moveSuccessSound : moveCancelSound;
        AudioManager.Instance.Play2DSound(targetClip, SettingManager.Instance._FxVolume);
    }

    private void Start()
    {
        Tile tile = GetComponent<Tile>();
        if(tile != null)
        {
            tile.onClickEvent.AddListener(PlayClickSound);
            tile.onRotateEvent.AddListener(PlayRotateSound);
            tile.startMoveEvent.AddListener(PlayMoveStartSound);
            tile.endMoveEvent.AddListener(PlayMoveEndSound);
        }
    }
}
