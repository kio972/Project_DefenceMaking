using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Item 
{
    public void UseItem();
}

public interface Refreshable
{
    public void RefreshItem();
}