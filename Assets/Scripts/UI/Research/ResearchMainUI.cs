using Cysharp.Threading.Tasks;
using Spine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ResearchMainUI : MonoBehaviour, ISwappableGameObject
{
    [SerializeField]
    GameObject uiPage;

    public ResearchSlot curResearch { get; private set; }

    private float curProgressTime;

    public float CurProgressTime { get => curProgressTime; }

    public float CurProgressRate { get => curResearch == null ? -1 : 1 - (curProgressTime / curResearch._ResearchData.requiredTime); }

    //private Coroutine researchCoroutine;

    private CancellationTokenSource researchCancelToken;

    private List<string> _completedResearchs;
    public List<string> completedResearchs { get => new List<string>(_completedResearchs); }

    [SerializeField]
    private AK.Wwise.Event openSound;
    [SerializeField]
    private AK.Wwise.Event completeSound;

    private async UniTask IResearch(ResearchSlot curResearch, float additionalTime)
    {
        this.curResearch = curResearch;

        float startTime = (GameManager.Instance.CurWave * 1440) + GameManager.Instance.Timer;
        float endTime = startTime + curResearch._ResearchData.requiredTime - additionalTime;
        curResearch.SetResearchState(ResearchState.InProgress);
        float curTime = startTime;
        await UniTask.Yield(researchCancelToken.Token);
        
        while(true)
        {
            curTime += GameManager.Instance.InGameDeltaTime;
            if(curTime >= endTime)
                break;
            curProgressTime = endTime - curTime;
            await UniTask.Yield(researchCancelToken.Token);
        }

        curProgressTime = 0f;
        curResearch.SetResearchState(ResearchState.Complete);
        //ResearchPopup popup = GetComponentInChildren<ResearchPopup>();
        //if (popup.gameObject.activeSelf && popup._CurResearch == curResearch._ResearchData)
        //    curResearch.CallPopUpUI();
        string targetMesseage = DataManager.Instance.GetDescription(curResearch._ResearchData.researchName);
        GameManager.Instance.notificationBar?.SetMesseage(targetMesseage + " " + DataManager.Instance.GetDescription("ui_ResearchCompleted"), NotificationType.Research);
        //AudioManager.Instance.Play2DSound("Complete_Tech", SettingManager.Instance._FxVolume);
        openSound?.Post(gameObject);

        this.curResearch = null;
        _completedResearchs.Add(curResearch._ResearchId);
        Research[] research = curResearch.GetComponents<Research>();
        foreach(var item in research)
            item?.ActiveResearch();
        
    }

    public bool StartResearch(ResearchSlot target, float additionalTime = 0)
    {
        researchCancelToken?.Cancel();
        researchCancelToken?.Dispose();
        researchCancelToken = new CancellationTokenSource();
        IResearch(target, additionalTime).Forget();
        return true;
    }

    public void StopResearch(ResearchSlot target)
    {
        if (curResearch == null)
            return;

        researchCancelToken?.Cancel();
        researchCancelToken?.Dispose();
        researchCancelToken = new CancellationTokenSource();
        curProgressTime = 0f;
        curResearch.SetResearchState(ResearchState.Incomplete);
        this.curResearch = null;
    }

    public void SetActive(bool value)
    {
        if (value)
        {
            InputManager.Instance.ResetTileClick();
            GameManager.Instance.SetPause(true);
            //AudioManager.Instance.Play2DSound("Tech_research_Open", SettingManager.Instance._FxVolume);
            openSound?.Post(gameObject);
            Animator btnAnim = btnObject.GetComponent<Animator>();
            btnAnim?.SetBool("End", true);
        }

        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
    }

    public void TryOpenUI()
    {
        GameObjectSwap swapper = transform.GetComponentInParent<GameObjectSwap>();
        if (swapper != null)
            swapper.SwapObject(this);
        else
            SetActive(true);
    }


    [SerializeField]
    private GameObject btnObject;

    private bool isActived { get => btnObject.activeSelf || uiPage.activeSelf; }

    public void Update()
    {
        if (Input.GetKeyDown(SettingManager.Instance.key_Research._CurKey) && isActived)
        {
            if (uiPage.activeSelf)
                SetActive(false);
            else
                TryOpenUI();
        }
    }

    public void SaveData(PlayerData data)
    {
        data.researchIdes = new List<string>(_completedResearchs);
        if(curResearch != null)
        {
            data.curResearch = curResearch._ResearchId;
            data.curResearchTime = curResearch._ResearchData.requiredTime - curProgressTime;
        }
    }

    public void LoadData(PlayerData data)
    {
        _completedResearchs = new List<string>(data.researchIdes);

        ResearchSlot[] slots = GetComponentsInChildren<ResearchSlot>(true);
        foreach (ResearchSlot slot in slots)
        {
            if (_completedResearchs.Contains(slot._ResearchId))
            {
                slot.SetResearchState(ResearchState.Complete);
                Research research = slot.GetComponent<Research>();
                research?.ActiveResearch();
            }
            else if(!string.IsNullOrEmpty(data.curResearch) && slot._ResearchId == data.curResearch)
                StartResearch(slot, data.curResearchTime);
        }
    }

    public void ForceActiveResearch(string id)
    {
        ResearchSlot slot = researchDic[id];
        slot.SetResearchState(ResearchState.Complete);
        Research research = slot.GetComponent<Research>();
        research?.ActiveResearch();
        _completedResearchs.Add(slot._ResearchId);
    }

    private Dictionary<string, ResearchSlot> researchDic = new Dictionary<string, ResearchSlot>();

    private void Awake()
    {
        if(_completedResearchs == null)
        {
            _completedResearchs = new List<string>();
            ResearchSlot[] slots = GetComponentsInChildren<ResearchSlot>(true);
            foreach (ResearchSlot slot in slots)
            {
                if (string.IsNullOrEmpty(slot._ResearchId) || researchDic.ContainsKey(slot._ResearchId))
                    continue;

                researchDic.Add(slot._ResearchId, slot);
                if (!_completedResearchs.Contains(slot._ResearchId))
                    continue;

                slot.SetResearchState(ResearchState.Complete);
                Research research = slot.GetComponent<Research>();
                research?.ActiveResearch();
            }
        }
    }
}
