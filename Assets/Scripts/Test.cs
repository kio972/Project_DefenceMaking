using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public GameObject prefab;

    IEnumerator SpawnStart()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        for (int i = 0; i < 5000; i++)
        {
            GameObject newone = Instantiate(prefab);
            Destroy(newone.GetComponentInChildren<Battler>());
            print(i + 1);
            yield return wait;
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnStart());
    }
}
