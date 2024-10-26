using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;

public class TileHidden : MonoBehaviour
{
    private TileNode _curNode;
    private GameObject _targetPrefab;

    private void ExcuteReveal()
    {
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
        Destroy(gameObject);
    }

    public void Init(TileNode curNode, GameObject targetPrefab)
    {
        _curNode = curNode;
        _targetPrefab = targetPrefab;

        transform.SetParent(_curNode.transform, false);

        NodeManager.Instance.hiddenTiles.Add(_curNode);
        NodeManager.Instance.directSightNodes.ObserveAdd().Where(_ => _.Value == _curNode).Subscribe(_ => ExcuteReveal()).AddTo(gameObject);
    }
}
