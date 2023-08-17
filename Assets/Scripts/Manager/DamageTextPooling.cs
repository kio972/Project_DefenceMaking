using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextPooling : IngameSingleton<DamageTextPooling>
{
    public static DamageTextPooling instance;
    public List<DamageText> damageTexts = new List<DamageText>();

    private DamageText InstantiateDamageText()
    {
        DamageText temp = Resources.Load<DamageText>("Prefab/UI/DamageText");
        return Instantiate(temp, transform);
    }

    public void TextEffect(Vector3 pos, int value, bool isHeal = false)
    {
        DamageText target = null;
        foreach(DamageText damageText in damageTexts)
        {
            if(!damageText.gameObject.activeSelf)
            {
                target = damageText;
                break;
            }
        }

        if (target == null)
        {
            target = InstantiateDamageText();
            damageTexts.Add(target);
        }

        target.StartEffect(value, isHeal);
        RectTransform rect = target.transform.GetComponent<RectTransform>();
        rect.position = pos + Vector3.up;
    }
}
