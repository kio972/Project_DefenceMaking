using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCard : MonoBehaviour, Item, Refreashable
{
    [SerializeField]
    private TileType targetType;

    private int targetIndex;

    private int GetRandomIndex(List<int> target)
    {
        int randomIndex = Random.Range(0, target.Count);

        return target[randomIndex];
    }

    public void UseItem()
    {
        GameManager.Instance.cardDeckController.AddCard(targetIndex);
        GameManager.Instance.cardDeckController.DrawCard(targetIndex);
    }

    public void RefreashItem()
    {
        switch (targetType)
        {
            case TileType.Path:
                targetIndex = GetRandomIndex(DataManager.Instance.PathCard_Indexs);
                break;
            case TileType.Room:
                targetIndex = GetRandomIndex(DataManager.Instance.RoomTypeCard_Indexs);
                break;
            case TileType.Environment:
                targetIndex = GetRandomIndex(DataManager.Instance.EnvironmentCard_Indexs);
                break;
        }
    }
}
