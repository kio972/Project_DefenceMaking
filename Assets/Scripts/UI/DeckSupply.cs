using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeckSupply : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    CardDeckController deckController;

    private int price = 5;

    private Coroutine position_Modify_Coroutine = null;

    public Vector3 originPos;
    [SerializeField]
    private float mouseOverTime = 0.2f;

    [SerializeField]
    private GameObject twin;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (position_Modify_Coroutine != null)
            StopCoroutine(position_Modify_Coroutine);
        position_Modify_Coroutine = StartCoroutine(UtilHelper.IMoveEffect(transform, transform.position, new Vector3(originPos.x, originPos.y - 30, originPos.z), mouseOverTime));

        AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (position_Modify_Coroutine != null)
            StopCoroutine(position_Modify_Coroutine);
        position_Modify_Coroutine = StartCoroutine(UtilHelper.IMoveEffect(transform, transform.position, originPos, mouseOverTime));
    }

    private void ResetTwin()
    {
        twin.gameObject.SetActive(false);
        twin.transform.position = transform.position;
    }

    private void DeckSupplyEffect()
    {
        if (deckController == null || twin == null)
            return;

        twin.gameObject.SetActive(true);
        StartCoroutine(UtilHelper.IMoveEffect(twin.transform, originPos, GameManager.Instance.cardDeckController.transform.position, 0.4f));
        StartCoroutine(UtilHelper.IScaleEffect(twin.transform, Vector3.one, Vector3.zero, 0.4f, () => { ResetTwin(); }));
    }

    public void SupplyPathCard()
    {
        if (deckController == null || GameManager.Instance.gold < price)
            return;

        GameManager.Instance.gold -= price;

        deckController.AddCard(DataManager.Instance.PathCard_Indexs[Random.Range(0, DataManager.Instance.PathCard_Indexs.Count)]);
        DeckSupplyEffect();
    }

    public void SupplyRoomCard()
    {
        if (deckController == null || GameManager.Instance.gold < price)
            return;

        GameManager.Instance.gold -= price;
        
        List<int> pool = new List<int>(DataManager.Instance.RoomCard_Indexs);
        foreach (int val in DataManager.Instance.RoomPartCard_Indexs)
            pool.Add(val);

        deckController.AddCard(pool[Random.Range(0, pool.Count)]);
        DeckSupplyEffect();
    }

    public void SupplyEnvironmentCard()
    {
        if (deckController == null || GameManager.Instance.gold < price)
            return;

        GameManager.Instance.gold -= price;

        deckController.AddCard(DataManager.Instance.EnvironmentCard_Indexs[Random.Range(0, DataManager.Instance.EnvironmentCard_Indexs.Count)]);
        DeckSupplyEffect();
    }

    void Awake()
    {
        originPos = transform.position;
    }
}
