using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbMaxUp : MonoBehaviour, Research
{
    [SerializeField]
    private int targetHerb = 1;

    [SerializeField]
    private int value = 100;

    public void ActiveResearch()
    {
        if (targetHerb == 1)
            GameManager.Instance.herb1Max += value;
        else if(targetHerb == 2)
            GameManager.Instance.herb2Max += value;
        else if(targetHerb == 3)
            GameManager.Instance.herb3Max += value;
    }
}