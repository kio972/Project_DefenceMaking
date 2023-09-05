using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerPooling : IngameSingleton<AdventurerPooling>
{
    private List<Adventurer> adventurerPool = new List<Adventurer>();
    
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
            adventurer = Instantiate(targetPrefab, transform);
            adventurerPool.Add(adventurer);
        }

        adventurer.ResetNode();
        adventurer.Init();
        adventurer.gameObject.SetActive(true);
        GameManager.Instance.adventurersList.Add(adventurer);
    }
}
