using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public enum ArrowColor
{
    None,
    Red,
    Green,
}

public interface ITileArrowEffect
{
    ArrowColor GetArrowColor(ITileKind target);
}

public interface ITileManaEffect : IManaSupply
{
    string GetManaText(ITileKind target);
}

public class TileArrowUI : MonoBehaviour
{
    [SerializeField]
    private SerializedDictionary<Direction, Image> arrowDic;

    [SerializeField]
    private SerializedDictionary<ArrowColor, Sprite> arrowSprites;

    public void SetOFF()
    {
        gameObject.SetActive(false);
    }

    public void SetTileArrow(Tile target)
    {
        gameObject.SetActive(true);
        foreach (var kvp in target.curNode.neighborNodeDic)
        {
            ArrowColor arrowColor = ArrowColor.None;
            if (kvp.Value.tileKind != null && kvp.Value.tileKind is ITileArrowEffect arrowEffect)
                arrowColor = arrowEffect.GetArrowColor(target);
            arrowDic[kvp.Key].gameObject.SetActive(arrowColor != ArrowColor.None);
            if (arrowSprites.ContainsKey(arrowColor))
                arrowDic[kvp.Key].sprite = arrowSprites[arrowColor];
        }
    }

    public void SetEnvironmentArrow(Environment target, TileNode targetNode)
    {
        gameObject.SetActive(true);
        if (target is ITileArrowEffect arrowEffect)
        {
            foreach(var kvp in targetNode.neighborNodeDic)
            {
                ArrowColor arrowColor = ArrowColor.None;
                if (kvp.Value.tileKind != null)
                    arrowColor = arrowEffect.GetArrowColor(kvp.Value.tileKind);
                arrowDic[kvp.Key].gameObject.SetActive(arrowColor != ArrowColor.None);
                if (arrowSprites.ContainsKey(arrowColor))
                    arrowDic[kvp.Key].sprite = arrowSprites[arrowColor];
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetArrow(ITileKind target, TileNode targetNode)
    {
        transform.position = targetNode.transform.position;
        if(target is Environment environment)
        {
            SetEnvironmentArrow(environment, targetNode);
        }
    }
}
