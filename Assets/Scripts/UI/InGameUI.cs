using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    private GameObject uiTopBlocker;
    [SerializeField]
    private GameObject uiRightBlocker;
    [SerializeField]
    private GameObject uiDownBlocker;
    
    public void Block_TopUI(bool value)
    {
        uiTopBlocker.SetActive(value);
    }

    public void Block_RightUI(bool value)
    {
        uiRightBlocker.SetActive(value);
    }

    public void Block_DownUI(bool value)
    {
        uiDownBlocker.SetActive(value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
