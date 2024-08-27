using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRewardObject
{
    void GetReward();
}

public class GoldBox : MonoBehaviour, IRewardObject
{
    private async UniTaskVoid ExcuteGetReward()
    {
        GameManager.Instance.gold += 100;
        AudioManager.Instance.Play2DSound("UI_Shop_Buy", SettingManager.Instance._FxVolume);

        float elapsedTime = 0f;
        float lerpTime = 0.5f;
        Color startColor = Color.white;
        Color endColor = Color.clear;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + (Vector3.up * 0.5f);
        while(elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            renderer.color = Color.Lerp(startColor, endColor, elapsedTime / lerpTime);
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / lerpTime);
            await UniTask.Yield();
        }

        Destroy(gameObject);
    }

    public void GetReward()
    {
        ExcuteGetReward().Forget();
    }
}
