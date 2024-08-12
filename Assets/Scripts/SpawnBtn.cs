using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpawnBtn : MonoBehaviour
{
    private Button btn;
    [SerializeField]
    private string targetId;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => { BattlerPooling.Instance.SpawnAdventurer(targetId, "id"); });
    }
}
