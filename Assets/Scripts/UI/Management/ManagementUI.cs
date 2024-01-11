using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagementUI : MonoBehaviour
{
    private bool initState = false;

    GameObject targetPrefab;

    [SerializeField]
    GameObject uiPage;

    private Dictionary<string, GameObject> guideObjects = new Dictionary<string, GameObject>();

    private GameObject curObject;
    private TileNode curNode;
    private CardType curType;
    private int curPrice;

    [SerializeField]
    private Button exitBtn;

    private List<ManageSlot> deployItems = new List<ManageSlot>();

    private InGameUI ingameUI;

    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
        if(value)
            SetItem();
    }

    public void DeployEnd()
    {
        curObject.SetActive(false);
        curObject = null;
        curType = CardType.None;
        curNode = null;

        NodeManager.Instance.SetManaGuide(false);
        InputManager.Instance.settingCard = false;
        NodeManager.Instance.SetGuideState(GuideState.None);
        UIManager.Instance.SetTab(uiPage, true, () => { GameManager.Instance.SetPause(false); });
        UIManager.Instance.CloseTab(exitBtn.gameObject);
        GameManager.Instance.cardLock = false;
        GameManager.Instance.SetPause(true);

        if (ingameUI == null)
            ingameUI = GetComponentInParent<InGameUI>();
        ingameUI?.SetRightUI(true);
        ingameUI?.SetDownUI(true);
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

            if (curType == CardType.Monster)
            {
                Dictionary<string, object> data = DataManager.Instance.Battler_Table[UtilHelper.Find_Data_Index(curObject.name, DataManager.Instance.Battler_Table, "name")];
                int requireMana = Convert.ToInt32(data["requiredMagicpower"]);
                CompleteRoom room = NodeManager.Instance.GetRoomByNode(curNode);
                if(room == null || room._RemainingMana < requireMana)
                {
                    GameManager.Instance.popUpMessage.ToastMsg("방의 마나가 부족합니다");
                    return;
                }

                room.spendedMana += requireMana;
                //BattlerPooling.Instance.SpawnMonster(curObject.name, curNode);
                MonsterSpawner monsterSpawner = Resources.Load<MonsterSpawner>("Prefab/Monster/MonsterSpawner");
                monsterSpawner = Instantiate(monsterSpawner, GameManager.Instance.cameraCanvas.transform);
                monsterSpawner.Init(curNode, curObject.name);
                AudioManager.Instance.Play2DSound("Set_monster", SettingManager.Instance._FxVolume);
            }
            else if (curType == CardType.Trap)
            {
                BattlerPooling.Instance.SpawnTrap(curObject.name, curNode);
                AudioManager.Instance.Play2DSound("Set_trap", SettingManager.Instance._FxVolume);
            }

            GameManager.Instance.gold -= curPrice;
            SetGuideState(curType);
        }
    }

    public void UpdateDeployState()
    {
        UpdateObjectPosition();
        NodeManager.Instance.SetManaGuide(curType == CardType.Monster);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            SetObjectOnMap();
        else if(Input.GetKeyDown(KeyCode.Mouse1))
            DeployEnd();
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
        }

        if (unitType == CardType.Monster)
        {
            Monster monster = guideObject.GetComponent<Monster>();
            monster.SetRotation();
        }

        curObject = guideObject;
    }

    private void UpdateObjectPosition()
    {
        curNode = UtilHelper.RayCastTile();

        if (curNode != null && curNode.GuideActive)
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
                NodeManager.Instance.SetGuideState(GuideState.Monster);
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
        UIManager.Instance.SetTab(exitBtn.gameObject, true, () => { DeployEnd(); }); 
    }

    public void SetItem()
    {
        if (!initState)
            Init();
    }

    private ManageSlot GetNextSlot()
    {
        foreach(ManageSlot slot in deployItems)
        {
            if (slot.gameObject.activeSelf)
                continue;

            slot.gameObject.SetActive(true);
            return slot;
        }

        ManageSlot newSlot = Instantiate(deployItems[0], deployItems[0].transform.parent);
        deployItems.Add(newSlot);
        newSlot.gameObject.SetActive(true);
        return newSlot;
    }

    public void Init()
    {
        ManageSlot[] temp = GetComponentsInChildren<ManageSlot>(true);
        deployItems = temp.ToList();
        foreach (ManageSlot slot in deployItems)
            slot.gameObject.SetActive(false);

        foreach (Dictionary<string, object> data in DataManager.Instance.Battler_Table)
        {
            string id = data["id"].ToString();
            if (id[2] != 'm' && id[2] != 't')
                continue;
            GetNextSlot().Init(data);
        }

        deployItems[0].SendInfo();

        initState = true;
    }

    public void Update()
    {
        if (curObject != null)
            UpdateDeployState();
    }
}
