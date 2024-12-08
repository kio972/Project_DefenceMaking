using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManaPool : IngameSingleton<RoomManaPool>
{
    [SerializeField]
    private ManaUI prefab;

    private List<ManaUI> manaList = new List<ManaUI>();
    private Dictionary<CompleteRoom, RoomLineDrawer> roomLineDic = new Dictionary<CompleteRoom, RoomLineDrawer>();

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

    private RoomLineDrawer GetLineUI(CompleteRoom room)
    {
        if(roomLineDic.ContainsKey(room))
            return roomLineDic[room];

        RoomLineDrawer roomLineDrawer = Resources.Load<RoomLineDrawer>("Prefab/UI/RoomGuideLine");
        roomLineDrawer = Instantiate(roomLineDrawer, transform);
        roomLineDrawer.Init(room);
        roomLineDic[room] = roomLineDrawer;
        return roomLineDrawer;
    }

    public void SetManaUI(bool value, List<CompleteRoom> rooms)
    {
        foreach (ManaUI obj in manaList)
            obj.gameObject.SetActive(false);
        foreach(var item in roomLineDic.Values)
            item.gameObject.SetActive(false);

        if(value)
        {
            foreach(CompleteRoom room in rooms)
            {
                if (room.IsDormant)
                    continue;

                ManaUI mana = GetManaUI();
                mana.Init(room.HeadRoom.transform, room);
                RoomLineDrawer roomLine = GetLineUI(room);
                roomLine.gameObject.SetActive(true);
            }
        }
    }
}
