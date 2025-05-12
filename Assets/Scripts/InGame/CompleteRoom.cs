using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class CompleteRoom
{
    private List<Tile> includeRooms;
    public int totalMana { get; private set; } = 0;

    private HashSet<MonsterSpawner> spawners = new HashSet<MonsterSpawner>();
    public List<Tile> _IncludeRooms { get => includeRooms; }
    public int _RemainingMana { get { return totalMana - _SpendedMana; } }

    bool isSingleRoom;

    CompositeDisposable disposables = new CompositeDisposable();

    public bool IsDormant
    {
        get
        {
            foreach (var rooms in _IncludeRooms)
            {
                if (rooms.IsDormant)
                    return true;
            }

            return false;
        }
    }

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

        roomManaStream.ThrottleFrame(1).Subscribe(_ => CalculateTotalMana()).AddTo(disposables);
    }

    private void CalculateTotalMana()
    {
        int totalMana = 0;
        foreach (Tile tile in includeRooms)
        {
            totalMana += tile.RoomMana;
            if(tile.objectKind is MonsterSpawner spawner)
            {
                this.spawners.Add(spawner);
            }
        }

        this.totalMana = totalMana;
    }
}
