using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class CompleteRoom
{
    private List<Tile> includeRooms;
    public int totalMana { get; private set; } = 0;

    private List<MonsterSpawner> spawners = new List<MonsterSpawner>();
    public List<Tile> _IncludeRooms { get => includeRooms; }
    public int _RemainingMana { get { return totalMana - _SpendedMana; } }
    public Tile HeadRoom { get { return CalculateMidTile(); } }

    bool isSingleRoom;

    CompositeDisposable disposables = new CompositeDisposable();

    private int _SpendedMana
    {
        get
        {
            int total = 0;
            foreach (MonsterSpawner spawner in spawners)
                total += spawner._RequiredMana;
            return total;
        }
    }

    public void SetSpawner(MonsterSpawner spawner, bool value)
    {
        if (value)
            spawners.Add(spawner);
        else
            spawners.Remove(spawner);
    }

    private Tile CalculateMidTile()
    {
        if (includeRooms.Count == 1)
            return includeRooms[0];

        Vector3 vector = Vector3.zero;
        foreach (Tile tile in includeRooms)
            vector += tile.transform.position;

        vector = vector / includeRooms.Count;

        Tile resultTile = includeRooms[0];
        float minMagnitude = (vector - includeRooms[0].transform.position).magnitude;

        foreach(Tile tile in includeRooms)
        {
            float targetMagnitude = (vector - tile.transform.position).magnitude;
            if(targetMagnitude < minMagnitude)
            {
                minMagnitude = targetMagnitude;
                resultTile = tile;
            }
        }

        return resultTile;
    }

    ~CompleteRoom()
    {
        disposables.Dispose();
    }

    public CompleteRoom(List<Tile> targetTiles, bool singleRoom = false)
    {
        includeRooms = targetTiles;
        this.isSingleRoom = singleRoom;
        CalculateTotalMana();

        var roomManaStream = targetTiles[0].roomManaProperty.ObserveEveryValueChanged(_ => _.Value);
        for(int i = 1; i < targetTiles.Count; i++)
        {
            var nextManaStream = targetTiles[i].roomManaProperty.ObserveEveryValueChanged(_ => _.Value);
            roomManaStream = Observable.Merge(roomManaStream, nextManaStream);
        }

        roomManaStream.ThrottleFrame(1).Subscribe(_ => CalculateTotalMana()).AddTo(targetTiles[0].gameObject);
    }

    private void CalculateTotalMana()
    {
        int totalMana = 0;
        foreach (Tile tile in includeRooms)
        {
            totalMana += tile.RoomMana;
        }

        this.totalMana = totalMana;
    }
}
