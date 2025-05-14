using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopUI : MonoBehaviour, ISwappableGameObject
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
    private List<ItemSlot> _itemSlots;
    public List<ItemSlot> itemSlots { get => new List<ItemSlot>(_itemSlots); }

    [SerializeField]
    private Transform itemShelf;

    [SerializeField]
    FMODUnity.EventReference openSound;

    private List<System.Action<Item>> buyItemEvents = new List<System.Action<Item>>();

    public void InvokeBuyItemEvents(Item item)
    {
        foreach (var itemEvent in buyItemEvents)
            itemEvent.Invoke(item);
    }

    public void AddBuyItemEvent(System.Action<Item> addEvent) => buyItemEvents.Add(addEvent);

    public void RemoveBuyItemEvent(System.Action<Item> removeEvent) => buyItemEvents.Remove(removeEvent);

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

        string conver = script[SettingManager.Instance.language.ToString()].ToString();
        string track0 = script["track0"].ToString();
        string track1 = script["track1"].ToString();

        PlayScript(conver, track0, track1);
    }

    private void SetItems()
    {
        foreach(ItemSlot slot in _itemSlots)
        {
            bool isUnlocked = true;
            if (slot.item is INeedUnlockItem needUnlock && !needUnlock.IsUnlock)
                isUnlocked = false;

            slot.gameObject.SetActive(isUnlocked);
        }
    }

    public void SetActive(bool value)
    {
        if (value)
        {
            SetItems();
            InputManager.Instance.ResetTileClick();
            GameManager.Instance.SetPause(true);
            Animator btnAnim = btnObject.GetComponent<Animator>();
            btnAnim?.SetBool("End", true);
        }

        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        
        if(value)
        {
            PlayScript("Shop047");
            //AudioManager.Instance.Play2DSound("Open_Store", SettingManager.Instance._FxVolume);
            FMODUnity.RuntimeManager.PlayOneShot(openSound);
        }
    }

    public void TryOpenUI()
    {
        GameObjectSwap swapper = transform.GetComponentInParent<GameObjectSwap>();
        if (swapper != null)
            swapper.SwapObject(this);
        else
            SetActive(true);
    }


    private bool initState = false;
    private void Init()
    {
        if (initState)
            return;

        //herbSlots = GetComponentsInChildren<HerbSlot>(true);
        //foreach (HerbSlot slot in herbSlots)
        //    slot.Init();

        _itemSlots = itemShelf.GetComponentsInChildren<ItemSlot>(true).ToList();
        List<ItemSlot> deactivedItems = new List<ItemSlot>();
        foreach(var item in _itemSlots)
        {
            if(!item.gameObject.activeSelf)
                deactivedItems.Add(item);
        }

        foreach(var deactivedItem in deactivedItems)
        {
            _itemSlots.Remove(deactivedItem);
        }

        foreach (ItemSlot slot in _itemSlots)
            slot.Init();

        initState = true;
    }

    private async UniTaskVoid Awake()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.IsInit);
        Init();
    }

    [SerializeField]
    private GameObject btnObject;

    private bool isActived { get => btnObject.activeSelf || uiPage.activeSelf; }

    void Update()
    {
        if (Input.GetKeyDown(SettingManager.Instance.key_Shop._CurKey) && isActived && !GetComponentInChildren<CardPackEffect>(true).gameObject.activeSelf)
        {
            if (uiPage.activeSelf)
                SetActive(false);
            else
                TryOpenUI();
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

        //for (int i = 0; i < herbSlots.Length; i++)
        //    herbSlots[i]._CurPrice = data.herbData[i].curVal;

        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (data.itemsData[i].id != -1)
            {
                object target = DataManager.Instance.deck_Table[data.itemsData[i].id]["text_name"];
                _itemSlots[i].SetItem(SpriteList.Instance.LoadSprite(DataManager.Instance.deck_Table[data.itemsData[i].id]["prefab"].ToString()), DataManager.Instance.GetDescription(target.ToString()));
            }

            _itemSlots[i]._CurPrice = data.itemsData[i].curVal;
            _itemSlots[i].IsSoldOut = data.itemsData[i].isSoldOut;
        }

        timerList[0].CurTime = data.itemFluctCool;
        timerList[0].CurRefreshTime = data.itemRefreshCool;
        curWave = GameManager.Instance.CurWave;
    }

    public void SaveData(PlayerData data)
    {
        //data.herbData = new List<ShopData>();
        //for (int i = 0; i < herbSlots.Length; i++)
        //{
        //    ShopData herb = new ShopData();
        //    herb.curVal = herbSlots[i]._CurPrice;
        //    data.herbData.Add(herb);
        //}

        data.itemsData = new List<ShopData>();
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            ShopData item = new ShopData();
            RandomCard random = _itemSlots[i].GetComponent<RandomCard>();
            item.id = random != null ? random._TargetIndex : -1;
            item.curVal = _itemSlots[i]._CurPrice;
            item.isSoldOut = _itemSlots[i].IsSoldOut;
            data.itemsData.Add(item);
        }

        data.itemFluctCool = timerList[0].CurTime;
        data.itemRefreshCool = timerList[0].CurRefreshTime;
    }
}
