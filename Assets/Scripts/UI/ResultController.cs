using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private Transform victory;
    [SerializeField]
    private Transform defeat;

    [SerializeField]
    private Image fade;

    [SerializeField]
    private MoveSceneBtn titleBtn;

    [SerializeField]
    FMODUnity.EventReference winSound;
    [SerializeField]
    FMODUnity.EventReference defeatSound;

    private void TitleBtnOn()
    {
        titleBtn.gameObject.SetActive(true);
        UtilHelper.IColorEffect(titleBtn.transform, Color.clear, Color.white, 0.5f).Forget();
    }

    private void FadeOn()
    {
        fade.gameObject.SetActive(true);
        UtilHelper.IColorEffect(fade.transform, Color.clear, new Color(0, 0, 0, 0.9f), 2f, () => TitleBtnOn()).Forget();
    }

    public async UniTaskVoid GameWin()
    {
        //if(GameManager.Instance.LastSpawnedAdventurer != null)
        //    FindObjectOfType<CameraController>()?.CamMoveToPos(GameManager.Instance.LastSpawnedAdventurer.transform.position);
        //else
        //    FindObjectOfType<CameraController>()?.ResetCamPos();
        //await UniTask.Delay(System.TimeSpan.FromSeconds(2.5f));

        //SaveManager.Instance.ResetPlayerData();

        TextMeshProUGUI text = titleBtn.GetComponentInChildren<TextMeshProUGUI>();
        if(SceneManager.GetActiveScene().name == "Stage0")
        {
            if(SaveManager.Instance.playerData.sceneName == "Stage0")
                SaveManager.Instance.ResetPlayerData();
            SaveManager.Instance.playerData = null;
            titleBtn.useLoadingScene = true;
            if (text != null)
                text.text = DataManager.Instance.GetDescription("ui_menu_continue");
            titleBtn.sceneName = "Stage1";
        }
        else
        {
            StoryManager.Instance.EnqueueScript("Dan900");
            await UniTask.WaitUntil(() => StoryManager.Instance.IsScriptQueueEmpty);

            fade.gameObject.SetActive(true);
            UtilHelper.IColorEffect(fade.transform, Color.clear, new Color(0, 0, 0, 0.9f), 2f).Forget();
            victory.gameObject.SetActive(true);
            defeat.gameObject.SetActive(false);
            //AudioManager.Instance.Play2DSound("Victory_icon_01", SettingManager.Instance._FxVolume);
            FMODUnity.RuntimeManager.PlayOneShot(winSound);
            await UniTask.WaitForSeconds(4);
            SceneController.Instance.MoveScene("DemoEnd");
            return;
            //titleBtn.useLoadingScene = false;
            //if (text != null)
            //    text.text = SettingManager.Instance.language == Languages.korean ? "Ÿ��Ʋ��" : "Main Menu";
            //titleBtn.sceneName = "TitleScene";
        }

        FadeOn();
        victory.gameObject.SetActive(true);
        defeat.gameObject.SetActive(false);
        //AudioManager.Instance.Play2DSound("Victory_icon_01", SettingManager.Instance._FxVolume);
        FMODUnity.RuntimeManager.PlayOneShot(winSound);
    }

    public async UniTaskVoid GameDefeat()
    {
        FindObjectOfType<CameraController>()?.ResetCamPos();
        await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
        if (GameManager.Instance.king.isDead)
            await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));
        else
        {
            StoryManager.Instance.EnqueueScript("Dan800");
            await UniTask.WaitUntil(() => StoryManager.Instance.IsScriptQueueEmpty);
        }

        TextMeshProUGUI text = titleBtn.GetComponentInChildren<TextMeshProUGUI>();
        if(text != null)
            text.text = DataManager.Instance.GetDescription("ui_menu_gotitle");
        titleBtn.useLoadingScene = false;
        titleBtn.sceneName = "TitleScene";

        FadeOn();
        victory.gameObject.SetActive(false);
        defeat.gameObject.SetActive(true);
        //AudioManager.Instance.Play2DSound("Defeat_icon_01", SettingManager.Instance._FxVolume);
        FMODUnity.RuntimeManager.PlayOneShot(defeatSound);
    }
}
