using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextWaveInfoSlot : MonoBehaviour
{
    [SerializeField]
    private List<Image> targetImgs = new List<Image>();
    [SerializeField]
    private GameObject lowCount;
    [SerializeField]
    private GameObject midCount;
    [SerializeField]
    private GameObject highCount;

    public void SetWaveInfo(WaveData waveData)
    {
        Sprite sprite = SpriteList.Instance.LoadSprite(waveData.adventurerName);
        foreach (Image im in targetImgs)
            im.sprite = sprite;

        lowCount.SetActive(true);
        midCount.SetActive(waveData.number > 5);
        highCount.SetActive(waveData.number > 10);
    }
}
