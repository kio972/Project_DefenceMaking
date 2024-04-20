using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest2001 : Quest
{
    private int startPathCount = -1;

    public override void CheckCondition()
    {
        if (startPathCount == -1)
            startPathCount = NodeManager.Instance.tileDictionary[TileType.Path].Count;
        curClearNum[0] = NodeManager.Instance.tileDictionary[TileType.Path].Count - startPathCount;
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 100;
    }
}

public class Quest2002 : Quest
{
    private int startRoomCount = -1;

    public override void CheckCondition()
    {
        if (startRoomCount == -1)
            startRoomCount = NodeManager.Instance.tileDictionary[TileType.Room].Count + NodeManager.Instance.tileDictionary[TileType.Room_Single].Count + NodeManager.Instance.tileDictionary[TileType.Door].Count;
        curClearNum[0] = NodeManager.Instance.tileDictionary[TileType.Room].Count + NodeManager.Instance.tileDictionary[TileType.Room_Single].Count + NodeManager.Instance.tileDictionary[TileType.Door].Count - startRoomCount;
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 100;
    }
}

public class Quest2003 : Quest
{
    private int startEnvironmentCount = -1;

    public override void CheckCondition()
    {
        if (startEnvironmentCount == -1)
            startEnvironmentCount = NodeManager.Instance.tileDictionary[TileType.Environment].Count;
        curClearNum[0] = NodeManager.Instance.tileDictionary[TileType.Environment].Count - startEnvironmentCount;
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 100;
    }
}

