using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CutSceneStarter : MonoBehaviour
{
    [SerializeField]
    TextAsset jsonFile;

    [SerializeField]
    Monster goblin;


    private List<(string targetName, int row, int col)> spawnerLists = new List<(string targetName, int row, int col)>
    {
        ("slime_mucus", -2, 7),
        ("slime_mucus", -1, 5),
        ("slime_poison", -2, 3),
        ("slime_poison", -2, 2),
        ("slime_poison", -2, 1),
        ("goblin_archer", 4, 3),
        ("goblin_warrior", 1, 9),
        ("goblin_scout", -1, 7),
        ("slime_explosion", 4, 6),
        ("golem_iron", 3, 8),
        ("golem_mithril", -1, 11),
        ("goblin_warrior", -1, 15),
    };

    private int curIndex = 0;

    private async UniTaskVoid ExcuteSpawn()
    {
        BattlerPooling.Instance.SpawnAdventurer("warrior_normal");
        await UniTask.WaitForSeconds(1);
        BattlerPooling.Instance.SpawnAdventurer("archer_normal");
        await UniTask.WaitForSeconds(1);
        BattlerPooling.Instance.SpawnAdventurer("thief_normal");

        await UniTask.WaitForSeconds(2);
        BattlerPooling.Instance.SpawnAdventurer("warrior_normal");
        await UniTask.WaitForSeconds(1);
        BattlerPooling.Instance.SpawnAdventurer("warrior_normal");
        await UniTask.WaitForSeconds(1);
        BattlerPooling.Instance.SpawnAdventurer("warrior_normal");
    }

    public void SpawnWave()
    {
        ExcuteSpawn().Forget();
    }

    public void SetSpawner()
    {
        var cur = spawnerLists[curIndex];
        TileNode node = NodeManager.Instance.FindNode(cur.row, cur.col);
        var room = NodeManager.Instance.FindRoom(cur.row, cur.col);
        BattlerPooling.Instance.SetSpawner(node, cur.targetName, room);

        curIndex++;
    }

    public void SetGoblinToMap()
    {
        Vector3 pos = goblin.transform.position;
        TileNode curNode = NodeManager.Instance.FindNode(-1, 16);
        goblin.enabled = true;
        goblin.SetStartPoint(curNode);
        goblin.Init();
        goblin.transform.position = pos;
    }

    void Start()
    {
        if (jsonFile == null)
            return;


        PlayerData playerData = SaveManager.Instance.LoadDataFromJsonFile<PlayerData>(jsonFile);
        GameManager.Instance.LoadGame(playerData);
        foreach(TileNode node in NodeManager.Instance._ActiveNodes)
        {
            if(node.curTile != null)
                node.curTile.gameObject.SetActive(false);
            if(node.environment != null)
                node.environment.gameObject.SetActive(false);
        }

        foreach(TileNode node in NodeManager.Instance.hiddenTiles)
        {
            TileHidden hidden = node.GetComponentInChildren<TileHidden>();
            hidden.gameObject.SetActive(false);
        }

        GameManager.Instance.speedController.SetSpeedNormal();
    }
}
