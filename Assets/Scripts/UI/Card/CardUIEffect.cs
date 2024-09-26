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

    public bool useSiblingArrange = true;
    public int originSiblingIndex = -1;

    private CompositeDisposable disposables = new CompositeDisposable();

    private CancellationTokenSource _scaleToken;
    private CancellationTokenSource _moveToken;

    public void ResetEffect()
    {
        effectTarget.localScale = Vector3.one;
        if (useSiblingArrange)
            effectTarget.position = originPos;
    }

    public void SetDrawState(bool value)
    {
        drawEnd = value;
    }

    public void OnPointerEnter()
    {
        if (GameManager.Instance.cardLock)
            return;

        if (!drawEnd)
            return;

        _moveToken.Cancel();
        _scaleToken.Cancel();
        _moveToken = new CancellationTokenSource();
        _scaleToken = new CancellationTokenSource();

        UtilHelper.IScaleEffect(effectTarget, effectTarget.localScale, Vector3.one * 1.5f, mouseOverTime, _scaleToken.Token).Forget();
        if(useSiblingArrange)
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
        _moveToken.Dispose();
        _scaleToken.Dispose();
        _moveToken = new CancellationTokenSource();
        _scaleToken = new CancellationTokenSource();

        UtilHelper.IScaleEffect(effectTarget, effectTarget.localScale, Vector3.one, mouseOverTime, _scaleToken.Token).Forget();
        if(useSiblingArrange)
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
        _moveToken.Dispose();
        _scaleToken.Dispose();
        _moveToken = new CancellationTokenSource();
        _scaleToken = new CancellationTokenSource();

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
        _scaleToken.Dispose();
        _scaleToken = new CancellationTokenSource();

        float lerpTime = 0.5f;
        await UtilHelper.IScaleEffect(transform, Vector3.zero, Vector3.one, lerpTime, _scaleToken.Token);

        drawEnd = true;
    }

    private void OnEnable()
    {
        _moveToken = new CancellationTokenSource();
        _scaleToken = new CancellationTokenSource();
    }

    private void Start()
    {
        Image image = GetComponent<Image>();
        if(image != null)
        {
            image.OnPointerEnterAsObservable().Where(_ => !GameManager.Instance.cardLock && drawEnd && !InputManager.Instance.settingCard).Subscribe(_ => OnPointerEnter());
            image.OnPointerExitAsObservable().Where(_ => drawEnd).Subscribe(_ => OnPointerExit());
        }
    }
}
