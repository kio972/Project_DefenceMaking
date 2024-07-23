using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CardUIEffect : MonoBehaviour
{
    [SerializeField]
    Transform effectTarget;

    [SerializeField]
    private float mouseOverTime = 0.2f;

    private bool drawEnd = false;

    public Vector3 originPos;
    public Quaternion originRot;

    public int originSiblingIndex = -1;

    private CompositeDisposable disposables = new CompositeDisposable();

    private CancellationTokenSource _scaleToken = new();
    private CancellationTokenSource _moveToken = new();

    public void OnPointerEnter()
    {
        if (GameManager.Instance.cardLock)
            return;

        if (!drawEnd)
            return;

        _moveToken.Cancel();
        _scaleToken.Cancel();
        _moveToken = new();
        _scaleToken = new();

        UtilHelper.IScaleEffect(effectTarget, effectTarget.localScale, Vector3.one * 1.5f, mouseOverTime, _scaleToken.Token).Forget();
        UtilHelper.IMoveEffect(effectTarget, effectTarget.position, new Vector3(originPos.x, 180, originPos.z), mouseOverTime, _moveToken.Token).Forget();

        effectTarget.rotation = Quaternion.identity;
        transform.SetAsLastSibling();

        AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);
    }

    public void OnPointerExit()
    {
        if (!drawEnd)
            return;

        _moveToken.Cancel();
        _scaleToken.Cancel();
        _moveToken = new();
        _scaleToken = new();

        UtilHelper.IScaleEffect(effectTarget, effectTarget.localScale, Vector3.one, mouseOverTime, _scaleToken.Token).Forget();
        UtilHelper.IMoveEffect(effectTarget, effectTarget.position, originPos, mouseOverTime, _moveToken.Token).Forget();

        effectTarget.rotation = originRot;
        if (originSiblingIndex != -1)
            transform.SetSiblingIndex(originSiblingIndex);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        _moveToken.Dispose();
        _scaleToken.Dispose();
    }

    private void OnDisable()
    {
        disposables.Dispose();
        _moveToken.Dispose();
        _scaleToken.Dispose();
    }

    public async UniTaskVoid DiscardEffect(bool isRecycle)
    {
        drawEnd = false;
        _moveToken.Cancel();
        _scaleToken.Cancel();
        _moveToken = new();
        _scaleToken = new();

        float lerpTime = 0.4f;
        UtilHelper.IColorEffect(transform, Color.white, new Color(1, 1, 1, 0), lerpTime).Forget();
        if (isRecycle)
        {
            UtilHelper.IMoveEffect(transform, originPos, GameManager.Instance.cardDeckController.transform.position, lerpTime, _moveToken.Token).Forget();
            await UtilHelper.IScaleEffect(transform, Vector3.one, Vector3.zero, lerpTime, _scaleToken.Token);
        }
        else
            await StartCoroutine(UtilHelper.IMoveEffect(transform, originPos, originPos + new Vector3(0, 150, 0), lerpTime));

        await UniTask.Yield();
        Destroy(gameObject);
    }

    public async UniTaskVoid DrawEffect()
    {
        drawEnd = false;

        _scaleToken.Cancel();
        _scaleToken = new();

        float lerpTime = 0.5f;
        await UtilHelper.IScaleEffect(transform, Vector3.zero, Vector3.one, lerpTime, _scaleToken.Token);

        drawEnd = true;
    }

    private void Start()
    {
        Image image = GetComponent<Image>();
        if(image != null)
        {
            image.OnPointerEnterAsObservable().Where(_ => !GameManager.Instance.cardLock && drawEnd).Subscribe(_ => OnPointerEnter());
            image.OnPointerExitAsObservable().Where(_ => drawEnd).Subscribe(_ => OnPointerExit());
        }
    }
}
