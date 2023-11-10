using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

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

    private void SetTrap(GameObject instancedObject)
    {
        Trap trap = instancedObject.GetComponent<Trap>();
        if (trap != null)
        {
            trap.transform.SetParent(curNode.curTile.transform);
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
        curObject = null;
        curType = CardType.None;
        curNode = null;

        InputManager.Instance.settingCard = false;
        NodeManager.Instance.SetGuideState(GuideState.None);
        uiPage.SetActive(true);
        exitBtn.gameObject.SetActive(false);
        GameManager.Instance.cardLock = false;
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

        if(Input.GetKeyDown(KeyCode.Mouse0))
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
            curObject.transform.SetParent(curNode.transform, false);
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

    public void DeployReady(CardType unitType, string prefabName)
    {
        SetGuideObject(unitType, prefabName);

        InputManager.Instance.settingCard = true;
        //함정인지 몬스터인지
        SetGuideState(curType);
        GameManager.Instance.cardLock = true;
        uiPage.SetActive(false);
        exitBtn.gameObject.SetActive(true);
    }

    public void SetItem()
    {
        if (!initState)
            Init();

        if(GameManager.Instance.researchLevel == 0)
        {

        }
    }

    public void Init()
    {
        foreach(Dictionary<string, object> data in DataManager.Instance.Battler_Table)
        {
            string id = data["id"].ToString();
            if (id[2] is not 'm' and 't')
                continue;


        }


        initState = true;
    }

    public void Update()
    {
        if (curObject != null)
            UpdateDeployState();
    }
}
