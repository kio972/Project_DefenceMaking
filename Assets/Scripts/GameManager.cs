using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float defaultSpeed = 1000f;

    float timer = 0f;
    public float Timer { get => timer; }

    public int gold = 0;


    public int startCardNumber = 6;
    public CardDeckController cardDeckController;

    public GameSpeedController speedController;


    public int king_Hp = 20;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * defaultSpeed;
        if(timer > 1440f)
        {
            //재화수급
            gold += 200;
            timer = 0f;
        }
    }

    private void Start()
    {
        for(int i = 0; i < startCardNumber; i++)
        {
            cardDeckController.FreeDrawCard();
        }

        speedController.SetSpeedZero();
    }
}
