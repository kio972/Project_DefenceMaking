using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CardPackEffect : MonoBehaviour
{
    [SerializeField]
    private Image packImg;

    [SerializeField]
    private List<Sprite> packSprites;

    [SerializeField]
    private CardUIEffect cardEx;

    private List<CardUIEffect> cards;

    public void ShowEffect(List<int> cardIds)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
