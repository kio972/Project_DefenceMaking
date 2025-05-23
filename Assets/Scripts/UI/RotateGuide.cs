using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.XR;

public class RotateGuide : MonoBehaviour
{
    [SerializeField]
    private CardDeckController cardDeckController;
    [SerializeField]
    private GameObject imgGroup;

    private Tile curTile = null;
    private bool isActive = false;

    void Start()
    {
        imgGroup.SetActive(false);

        cardDeckController.curHandlingObject
            .Select(_ => _ != null && _.GetComponent<Tile>() != null)
            .Do(_ =>
            {
                if (_ == false)
                    imgGroup.SetActive(false);
            })
            .Subscribe(_ => isActive = _);

        cardDeckController.curHandlingObject
            .Where(_ => _ != null)
            .Select(_ => _.GetComponent<Tile>())
            .Subscribe(tile => curTile = tile);

        //InputManager.Instance._settingCard.DelayFrame(1).Subscribe(_ => imgGroup.SetActive(_ && NodeManager.Instance._GuideState == GuideState.Tile))
        //    .AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => isActive) // imgGroup이 활성 상태일 때만
            .Subscribe(_ =>
            {
                TileNode curNode = UtilHelper.RayCastTile();
                if (curNode != null && curNode.GuideActive)
                    transform.position = Camera.main.WorldToScreenPoint(curNode.transform.position);
                else
                    transform.position = new Vector3(0, 10000, 0);

                imgGroup.SetActive(curTile != null && curNode != null && curTile.GetAvailableCount(curNode) > 1);
            })
            .AddTo(this);
    }
}
