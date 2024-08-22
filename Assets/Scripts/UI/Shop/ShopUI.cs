using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField]
    GameObject uiPage;

    [SerializeField]
    List<FluctTimer> timerList;

    private int curWave;

    [SerializeField]
    private IllustrateUI shopIllur;
    [SerializeField]
    private PopUpMessage malpoongsun;

    private HerbSlot[] herbSlots;
    private ItemSlot[] itemSlots;

    [SerializeField]
    private Transform itemShelf;

    public void PlayScript(string message, string anim, string anim2 = "")
    {
        malpoongsun.ToastMsg(message);
        if (anim == "idle")
            shopIllur.SetAnim(anim, true, 0);
        else
            shopIllur.SetAnim(anim, false, 0, "idle");

        if (!string.IsNullOrEmpty(anim2))
            shopIllur.SetAnim(anim2, false, 1);
    }

    public void PlayScript(string scriptID)
    {
        Dictionary<string, object> script = DataManager.Instance.GetMalpoongsunScript(scriptID);
        if (script == null)
            return;

        string conver = script["script"].ToString();
        string track0 = script["track0"].ToString();
        string track1 = script["track1"].ToString();

        PlayScript(conver, track0, track1);
    }

    public void SetActive(bool value)
    {
        if (value)
            InputManager.Instance.ResetTileClick();

        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);

        if(value)
        {
            PlayScript("Shop000");
            AudioManager.Instance.Play2DSound("Open_Store", SettingManager.Instance._FxVolume);
        }
    }

    private bool initState = false;
    private void Init()
    {
        if (initState)
            return;

        herbSlots = GetComponentsInChildren<HerbSlot>(true);
        foreach (HerbSlot slot in herbSlots)
            slot.Init();

        itemSlots = itemShelf.GetComponentsInChildren<ItemSlot>(true);
        foreach (ItemSlot slot in itemSlots)
            slot.Init();

        initState = true;
    }

    private void Awake()
    {
        Init();
    }

    [SerializeField]
    private GameObject btnObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3) && btnObject.activeSelf)
        {
            if (UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
                SetActive(true);
            else if (uiPage.activeSelf)
                SetActive(false);
        }

        if (curWave == GameManager.Instance.CurWave)
            return;

        curWave = GameManager.Instance.CurWave;
        foreach (FluctTimer timer in timerList)
            timer.IncreaseTime();
    }

    public void LoadData(PlayerData data)
    {
        if (!initState)
            Init();

        for (int i = 0; i < herbSlots.Length; i++)
            herbSlots[i]._CurPrice = data.herbData[i].curVal;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (data.itemsData[i].id != -1)
            {
                object target = DataManager.Instance.Deck_Table[data.itemsData[i].id]["text_name"];
                itemSlots[i].SetItem(SpriteList.Instance.LoadSprite(DataManager.Instance.Deck_Table[data.itemsData[i].id]["prefab"].ToString()), DataManager.Instance.GetDescription(target.ToString()));
            }

            itemSlots[i]._CurPrice = data.itemsData[i].curVal;
            itemSlots[i].IsSoldOut = data.itemsData[i].isSoldOut;
        }

        timerList[0]._CurTime = data.itemFluctCool;
        curWave = GameManager.Instance.CurWave;
    }

    public void SaveData(PlayerData data)
    {
        data.herbData = new List<ShopData>();
        for (int i = 0; i < herbSlots.Length; i++)
        {
            ShopData herb = new ShopData();
            herb.curVal = herbSlots[i]._CurPrice;
            data.herbData.Add(herb);
        }

        data.itemsData = new List<ShopData>();
        for (int i = 0; i < itemSlots.Length; i++)
        {
            ShopData item = new ShopData();
            RandomCard random = itemSlots[i].GetComponent<RandomCard>();
            item.id = random != null ? random._TargetIndex : -1;
            item.curVal = itemSlots[i]._CurPrice;
            item.isSoldOut = itemSlots[i].IsSoldOut;
            data.itemsData.Add(item);
        }

        data.itemFluctCool = timerList[0]._CurTime;
    }
}
