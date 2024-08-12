using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FischlWorks_FogWar;

public class FogOfWarController : MonoBehaviour
{
    csFogWar csFogWar;
    [SerializeField]
    private int tileSight = 2;
    private List<GameObject> tileSights = new List<GameObject>();

    private bool AddFogReveal(GameObject tileObject)
    {
        if (tileObject == null || csFogWar == null)
            return false;

        Tile tile = tileObject.GetComponent<Tile>();
        if (tile != null && tile.IsDormant)
            return false;

        csFogWar.FogRevealer newSight = new csFogWar.FogRevealer(tileObject.transform, tileSight, false);
        int index = csFogWar.AddFogRevealer(newSight);
        tileSights.Add(tileObject);
        return true;
    }

    private bool RemoveFogReveal(GameObject tileObject)
    {
        if (tileObject == null || csFogWar == null)
            return false;

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
