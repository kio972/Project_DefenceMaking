using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationListener : MonoBehaviour
{
    private Battler battler;

    [SerializeField]
    private List<AudioClip> attackSounds = new List<AudioClip>();
    [SerializeField]
    private float attackSoundMult = 1f;
    [Space]
    [SerializeField]
    private List<AudioClip> deadSounds = new List<AudioClip>();
    [SerializeField]
    private float deadSoundMult = 1f;

    void Dead()
    {
        if (deadSounds.Count > 0)
            AudioManager.Instance.Play3DSound(deadSounds[Random.Range(0, deadSounds.Count)], transform.position, SettingManager.Instance._FxVolume * deadSoundMult);
    }

    void Attack()
    {
        battler.Attack();
        if(attackSounds.Count > 0)
            AudioManager.Instance.Play3DSound(attackSounds[Random.Range(0, attackSounds.Count)], transform.position, SettingManager.Instance._FxVolume * attackSoundMult);
    }

    private void Awake()
    {
        if (battler == null)
            battler = GetComponentInParent<Battler>();
    }
}
