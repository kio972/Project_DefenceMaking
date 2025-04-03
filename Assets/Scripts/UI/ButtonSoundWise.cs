using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundWise : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public bool soundOnClick = true;
    [Space]
    public bool soundOnHover = true;
    public bool soundOnUnHover = true;
    //[Space]
    //public AK.Wwise.Event soundOnClip;
    //public AK.Wwise.Event soundOnHoverClip;
    //public AK.Wwise.Event soundOnUnHoverClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (soundOnClick)
        {
            //soundOnClip.Post(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (soundOnHover)
        {
            //soundOnHoverClip.Post(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (soundOnUnHover)
        {
            //soundOnUnHoverClip.Post(gameObject);
        }
    }
}