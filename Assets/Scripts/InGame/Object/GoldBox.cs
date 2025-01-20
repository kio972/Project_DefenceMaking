using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IRewardObject
{
    void GetReward();
}

public class GoldBox : MonoBehaviour, IRewardObject
{
    [SerializeField]
    private int goldAmount = 100;

    [SerializeField]
    private UnityEvent buyEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (isFinish)
            return;

        Battler battle = other.GetComponent<Battler>();
        if (battle == null || battle.unitType == UnitType.Enemy) return;

        GetReward();
        isFinish = true;
    }

    private bool isFinish = false;

    private async UniTaskVoid ExcuteGetReward()
    {
        GameManager.Instance.gold += goldAmount;
        buyEvent.Invoke();
        //AudioManager.Instance.Play2DSound("UI_Shop_Buy", SettingManager.Instance._FxVolume);
        DamageTextPooling.Instance.TextEffect(transform.position - (Vector3.down * 0.2f), $"+{goldAmount}<sprite name=Gold>", 27f, Color.yellow, true);

        float elapsedTime = 0f;
        float lerpTime = 0.5f;
        Color startColor = Color.white;
        Color endColor = Color.clear;
        //Vector3 startPosition = transform.position;
        //Vector3 endPosition = startPosition + (Vector3.up * 0.5f);
        while(elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            renderer.color = Color.Lerp(startColor, endColor, elapsedTime / lerpTime);
            //transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / lerpTime);
            await UniTask.Yield();
        }

        Destroy(gameObject);
    }

    public void GetReward()
    {
        ExcuteGetReward().Forget();
    }
}
