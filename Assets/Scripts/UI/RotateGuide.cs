using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.XR;

public class RotateGuide : MonoBehaviour
{
    [SerializeField]
    private GameObject imgGroup;

    void Start()
    {
        imgGroup.SetActive(false);
        
        InputManager.Instance._settingCard.DelayFrame(1).Subscribe(_ => imgGroup.SetActive(_ && NodeManager.Instance._GuideState == GuideState.Tile))
            .AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => imgGroup.activeSelf) // imgGroup�� Ȱ�� ������ ����
            .Subscribe(_ =>
            {
                TileNode curNode = UtilHelper.RayCastTile();
                if (curNode != null && curNode.GuideActive)
                    transform.position = Camera.main.WorldToScreenPoint(curNode.transform.position);
                else
                    transform.position = new Vector3(0, 10000, 0);
            })
            .AddTo(this);
    }
}
