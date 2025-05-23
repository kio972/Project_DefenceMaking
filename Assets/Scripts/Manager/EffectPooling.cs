using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPooling : Singleton<EffectPooling>
{
    private List<GameObject> fxEffects = new List<GameObject>();

    public void StopAllEffect()
    {
        foreach(GameObject fxEffect in fxEffects)
        {
            ParticleSystem particleSystem = fxEffect.GetComponentInChildren<ParticleSystem>(true);
            particleSystem.Stop();
            fxEffect.gameObject.SetActive(false);
        }
    }

    public void PlayEffect(string effectName, Transform pos, Vector3 modifyPos = new Vector3(), float modifyScale = 1f, Vector3 target = new Vector3())
    {
        string particleAddress = "Prefab/Effect/" + effectName;
        GameObject fxEffect = Resources.Load<GameObject>(particleAddress);
        if (fxEffect == null)
            return;
        PlayEffect(fxEffect, pos, modifyPos, modifyScale, target);
    }

    public void PlayEffect(GameObject particle, Transform pos, Vector3 modifyPos = new Vector3(), float modifyScale = 1f, Vector3 target = new Vector3())
    {
        if (particle == null)
            return;

        string particleName = particle.name;
        foreach (GameObject fxEffect in fxEffects)
        {
            if (fxEffect.name.Contains(particleName))
            {
                ParticleSystem particleSystem = fxEffect.GetComponentInChildren<ParticleSystem>(true);
                if (!particleSystem.isPlaying)
                {
                    fxEffect.gameObject.SetActive(true);
                    Vector3 targetPos = pos.position;
                    if (modifyPos != new Vector3())
                    {
                        Vector3 modifiedOffset = pos.rotation * modifyPos;
                        targetPos += modifiedOffset;
                    }
                    fxEffect.transform.position = targetPos;
                    fxEffect.transform.rotation = pos.rotation;
                    if (target != new Vector3())
                        fxEffect.transform.LookAt(target + modifyPos);
                    fxEffect.transform.localScale = new Vector3(modifyScale, modifyScale, modifyScale);
                    particleSystem.Play();
                    return;
                }
            }
        }

        InstanceParticle(particle, pos, modifyPos, modifyScale, target);
    }

    private void InstanceParticle(GameObject particle, Transform pos, Vector3 modifyPos = new Vector3(), float modifyScale = 1f, Vector3 target = new Vector3())
    {
        GameObject effect = Instantiate(particle, transform);
        fxEffects.Add(effect);
        Vector3 targetPos = pos.position;
        if (modifyPos != new Vector3())
        {
            Vector3 modifiedOffset = pos.rotation * modifyPos;
            targetPos += modifiedOffset;
        }
        effect.transform.position = targetPos;
        effect.transform.rotation = pos.rotation;
        if (target != new Vector3())
            effect.transform.LookAt(target + modifyPos);
        effect.transform.localScale = new Vector3(modifyScale, modifyScale, modifyScale);
        ParticleSystem particleSystem = effect.GetComponentInChildren<ParticleSystem>();
        particleSystem.Play();
    }
}