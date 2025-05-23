using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DamageTextPooling : IngameSingleton<DamageTextPooling>
{
    private List<DamageText> damageTexts = new List<DamageText>();
    private StringBuilder sb = new StringBuilder();

    private DamageText InstantiateDamageText()
    {
        DamageText temp = Resources.Load<DamageText>("Prefab/UI/DamageText");
        return Instantiate(temp, transform);
    }

    public void TextEffect(Vector3 pos, int value, float fontSize, Color color, bool isBold, bool isHeal = false, float randomOffset = 0.25f)
    {
        sb.Clear();
        sb.Append(isHeal ? '+' : '-');
        sb.Append(value);

        TextEffect(pos, sb.ToString(), fontSize, color, isBold, randomOffset);
    }

    public void TextEffect(Vector3 pos, string text, float fontSize, Color color, bool isBold, float randomOffset = 0)
    {
        DamageText target = null;
        foreach (DamageText damageText in damageTexts)
        {
            if (!damageText.gameObject.activeSelf)
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

        target.StartEffect(text, fontSize, color, isBold);
        RectTransform rect = target.transform.GetComponent<RectTransform>();
        Vector3 offset = new Vector3(Random.Range(-randomOffset, randomOffset), Random.Range(-randomOffset, randomOffset), 0);
        rect.position = pos + Vector3.up + offset;
    }
}
