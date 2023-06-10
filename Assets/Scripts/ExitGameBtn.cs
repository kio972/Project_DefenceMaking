using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameBtn : MonoBehaviour
{
    [SerializeField]
    private Button btn;

    void ExitGame()
    {
        Application.Quit();
    }

    void Start()
    {
        if (btn == null)
            btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(ExitGame);
    }
}
