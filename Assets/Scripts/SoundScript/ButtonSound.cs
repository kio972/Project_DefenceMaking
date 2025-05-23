using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public bool soundOnClick = true;
    [Space]
    public bool soundOnHover = true;
    public bool soundOnUnHover = true;
    [Space]
    public AudioClip soundOnClip;
    public AudioClip soundOnHoverClip;
    public AudioClip soundOnUnHoverClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (soundOnClick)
        {
            AudioManager.Instance.Play2DSound(soundOnClip, SettingManager.Instance._UIVolume);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (soundOnHover)
        {
            AudioManager.Instance.Play2DSound(soundOnHoverClip, SettingManager.Instance._UIVolume);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (soundOnUnHover)
        {
            AudioManager.Instance.Play2DSound(soundOnUnHoverClip, SettingManager.Instance._UIVolume);
        }
    }
}