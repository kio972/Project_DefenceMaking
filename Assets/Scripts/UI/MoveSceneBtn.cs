using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSceneBtn : MonoBehaviour
{
    [SerializeField]
    private Button btn;
    public string sceneName;

    public bool useLoadingScene = false;

    protected virtual void MoveScene()
    {
        if (useLoadingScene)
        {
            SettingManager.Instance.nextScene = sceneName;
            SceneController.Instance.MoveScene("LoadScene");
        }
        else
            SceneController.Instance.MoveScene(sceneName);
    }


    // Start is called before the first frame update
    void Start()
    {
        if (btn == null)
            btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(MoveScene);
    }
}
