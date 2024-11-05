using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseTileBuilder
{
    void BuildBaseTile(TileNode targetNode);
}

public class TileReward : MonoBehaviour, IBaseTileBuilder
{
    [SerializeField]
    private List<GameObject> targetTiles;
    [SerializeField]
    private bool isUpgraded;

    public void BuildBaseTile(TileNode targetNode)
    {
        if (targetTiles == null || targetTiles.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        int randomIndex = Random.Range(0, targetTiles.Count);
        GameObject targetPrefab = targetTiles[randomIndex];

        GameObject newObject = Instantiate(targetPrefab);
        newObject.transform.SetParent(targetNode.transform, false);
        
        Tile tile = newObject.GetComponent<Tile>();
        if (tile != null)
        {
            tile.Init(targetNode, true, false, false);
            if (isUpgraded)
            {
                TileUpgrader upgrade = tile.GetComponent<TileUpgrader>();
                upgrade?.UpgradeTile();
            }
        }

        Environment environment = newObject.GetComponent<Environment>();
        if (environment != null)
            environment.Init(targetNode);
    }
}