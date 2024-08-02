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