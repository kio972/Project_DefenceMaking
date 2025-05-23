using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Item 
{
    public void UseItem();
}

public interface IRefreshableItem
{
    public void RefreshItem();
}

public interface IMalPoongSunOnClick
{
    public void PlayOnClickScript();
}

public interface INeedUnlockItem
{
    public bool IsUnlock { get; }
}

public interface IInfoChangeableItem
{
    public ItemInfo Info { get; }
    public object additional { get; }
}