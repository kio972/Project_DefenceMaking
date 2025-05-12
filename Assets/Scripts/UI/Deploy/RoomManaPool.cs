using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomManaPool : IngameSingleton<RoomManaPool>
{
    [SerializeField]
    private ManaUI prefab;

    private List<ManaUI> manaList = new List<ManaUI>();
    private Dictionary<CompleteRoom, RoomLineDrawer> roomLineDic = new Dictionary<CompleteRoom, RoomLineDrawer>();

    private RoomLineDrawer tempRoomLine;

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
        roomLineDrawer.Init(room._IncludeRooms);
        roomLineDic[room] = roomLineDrawer;
        return roomLineDrawer;
    }

    public void ShowManaGuide(bool value, List<Tile> rooms, bool isCompleteRoom = false)
    {
        if (tempRoomLine == null)
            tempRoomLine = Instantiate(Resources.Load<RoomLineDrawer>("Prefab/UI/RoomGuideLine"), transform);

        tempRoomLine.gameObject.SetActive(value);

        if (!value)
            return;

        ManaUI mana = GetManaUI();
        mana.Init(UtilHelper.CalculateMidTile(rooms).transform, rooms, isCompleteRoom);
        tempRoomLine.Init(rooms);
        tempRoomLine.gameObject.SetActive(true);
        if (isCompleteRoom)
            tempRoomLine.SetColor();
        else
            tempRoomLine.SetColor(Color.gray);
    }

    public void SetManaUI(bool value, List<CompleteRoom> rooms)
    {
        foreach (var item in roomLineDic.Values)
            item.gameObject.SetActive(false);

        if(value)
        {
            foreach(CompleteRoom room in rooms)
            {
                if (room.IsDormant)
                    continue;

                ManaUI mana = GetManaUI();
                mana.Init(UtilHelper.CalculateMidTile(room._IncludeRooms).transform, room);
                RoomLineDrawer roomLine = GetLineUI(room);
                roomLine.gameObject.SetActive(true);
            }
        }
    }

    public void HideAllManaText()
    {
        foreach (ManaUI obj in manaList)
            obj.gameObject.SetActive(false);
    }

    public void ShowManaText(Transform followTarget, string desc)
    {
        ManaUI mana = GetManaUI();
        mana.Init(followTarget, desc);
    }
}
