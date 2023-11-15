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

    public void SetActive(bool value)
    {
        UIManager.Instance.SetTab(uiPage, value, () => { GameManager.Instance.SetPause(false); });
        GameManager.Instance.SetPause(value);
        if(value)
            SetItem();
    }

    private void SetTrap(GameObject instancedObject)
    {
        Trap trap = instancedObject.GetComponent<Trap>();
        if (trap != null)
        {
            Vector3 scale = trap.transform.lossyScale;
            trap.transform.SetParent(curNode.curTile.transform, true);
            trap.transform.localPosition = Vector3.zero;
            trap.transform.localScale = scale;
            trap.Init(curNode.curTile);
        }

        AudioManager.Instance.Play2DSound("Set_trap", SettingManager.Instance._FxVolume);
    }

    private void SetMonster(GameObject instancedObject)
    {
        Monster monster = instancedObject.GetComponent<Monster>();
        if (monster != null)
        {
            monster.SetStartPoint(curNode);
            monster.transform.position = curNode.transform.position;
            monster.Init();
            curNode.curTile.monster = monster;
        }

        AudioManager.Instance.Play2DSound("Set_monster", SettingManager.Instance._FxVolume);
    }

    public void DeployEnd()
    {
        curObject.SetActive(false);
        curObject = null;
        curType = CardType.None;
        curNode = null;

        InputManager.Instance.settingCard = false;
        NodeManager.Instance.SetGuideState(GuideState.None);
        UIManager.Instance.SetTab(uiPage, true, () => { GameManager.Instance.SetPause(false); });
        UIManager.Instance.CloseTab(exitBtn.gameObject);
        GameManager.Instance.cardLock = false;
        GameManager.Instance.SetPause(true);
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

            GameManager.Instance.gold -= curPrice;

            GameObject newObject = Instantiate(curObject);
            if (curType == CardType.Monster)
                SetMonster(newObject);
            else if (curType == CardType.Trap)
                SetTrap(newObject);

            SetGuideState(curType);
        }
    }

    public void UpdateDeployState()
    {
        UpdateObjectPosition();

        if (Input.GetKeyDown(KeyCode.Mouse0))
            SetObjectOnMap();
        else if(Input.GetKeyDown(KeyCode.Mouse1))
            DeployEnd();
    }

    public void SetGuideObject(CardType unitType, string prefabName)
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

    public void DeployReady(CardType unitType, string prefabName, int cost)
    {
        SetGuideObject(unitType, prefabName);
        curPrice = cost;
        GameManager.Instance.SetPause(false);

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
