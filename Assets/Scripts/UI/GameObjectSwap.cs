using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISwappableGameObject
{
    void SetActive(bool value);
}

public class GameObjectSwap : MonoBehaviour
{
    ISwappableGameObject[] swapableObjects;

    public void SwapObject(ISwappableGameObject swapableObject)
    {
        foreach(var obj in swapableObjects)
            obj.SetActive(false);

        if(UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
            swapableObject.SetActive(true);
    }

    private void Awake()
    {
        swapableObjects = GetComponentsInChildren<ISwappableGameObject>(true);
    }
}
