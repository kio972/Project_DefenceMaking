using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField]
    GameObject uiPage;

    public void SetActive(bool value)
    {
        uiPage.SetActive(value);
        GameManager.Instance.SetPause(value);
    }
}
