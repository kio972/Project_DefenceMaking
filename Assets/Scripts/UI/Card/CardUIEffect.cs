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

    [SerializeField]
    private bool drawEnd = false;

    public Vector3 originPos;
    public Quaternion originRot;

    public bool useSiblingArrange = true;
    public int originSiblingIndex = -1;

    private CompositeDisposable disposables = new CompositeDisposable();

    private CancellationTokenSource _scaleToken;
    private CancellationTokenSource _moveToken;

    [SerializeField]
    private float sizeMult = 1.5f;

    [SerializeField]
    private AK.Wwise.Event mouseOverSound;

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

        UtilHelper.IScaleEffect(effectTarget, effectTarget.localScale, Vector3.one * sizeMult, mouseOverTime, _scaleToken.Token).Forget();
        if(useSiblingArrange)
            UtilHelper.IMoveEffect(effectTarget, effectTarget.position, new Vector3(originPos.x, 180, originPos.z), mouseOverTime, _moveToken.Token).Forget();

        effectTarget.rotation = Quaternion.identity;
        if(useSiblingArrange)
            transform.SetAsLastSibling();

        //AudioManager.Instance.Play2DSound("Click_card", SettingManager.Instance._FxVolume);
        mouseOverSound?.Post(gameObject);
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
        if (useSiblingArrange && originSiblingIndex != -1)
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

    public async UniTaskVoid OnMagicDrag()
    {
        await UniTask.Yield();
        //_moveToken.Cancel();
        _scaleToken.Cancel();
        //_moveToken.Dispose();
        _scaleToken.Dispose();
        //_moveToken = new CancellationTokenSource();
        _scaleToken = new CancellationTokenSource();
        
        UtilHelper.IScaleEffect(effectTarget, effectTarget.localScale, Vector3.one, mouseOverTime, _scaleToken.Token).Forget();
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
        UtilHelper.IColorEffect(effectTarget, Color.white, new Color(1, 1, 1, 0), lerpTime).Forget();
        if (isRecycle)
        {
            UtilHelper.IMoveEffect(transform, transform.position, GameManager.Instance.cardDeckController.transform.position, lerpTime, _moveToken.Token).Forget();
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

    public async UniTaskVoid MagicDrag()
    {
        //Vector3 originPos = transform.position;
        drawEnd = false;
        _moveToken.Cancel();
        _scaleToken.Cancel();
        _moveToken.Dispose();
        _scaleToken.Dispose();
        _moveToken = new CancellationTokenSource();
        _scaleToken = new CancellationTokenSource();

        await UniTask.WaitUntil(() => !Input.GetKeyDown(SettingManager.Instance.key_BasicControl._CurKey), cancellationToken: _moveToken.Token);
        while(true)
        {
            bool cancelInput = Input.GetKeyDown(SettingManager.Instance.key_BasicControl._CurKey) || Input.GetKeyDown(SettingManager.Instance.key_CancelControl._CurKey) || Input.GetKeyDown(SettingManager.Instance.key_Deploy._CurKey) || Input.GetKeyDown(SettingManager.Instance.key_Research._CurKey) || Input.GetKeyDown(SettingManager.Instance.key_Shop._CurKey) || Input.GetKeyDown(KeyCode.Escape) || GameManager.Instance.isPause;
            if(cancelInput)
                break;
            transform.position = Input.mousePosition;
            transform.localScale = Vector3.one;
            effectTarget.rotation = Quaternion.identity;
            effectTarget.position = transform.position;

            await UniTask.Yield(cancellationToken: _moveToken.Token);
        }

        transform.position = originPos;
        drawEnd = true;
        OnPointerExit();
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

            //CardFramework cardFramework = GetComponent<CardFramework>();
            //if(cardFramework != null && cardFramework._cardInfo.Value.cardType == CardType.Magic)
            //{
            //    var startStream = image.OnPointerDownAsObservable()
            //        .Subscribe(_ => MagicDrag().Forget()).AddTo(disposables);
            //}
        }
    }
}
