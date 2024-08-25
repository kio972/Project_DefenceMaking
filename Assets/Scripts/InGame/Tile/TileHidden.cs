using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class TileHidden : MonoBehaviour
{
    private TileNode _curNode;
    private GameObject _targetPrefab;

    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    private void OnDestroy()
    {
        tokenSource.Cancel();
        tokenSource.Dispose();
    }

    private bool CheckReval(GameObject obj)
    {
        CheckReval().Forget();
        return true;
    }

    private async UniTaskVoid CheckReval()
    {
        if (!gameObject.activeSelf)
            return;

        if (!NodeManager.Instance.GetDistanceNodes(1).Contains(_curNode) && !NodeManager.Instance.GetDistanceNodes(2).Contains(_curNode))
            return;

        GameObject newObject = Instantiate(_targetPrefab);

        Tile tile = newObject.GetComponent<Tile>();
        if (tile != null)
        {
            tile.transform.SetParent(_curNode.transform, false);
            tile.Init(_curNode, true, false, false);
        }

        Environment environment = newObject.GetComponent<Environment>();
        if (environment != null)
        {
            environment.Init(_curNode);
        }

        NodeManager.Instance.hiddenTiles.Remove(_curNode);
        AudioManager.Instance.Play2DSound("Demon_Attack_B", SettingManager.Instance._FxVolume);

        gameObject.SetActive(false);
        await UniTask.Yield();
        NodeManager.Instance.RemoveSetTileEvent(CheckReval);
        Destroy(gameObject);
    }

    public async UniTaskVoid Init(TileNode curNode, GameObject targetPrefab)
    {
        _curNode = curNode;
        _targetPrefab = targetPrefab;

        transform.SetParent(_curNode.transform, false);

        NodeManager.Instance.hiddenTiles.Add(_curNode);
        await UniTask.Yield();
        NodeManager.Instance.AddSetTileEvent(CheckReval);
    }
}
