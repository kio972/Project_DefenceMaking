using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHidden : MonoBehaviour
{
    private TileNode _curNode;
    private GameObject _targetPrefab;

    private void OnDisable()
    {
        NodeManager.Instance.RemoveSetTileEvent(CheckReval);
    }

    private bool CheckReval(GameObject obj)
    {
        if (!NodeManager.Instance.GetDistanceNodes(1).Contains(_curNode) && !NodeManager.Instance.GetDistanceNodes(2).Contains(_curNode))
            return false;

        GameObject newObject = Instantiate(_targetPrefab);

        Tile tile = newObject.GetComponent<Tile>();
        if (tile != null)
        {
            tile.transform.SetParent(_curNode.transform, false);
            tile.Init(_curNode, true, false, false);
        }

        Environment environment = newObject.GetComponent<Environment>();
        if(environment != null)
        {
            environment.Init(_curNode);
        }

        NodeManager.Instance.hiddenTiles.Remove(_curNode);

        gameObject.SetActive(false);
        Destroy(this, 0.1f);
        return true;
    }

    public void Init(TileNode curNode, GameObject targetPrefab)
    {
        _curNode = curNode;
        _targetPrefab = targetPrefab;

        transform.SetParent(_curNode.transform, false);

        NodeManager.Instance.hiddenTiles.Add(_curNode);
        NodeManager.Instance.AddSetTileEvent(CheckReval);
    }
}
