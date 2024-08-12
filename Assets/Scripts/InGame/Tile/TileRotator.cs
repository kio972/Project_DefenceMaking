using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TileRotator : MonoBehaviour
{
    private Tile tile;
    [SerializeField]
    private SpriteRenderer targetImage;
    [SerializeField]
    private List<Sprite> sprites;

    private void SetRotation(int rotationCount)
    {
        float rate = -60f * rotationCount;
        transform.localRotation = Quaternion.Euler(0f, rate, 0f);

        if (targetImage != null && rotationCount < sprites.Count)
            targetImage.sprite = sprites[rotationCount];
    }

    public void Start()
    {
        if (tile == null)
            tile = GetComponentInParent<Tile>();

        if(tile != null)
            tile.rotationCount.Subscribe(_ => SetRotation(_)).AddTo(gameObject);
    }
}
