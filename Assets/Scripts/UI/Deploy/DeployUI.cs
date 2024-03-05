using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeployUI : MonoBehaviour
{
    private bool initState = false;

    GameObject targetPrefab;

    [SerializeField]
    GameObject uiPage;
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

    private List<DeploySlot> deployItems = new List<DeploySlot>();

    private InGameUI ingameUI;

    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
        if (value)
            SetItem();
    }

    public void SetActive(bool value, bool updateItem = true)
    {
        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
        if(value && updateItem)
            SetItem();
    }

    public void DeployEnd()
    {
        if (curObject == null)
            return;

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

    private bool HaveMana(CompleteRoom room, out int outMana)
    {
        Dictionary<string, object> data = DataManager.Instance.Battler_Table[UtilHelper.Find_Data_Index(curObject.name, DataManager.Instance.Battler_Table, "name")];
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
                GameManager.Instance.popUpMessage.ToastMsg("��尡 �����մϴ�");
                return;
            }

            if (curType == CardType.Monster)
            {
                CompleteRoom room = NodeManager.Instance.GetRoomByNode(curNode);
                int requiredMana;

                if (!HaveMana(room, out requiredMana))
                {
                    GameManager.Instance.popUpMessage.ToastMsg("���� ������ �����մϴ�");
                    return;
                }

                room.spendedMana += requiredMana;
                //BattlerPooling.Instance.SpawnMonster(curObject.name, curNode);
                MonsterSpawner monsterSpawner = Resources.Load<MonsterSpawner>("Prefab/Monster/MonsterSpawner");
                monsterSpawner = Instantiate(monsterSpawner, GameManager.Instance.worldCanvas.transform);
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

            //DeployEnd();
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
        //�������� ��������
        SetGuideState(curType);
        GameManager.Instance.cardLock = true;
        UIManager.Instance.CloseTab(uiPage);
        UIManager.Instance.SetTab(exitBtn.gameObject, true, () => { DeployEnd(); }); 
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
