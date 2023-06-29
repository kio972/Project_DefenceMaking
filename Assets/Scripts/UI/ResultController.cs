using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private Transform victory;
    [SerializeField]
    private Transform defeat;

    public void GameWin()
    {
        victory.gameObject.SetActive(true);
        defeat.gameObject.SetActive(false);
    }

    public void GameDefeat()
    {
        victory.gameObject.SetActive(false);
        defeat.gameObject.SetActive(true);
    }
}
