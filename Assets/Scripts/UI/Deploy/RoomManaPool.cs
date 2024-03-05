using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManaPool : IngameSingleton<RoomManaPool>
{
    [SerializeField]
    private ManaUI prefab;

    private List<ManaUI> manaList = new List<ManaUI>();
    
    private ManaUI GetManaUI()
    {
        foreach(ManaUI obj in manaList)
        {
            if (obj.gameObject.activeSelf)
                continue;
            obj.gameObject.SetActive(true);
            return obj;
        }

        ManaUI newManaUI = Instantiate(prefab, transform);
        manaList.Add(newManaUI);
        return newManaUI;
    }

    public void SetManaUI(bool value, List<CompleteRoom> rooms)
    {
        foreach (ManaUI obj in manaList)
            obj.gameObject.SetActive(false);

        if(value)
        {
            foreach(CompleteRoom room in rooms)
            {
                ManaUI mana = GetManaUI();
                mana.Init(room.HeadRoom.transform, room);
            }
        }
    }
}
