using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;

public class TitleUI : MonoBehaviour
{
    public SkeletonGraphic img;
    private bool clicked = false;

    public void ResetAnimation()
    {
        if (!clicked)
            return;

        clicked = false;
    }

}
