using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundFmod : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public bool soundOnClick = true;
    [Space]
    public bool soundOnHover = true;
    public bool soundOnUnHover = true;
    [Space]
    public FMODUnity.EventReference soundOnClip;
    public FMODUnity.EventReference soundOnHoverClip;
    public FMODUnity.EventReference soundOnUnHoverClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (soundOnClick)
        {
            FMODUnity.RuntimeManager.PlayOneShot(soundOnClip);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (soundOnHover)
        {
            FMODUnity.RuntimeManager.PlayOneShot(soundOnHoverClip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (soundOnUnHover)
        {
            FMODUnity.RuntimeManager.PlayOneShot(soundOnUnHoverClip);
        }
    }
}