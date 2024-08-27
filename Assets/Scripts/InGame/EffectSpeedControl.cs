using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class EffectSpeedControl : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    [SerializeField]
    private float multRate = 1f;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        var mainModule = _particleSystem.main;
        GameManager.Instance._timeScale.Subscribe(_ => mainModule.simulationSpeed = _ * multRate).AddTo(gameObject);
    }
}