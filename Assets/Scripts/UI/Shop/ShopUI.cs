using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField]
    GameObject uiPage;


    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
    }
}
