using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedController : MonoBehaviour
{
    [SerializeField]
    private Button speed_0;
    [SerializeField]
    private Button speed_1;
    [SerializeField]
    private Button speed_2;
    
    public void SetSpeedZero()
    {
        Time.timeScale = 0;
    }

    public void SetSpeedNormal()
    {
        if (PathFinder.Instance.FindPath(NodeManager.Instance.startPoint) == null)
            return;

        Time.timeScale = 1;
    }

    public void SetSpeedFast()
    {
        if (PathFinder.Instance.FindPath(NodeManager.Instance.startPoint) == null)
            return;

        Time.timeScale = 2;
    }

    public void Start()
    {
        speed_0.onClick.AddListener(SetSpeedZero);
        speed_1.onClick.AddListener(SetSpeedNormal);
        speed_2.onClick.AddListener(SetSpeedFast);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
