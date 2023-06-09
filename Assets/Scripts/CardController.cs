using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    MapTile,
    Monster,
    Adventurer,
}

public class CardController : MonoBehaviour
{
    public CardType cardType;
    [SerializeField]
    private GameObject targetPrefab;

    private Button button;

    private GameObject instancedObject;

    private void CallCard()
    {

    }

    public void Init(CardType type, GameObject targetPrefab)
    {
        this.cardType = type;
        this.targetPrefab = targetPrefab;
        button = GetComponent<Button>();
        button.onClick.AddListener(CallCard);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
