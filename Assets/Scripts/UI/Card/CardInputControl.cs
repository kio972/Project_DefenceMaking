using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(CardFramework))]
public class CardInputControl : MonoBehaviour
{
    [SerializeField]
    private bool useDrag = true;
    [SerializeField]
    private bool useClick = true;

    [SerializeField]
    private bool useShortCutKey = false;

    private enum CardInputState
    {
        None,
        Ready,
        While,
    }

    private CardInputState _state;
    bool isStartStreamStarted = false;
    protected CompositeDisposable disposables = new CompositeDisposable();

    private bool ShorcutKeyInput(int index)
    {
        index = index + 49;
        if (index == 58)
            index = 48;
        else if (index > 58)
            return false;

        KeyCode targetKey = (KeyCode)(index);
        return Input.GetKeyDown(targetKey);
    }

    private bool ClickInput()
    {
        return Input.GetKeyUp(KeyCode.Mouse0);
    }

    private bool CancelInput()
    {
        return Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(SettingManager.Instance.key_Deploy._CurKey) || Input.GetKeyDown(SettingManager.Instance.key_Research._CurKey) || Input.GetKeyDown(SettingManager.Instance.key_Shop._CurKey) || Input.GetKeyDown(KeyCode.Escape) || GameManager.Instance.isPause;
    }

    public void StopAllInputCheck()
    {
        disposables.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        CardFramework cardFramework = GetComponent<CardFramework>();
        Image image = GetComponent<Image>();

        var startStream = image.OnPointerDownAsObservable()
            .Where(_ => !InputManager.Instance.settingCard && _state == CardInputState.None)
            .Subscribe(_ => _state = CardInputState.Ready)
            .AddTo(disposables);

        if(useShortCutKey)
        {
            var shortCutStream = Observable.EveryUpdate()
                .Where(_ => !InputManager.Instance.settingCard && _state == CardInputState.None)
                .Where(_ => ShorcutKeyInput(cardFramework.handIndex))
                .Where(_ => UIManager.Instance._OpendUICount == 0 && !GameManager.Instance.isPause)
                .DelayFrame(1)
                .Do(_ => _state = CardInputState.While)
                .Subscribe(_ => cardFramework.CallCard())
                .AddTo(disposables);
        }

        if(useClick)
        {
            var clickStream = image.OnPointerUpAsObservable()
            .Where(_ => _state == CardInputState.Ready)
            .DelayFrame(1)
            .Do(_ => _state = CardInputState.While)
            .Subscribe(_ => cardFramework.CallCard())
            .AddTo(disposables);
        }

        if(useDrag)
        {
            var dragStream = image.OnPointerExitAsObservable()
            .Where(_ => _state == CardInputState.Ready)
            .DelayFrame(1)
            .Do(_ => _state = CardInputState.While)
            .Subscribe(_ => cardFramework.CallCard())
            .AddTo(disposables);
        }

        var endStream = Observable.EveryUpdate()
            .Where(_ => _state == CardInputState.While)
            .Where(_ => ClickInput() || CancelInput())
            .Do(_ => cardFramework.EndCard(CancelInput()))
            .DelayFrame(1)
            .Subscribe(_ => _state = CardInputState.None);
    }
}
