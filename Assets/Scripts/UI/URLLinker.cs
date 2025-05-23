using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class URLLinker : MonoBehaviour
{
    [SerializeField]
    private string urlAdress;

    private Button btn;

    private void GoToURL()
    {
        if (string.IsNullOrEmpty(urlAdress))
            return;

        Application.OpenURL(urlAdress);
    }

    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        if(btn != null)
            btn.onClick.AddListener(GoToURL);
    }
}
