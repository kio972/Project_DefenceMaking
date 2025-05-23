using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataImport : MonoBehaviour
{
    [SerializeField]
    private TextAsset _waveData;
    [SerializeField]
    private TextAsset _timeRateData;
    [SerializeField]
    private TextAsset _deckListData;
    [SerializeField]
    private TextAsset _scriptData;
    [SerializeField]
    private TextAsset _questData;
    [SerializeField]
    private TextAsset _questMessageData;
    [SerializeField]
    private TextAsset _hiddenTileData;

    private void Awake()
    {
        DataManager.Instance.SetStageData(_waveData, _timeRateData, _deckListData, _scriptData, _questData, _questMessageData, _hiddenTileData);
    }
}
