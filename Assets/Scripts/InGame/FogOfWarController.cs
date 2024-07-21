using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FischlWorks_FogWar;

public class FogOfWarController : MonoBehaviour
{
    csFogWar csFogWar;
    [SerializeField]
    private int tileSight = 2;

    private Dictionary<GameObject, int> tileSightDic = new Dictionary<GameObject, int>();

    private List<GameObject> tileSights = new List<GameObject>();

    private bool AddFogReveal(GameObject tileObject)
    {
        if (tileObject == null || csFogWar == null)
            return false;

        csFogWar.FogRevealer newSight = new csFogWar.FogRevealer(tileObject.transform, tileSight, false);
        int index = csFogWar.AddFogRevealer(newSight);
        //tileSightDic.Add(tileObject, index);
        tileSights.Add(tileObject);
        return true;
    }

    private bool RemoveFogReveal(GameObject tileObject)
    {
        if (tileObject == null || csFogWar == null)
            return false;

        //int index = tileSightDic[tileObject];
        //csFogWar.RemoveFogRevealer(index);
        //tileSightDic.Remove(tileObject);
        int index = tileSights.FindIndex(x => x == tileObject);
        if (index == -1)
            return false;
        csFogWar.RemoveFogRevealer(index);
        tileSights.Remove(tileObject);

        return true;
    }

    private void Awake()
    {
        csFogWar = GetComponent<csFogWar>();
        NodeManager.Instance.AddSetTileEvent(AddFogReveal);
        NodeManager.Instance.AddRemoveTileEvent(RemoveFogReveal);
    }
}
