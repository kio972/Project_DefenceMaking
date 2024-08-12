using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolveController : MonoBehaviour
{
    public bool isAppare;
    public AnimationCurve appareCurve;
    public float appareGoaltime = 1;

    public bool isDisappare;
    public AnimationCurve disappareCurve;
    public float disappareGoaltime = 1;

    [ColorUsage(true, true)]
    public Color burnColor;
    Image imageComp;
    float currentTime = 0;

    private Material material = null;

    // Start is called before the first frame update
    void Start()
    {
        imageComp = GetComponent<Image>();
        material = Instantiate(imageComp.material);
        imageComp.material = material;
        imageComp.material.SetColor("_Color", burnColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAppare && appareGoaltime != 0)
        {
            imageComp.material.SetFloat("_Switch", 0);

            Color.RGBToHSV(burnColor, out float H, out float S, out float V);
            if (currentTime < appareGoaltime)
            {
                currentTime += Time.deltaTime;
                imageComp.material.SetFloat("_DissolvePower", appareCurve.Evaluate(currentTime / appareGoaltime));
            }
            else
            {
                isAppare = false;
                currentTime = 0;
            }
        }

        if (isDisappare && disappareGoaltime != 0)
        {
            imageComp.material.SetFloat("_Switch", 1);
            Color.RGBToHSV(burnColor, out float H, out float S, out float V);
            if (currentTime < disappareGoaltime)
            {
                currentTime += Time.deltaTime;
                imageComp.material.SetFloat("_DissolvePower", disappareCurve.Evaluate(currentTime / disappareGoaltime));
                imageComp.material.SetColor("_HSV", new Color(H, S, V, 0));
            }
            else
            {
                isDisappare = false;
                currentTime = 0;
            }
        }



    }
}
