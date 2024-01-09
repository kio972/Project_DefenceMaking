using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BattlerPooling : IngameSingleton<BattlerPooling>
{
    private List<Adventurer> adventurerPool = new List<Adventurer>();
    
    private List<Monster> monsterPool = new List<Monster>();

    private List<Trap> trapPool = new List<Trap>();

    private GameObject advernturers;
    private GameObject monsters;
    private GameObject traps;

    private Transform Adventurers
    {
        get
        {
            if(advernturers == null)
            {
                GameObject newObject = new GameObject("Adventurers");
                newObject.transform.SetParent(transform);
                advernturers = newObject;
            }
            return advernturers.transform;
        }
    }

    private Transform Monsters
    {
        get
        {
            if (monsters == null)
            {
                GameObject newObject = new GameObject("Monsters");
                newObject.transform.SetParent(transform);
                monsters = newObject;
            }
            return monsters.transform;
        }
    }

    private Transform Traps
    {
        get
        {
            if (traps == null)
            {
                GameObject newObject = new GameObject("Traps");
                newObject.transform.SetParent(transform);
                traps = newObject;
            }
            return traps.transform;
        }
    }

    private Trap GetTrapInPool(string trapId)
    {
        foreach(Trap trap in trapPool)
        {
            if (!trap.gameObject.activeSelf && trap.BattlerID == trapId)
                return trap;
        }

        return null;
    }

    public void SpawnTrap(string trapName, TileNode targetTile)
    {
        int trapIndex = UtilHelper.Find_Data_Index(trapName, DataManager.Instance.Battler_Table, "name");
        string prefab = DataManager.Instance.Battler_Table[trapIndex]["prefab"].ToString();
        string trapId = DataManager.Instance.Battler_Table[trapIndex]["id"].ToString();

        Trap trap = GetTrapInPool(trapId);

        if(trap == null)
        {
            Trap targetPrefab = Resources.Load<Trap>("Prefab/Trap/" + prefab);
            trap = Instantiate(targetPrefab, Traps);
            trapPool.Add(trap);
        }

        trap.Init(targetTile.curTile);
        trap.gameObject.SetActive(true);
    }

    private Monster GetMonsterInPool(string monsterId)
    {
        foreach (Monster monster in monsterPool)
        {
            if (!monster.gameObject.activeSelf && monster.BattlerID == monsterId)
                return monster;
        }

        return null;
    }

    public Monster SpawnMonster(string monsterName, TileNode startTile)
    {
        int monsterIndex = UtilHelper.Find_Data_Index(monsterName, DataManager.Instance.Battler_Table, "name");
        string prefab = DataManager.Instance.Battler_Table[monsterIndex]["prefab"].ToString();
        string monsterId = DataManager.Instance.Battler_Table[monsterIndex]["id"].ToString();

        Monster monster = GetMonsterInPool(monsterId);

        if(monster == null)
        {
            Monster targetPrefab = Resources.Load<Monster>("Prefab/Monster/" + prefab);
            monster = Instantiate(targetPrefab, Monsters);
            monsterPool.Add(monster);
        }

        monster.Init();
        monster.SetStartPoint(startTile);
        monster.gameObject.SetActive(true);

        return monster;
    }

    private Adventurer GetAdventurerInPool(string adventurerId)
    {
        foreach(Adventurer adventurer in adventurerPool)
        {
            if (!adventurer.gameObject.activeSelf && adventurer.BattlerID == adventurerId)
                return adventurer;
        }

        return null;
    }

    public void SpawnAdventurer(string adventurerName)
    {
        //1. 현재 adventurerPool에 해당 adventurerId의 activeSelf == false인 타겟 탐색
        //2. 1번의 타겟이 존재한다면 Init시켜주고, SetActive(true)
        //3. 존재하지 않는다면 새로 Instantiate

        int adventurerIndex = UtilHelper.Find_Data_Index(adventurerName, DataManager.Instance.Battler_Table, "name");
        string prefab = DataManager.Instance.Battler_Table[adventurerIndex]["prefab"].ToString();
        string adventurerId = DataManager.Instance.Battler_Table[adventurerIndex]["id"].ToString();

        Adventurer adventurer = GetAdventurerInPool(adventurerId);

        if(adventurer == null)
        {
            Adventurer targetPrefab = Resources.Load<Adventurer>("Prefab/Adventurer/" + prefab);
            adventurer = Instantiate(targetPrefab, Adventurers);
            adventurerPool.Add(adventurer);
        }

        adventurer.Init();
        adventurer.gameObject.SetActive(true);
    }
}
