using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class DeployUI : MonoBehaviour
{
    private bool initState = false;

    GameObject targetPrefab;

    [SerializeField]
    GameObject uiPage;

    [SerializeField]
    GameObject btnBlocker;

    public int DeployStep
    {
        get
        {
            if (!uiPage.activeSelf && !exitBtn.gameObject.activeSelf)
                return 0;
            else if (uiPage.activeSelf)
                return 1;
            else if (exitBtn.gameObject.activeSelf)
                return 2;

            return 0;
        }
    }

    private Dictionary<string, GameObject> guideObjects = new Dictionary<string, GameObject>();

    private GameObject curObject;
    private TileNode curNode;
    private CardType curType;
    private int curPrice;

    [SerializeField]
    private Button exitBtn;
    [SerializeField]
    private GameObject deployingWindow;

    private List<DeploySlot> deployItems = new List<DeploySlot>();

    private InGameUI ingameUI;

    public void UpdateMana()
    {
        foreach (DeploySlot slot in deployItems)
            slot.UpdateMana();
    }

    public void SetActive(bool value)
    {
        if (value)
            InputManager.Instance.ResetTileClick();

        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
        if (value)
            SetItem();

        if (value)
            AudioManager.Instance.Play2DSound("Recruit_Open_wood", SettingManager.Instance._FxVolume);
    }

    public void SetActive(bool value, bool updateItem = true)
    {
        if (value)
            InputManager.Instance.ResetTileClick();

        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
        if (value && updateItem)
            SetItem();

        if (value)
            AudioManager.Instance.Play2DSound("Recruit_Open_wood", SettingManager.Instance._FxVolume);
    }

    public async UniTaskVoid DeployEnd()
    {
        if (curObject == null)
            return;

        curObject.SetActive(false);
        curObject = null;
        curType = CardType.None;
        curNode = null;

        NodeManager.Instance.SetGuideState(GuideState.None);
        NodeManager.Instance.SetManaGuide(false);
        //UIManager.Instance.SetTab(uiPage, true, () => { GameManager.Instance.SetPause(false); });
        UIManager.Instance.CloseTab(deployingWindow.gameObject);
        GameManager.Instance.cardLock = false;
        GameManager.Instance.speedController.SetSpeedPrev();
        //GameManager.Instance.SetPause(true);

        if (ingameUI == null)
            ingameUI = GetComponentInParent<InGameUI>();
        ingameUI?.SetRightUI(true);
        ingameUI?.SetDownUI(true);
        btnBlocker.SetActive(false);

        await UniTask.WaitUntil(() => !Input.GetKey(KeyCode.Mouse0));
        InputManager.Instance.settingCard = false;
    }

    private bool HaveMana(CompleteRoom room, out int outMana)
    {
        Dictionary<string, object> data = DataManager.Instance.battler_Table[UtilHelper.Find_Data_Index(curObject.name, DataManager.Instance.battler_Table, "name")];
        int requireMana = Convert.ToInt32(data["requiredMagicpower"]);
        MonsterType monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), data["type"].ToString());
        requireMana -= PassiveManager.Instance._MonsterTypeReduceMana_Weight[(int)monsterType];

        outMana = requireMana;

        return room != null && room._RemainingMana >= requireMana;
    }

    private void SetObjectOnMap(bool cancel = false)
    {
        if (!cancel && curNode != null && curNode.setAvail)
        {
            if (GameManager.Instance.gold < curPrice)
            {
                GameManager.Instance.popUpMessage.ToastMsg("골드가 부족합니다");
                return;
            }

            if (curType == CardType.Spawner)
            {
                CompleteRoom room = NodeManager.Instance.GetRoomByNode(curNode);
                int requiredMana;

                if (!HaveMana(room, out requiredMana))
                {
                    GameManager.Instance.popUpMessage.ToastMsg("방의 마나가 부족합니다");
                    return;
                }

                BattlerPooling.Instance.SetSpawner(curNode, curObject.name, room);
                AudioManager.Instance.Play2DSound("Set_trap", SettingManager.Instance._FxVolume);
            }
            else if (curType == CardType.Trap)
            {
                if(GameManager.Instance.IsAdventurererOnTile(curNode))
                {
                    GameManager.Instance.popUpMessage.ToastMsg("적이 있는 타일에는 설치할 수 없습니다.");
                    return;
                }

                BattlerPooling.Instance.SpawnTrap(curObject.name, curNode);
                AudioManager.Instance.Play2DSound("Set_trap", SettingManager.Instance._FxVolume);
            }
            else if(curType == CardType.Monster)
            {
                CompleteRoom room = NodeManager.Instance.GetRoomByNode(curNode);
                int requiredMana;

                if (!HaveMana(room, out requiredMana))
                {
                    GameManager.Instance.popUpMessage.ToastMsg("방의 마나가 부족합니다");
                    return;
                }
                if (GameManager.Instance._CurMana + requiredMana > GameManager.Instance._TotalMana)
                {
                    GameManager.Instance.popUpMessage.ToastMsg("보유중인 마나가 부족합니다");
                    return;
                }

                BattlerPooling.Instance.SpawnMonster(curObject.name, curNode);
                AudioManager.Instance.Play2DSound("Set_trap", SettingManager.Instance._FxVolume);
            }

            GameManager.Instance.gold -= curPrice;
            SetGuideState(curType);

            if (!Input.GetKey(KeyCode.LeftShift))
                DeployEnd().Forget();
        }
        else if(curNode == null || curNode.curTile == null)
            DeployEnd().Forget();
    }

    public void UpdateDeployState()
    {
        UpdateObjectPosition();
        NodeManager.Instance.SetManaGuide(curType is CardType.Monster or CardType.Spawner);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetObjectOnMap();
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1))
            DeployEnd().Forget();
    }

    public void SetGuideObject(CardType unitType, string targetName, string prefabName)
    {
        GameObject guideObject = null;
        targetPrefab = UtilHelper.GetCardPrefab(unitType, prefabName);
        curType = unitType;

        if (guideObjects.ContainsKey(prefabName))
        {
            guideObject = guideObjects[prefabName];
            guideObject.SetActive(true);
        }
        else
        {
            guideObject = Instantiate(targetPrefab);
            guideObject.name = targetName;
            guideObjects.Add(prefabName, guideObject);
            Battler battler = guideObject.GetComponent<Battler>();
            if(battler != null) Destroy(battler);
        }

        if (unitType is CardType.Monster or CardType.Spawner)
        {
            Monster monster = guideObject.GetComponent<Monster>();
            monster.SetRotation();
        }

        curObject = guideObject;
    }

    private void UpdateObjectPosition()
    {
        curNode = UtilHelper.RayCastTile();

        if (curNode != null && curNode.GuideActive && curNode.curTile != null && !curNode.curTile.HaveSpawner)
        {
            curObject.transform.SetParent(curNode.transform, true);
            curObject.transform.position = curNode.transform.position;
        }
        else
        {
            curObject.transform.position = new Vector3(0, 10000, 0);
        }
    }

    private void SetGuideState(CardType unitType)
    {
        switch (unitType)
        {
            case CardType.Monster:
                NodeManager.Instance.SetGuideState(GuideState.Spawner);
                break;
            case CardType.Spawner:
                NodeManager.Instance.SetGuideState(GuideState.Spawner);
                break;
            case CardType.Trap:
                NodeManager.Instance.SetGuideState(GuideState.Trap);
                break;
        }
    }

    public void DeployReady(CardType unitType, string targetName, string prefabName, int cost)
    {
        SetGuideObject(unitType, targetName, prefabName);
        curPrice = cost;
        GameManager.Instance.SetPause(false);

        if (ingameUI == null)
            ingameUI = GetComponentInParent<InGameUI>();
        ingameUI?.SetRightUI(false);
        ingameUI?.SetDownUI(false);

        InputManager.Instance.settingCard = true;
        //함정인지 몬스터인지
        SetGuideState(curType);
        GameManager.Instance.cardLock = true;
        UIManager.Instance.CloseTab(uiPage);
        UIManager.Instance.SetTab(deployingWindow.gameObject, true, () => { DeployEnd().Forget(); });
        btnBlocker.SetActive(true);
    }

    public void SetItem()
    {
        if (!initState)
            Init();

        foreach(DeploySlot slot in deployItems)
        {
            //if (!slot.gameObject.activeSelf)
            //    continue;
            if (slot.cardType == CardType.Trap)
            {
                slot.gameObject.SetActive(true);
                continue;
            }

            slot.gameObject.SetActive(slot.IsUnlocked);
        }
    }

    private DeploySlot GetNextSlot()
    {
        foreach(DeploySlot slot in deployItems)
        {
            if (slot.gameObject.activeSelf)
                continue;

            slot.gameObject.SetActive(true);
            return slot;
        }

        DeploySlot newSlot = Instantiate(deployItems[0], deployItems[0].transform.parent);
        deployItems.Add(newSlot);
        newSlot.gameObject.SetActive(true);
        return newSlot;
    }

    public void Init()
    {
        DeploySlot[] temp = GetComponentsInChildren<DeploySlot>(true);
        deployItems = temp.ToList();
        foreach (DeploySlot slot in deployItems)
            slot.gameObject.SetActive(false);

        foreach (Dictionary<string, object> data in DataManager.Instance.battler_Table)
        {
            if (string.IsNullOrEmpty(data["prefab"].ToString()))
                continue;

            string id = data["id"].ToString();
            if (id[2] != 'm' && id[2] != 't')
                continue;
            GetNextSlot().Init(data);
        }

        deployItems[0].SendInfo();

        initState = true;
    }

    [SerializeField]
    private GameObject btnObject;

    public void Update()
    {
        if (curObject != null)
            UpdateDeployState();

        if (Input.GetKeyDown(SettingManager.Instance.key_Deploy._CurKey) && btnObject.activeSelf)
        {
            if (UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
                SetActive(true);
            else if (uiPage.activeSelf)
                SetActive(false);
        }
    }
}
