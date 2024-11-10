using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;

public class TileHidden : MonoBehaviour, ITileKind
{
    private TileNode _curNode;
    public TileNode curNode { get => _curNode; }
    private GameObject _targetPrefab;

    private void ExcuteReveal()
    {
        if(NodeManager.Instance.hiddenPrioritys.Count > 0)
            _targetPrefab = NodeManager.Instance.hiddenPrioritys.Dequeue();
        else
            _targetPrefab = GetPrefab();

        if(_targetPrefab != null)
        {
            GameObject newObject = Instantiate(_targetPrefab, _curNode.transform, true);
            newObject.transform.position = _curNode.transform.position;
            IBaseTileBuilder tileBuilder = newObject.GetComponent<IBaseTileBuilder>();
            tileBuilder?.BuildBaseTile(_curNode);
        }

        NodeManager.Instance.hiddenTiles.Remove(_curNode);
        AudioManager.Instance.Play2DSound("Demon_Attack_B", SettingManager.Instance._FxVolume);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private string GetTargetName()
    {
        Dictionary<string, object> data = DataManager.Instance.hiddenTile_WaveTable[GameManager.Instance.CurWave];
        int totalToken = 0;
        foreach (var kvp in data)
        {
            if (kvp.Key == "level")
                continue;
            totalToken += System.Convert.ToInt32(kvp.Value);
        }

        int randomValue = Random.Range(0, totalToken);
        int cumulative = 0;
        // GameObject와 그 int 값을 반복하여 확률적 선택
        foreach (var kvp in data)
        {
            if (kvp.Key == "level")
                continue;
            int value = System.Convert.ToInt32(kvp.Value);
            cumulative += value;
            if (randomValue < cumulative && value != 0)
                return kvp.Key;
        }

        return null;
    }

    private GameObject GetPrefab()
    {
        string targetName = GetTargetName();

        GameObject targetPrefab = Resources.Load<GameObject>("Prefab/Objects/" + targetName);

        return targetPrefab;
    }

    public void Init(TileNode curNode)
    {
        _curNode = curNode;

        transform.SetParent(_curNode.transform, false);

        NodeManager.Instance.hiddenTiles.Add(_curNode);
        NodeManager.Instance.directSightNodes.ObserveAdd().Where(_ => _.Value == _curNode).Subscribe(_ => ExcuteReveal()).AddTo(gameObject);
    }
}
