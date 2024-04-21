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
            startEnvironmentCount = NodeManager.Instance.environments.Count;
        curClearNum[0] = NodeManager.Instance.environments.Count - startEnvironmentCount;
        if (curClearNum[0] >= Mathf.Abs(_ClearNum[0]))
            isComplete[0] = true;
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 100;
    }
}

public class Quest2004 : Quest
{
    private int startHerbCount1 = -1;
    private int startHerbCount2 = -1;
    private int startHerbCount3 = -1;

    public override void CheckCondition()
    {
        if(startHerbCount1 == -1)
        {
            startHerbCount1 = GameManager.Instance.herb1;
            startHerbCount2 = GameManager.Instance.herb2;
            startHerbCount3 = GameManager.Instance.herb3;
        }

        curClearNum[0] = GameManager.Instance.herb1 - startHerbCount1;
        curClearNum[1] = GameManager.Instance.herb2 - startHerbCount2;
        curClearNum[2] = GameManager.Instance.herb3 - startHerbCount3;

        for(int i = 0; i < curClearNum.Count; i++)
        {
            if (curClearNum[i] >= Mathf.Abs(_ClearNum[i]))
                isComplete[i] = true;
        }
    }

    public override void CompleteQuest()
    {
        base.CompleteQuest();
        GameManager.Instance.gold += 2000;
    }
}

