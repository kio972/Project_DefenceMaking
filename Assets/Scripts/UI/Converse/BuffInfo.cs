using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public struct BuffTable
{
    public int ally_defense;
    public float ally_attackSpeed;
    public float ally_damageRate;
    public int ally_heal;
    public float ally_hpRate;
    public int ally_maxHp;
    public int gold;

    public float enemy_attackSpeed;
    public float enemy_damageRate;
    public string[] enemy_raise;

    public BuffTable(Dictionary<string, object> buffData)
    {
        int.TryParse(buffData["ally_defense"].ToString(), out ally_defense);
        float.TryParse(buffData["ally_attackSpeed"].ToString(), out ally_attackSpeed);
        float.TryParse(buffData["ally_damageRate"].ToString(), out ally_damageRate);
        int.TryParse(buffData["ally_heal"].ToString(), out ally_heal);
        float.TryParse(buffData["ally_hpRate"].ToString(), out ally_hpRate);
        int.TryParse(buffData["ally_maxHp"].ToString(), out ally_maxHp);
        int.TryParse(buffData["gold"].ToString(), out gold);

        float.TryParse(buffData["enemy_attackSpeed"].ToString(), out enemy_attackSpeed);
        float.TryParse(buffData["enemy_damageRate"].ToString(), out enemy_damageRate);
        enemy_raise = buffData["enemy_raise"].ToString().Split('&');
    }
}

public class BuffInfo : MonoBehaviour
{
    [SerializeField]
    GameObject buffZone;
    [SerializeField]
    GameObject debuffZone;

    [SerializeField]
    private List<TextMeshProUGUI> buffTexts;
    [SerializeField]
    private List<TextMeshProUGUI> debuffTexts;

    [SerializeField]
    private GameObject border;
    [SerializeField]
    private GameObject noChangeText;

    private void ResetTexts()
    {
        foreach (var text in buffTexts)
            text.gameObject.SetActive(false);
        foreach (var text in debuffTexts)
            text.gameObject.SetActive(false);
    }

    private string ConvertValue(float target, out bool isBuff, bool reverse = false)
    {
        string result = "+ ";
        isBuff = true;
        if (target < 0)
        {
            result = "- ";
            isBuff = false;
        }
        result = result + Mathf.Abs(target).ToString() + "%";

        if (reverse)
            isBuff = !isBuff;

        return result;
    }

    private string ConvertValue(int target, out bool isBuff, bool reverse = false)
    {
        string result = "+ ";
        isBuff = true;
        if (target < 0)
        {
            result = "- ";
            isBuff = false;
        }
        result = result + Mathf.Abs(target).ToString();

        if (reverse)
            isBuff = !isBuff;

        return result;
    }

    private void SetText(string value, bool isBuff)
    {
        TextMeshProUGUI targetText = null;
        List<TextMeshProUGUI> targetPool = isBuff ? buffTexts : debuffTexts;
        foreach(TextMeshProUGUI text in targetPool)
        {
            if (text.gameObject.activeSelf)
                continue;
            targetText = text;
            break;
        }

        if (targetText == null)
        {
            targetText = Instantiate(targetPool[0], targetPool[0].transform.parent);
            targetPool.Add(targetText);
        }

        targetText.gameObject.SetActive(true);
        targetText.text = value;
    }

    public void SetInfo(Dictionary<string, object> buffData)
    {
        ResetTexts();
        BuffTable table = new BuffTable(buffData);
        if (table.gold != 0)
        {
            bool isBuff;
            string newString = "소지금 " + ConvertValue(table.gold, out isBuff);
            SetText(newString, isBuff);
        }

        if (table.ally_defense != 0)
        {
            bool isBuff;
            string newString = "몬스터 방어력 " + ConvertValue(table.ally_defense, out isBuff);
            SetText(newString, isBuff);
        }

        if (table.ally_attackSpeed != 0)
        {
            bool isBuff;
            string newString = "몬스터 공격속도 " + ConvertValue(table.ally_attackSpeed, out isBuff);
            SetText(newString, isBuff);
        }

        if (table.ally_damageRate != 0)
        {
            bool isBuff;
            string newString = "몬스터 공격력 " + ConvertValue(table.ally_damageRate, out isBuff);
            SetText(newString, isBuff);
        }

        if (table.ally_heal != 0)
        {
            bool isBuff;
            string newString = "몬스터 체력 회복 " + ConvertValue(table.ally_heal, out isBuff);
            SetText(newString, isBuff);
        }

        if (table.ally_hpRate != 0)
        {
            bool isBuff;
            string newString = "몬스터 최대체력 " + ConvertValue(table.ally_hpRate, out isBuff);
            SetText(newString, isBuff);
        }

        if (table.ally_maxHp != 0)
        {
            bool isBuff;
            string newString = "몬스터 최대체력 " + ConvertValue(table.ally_maxHp, out isBuff);
            SetText(newString, isBuff);
        }

        if (table.enemy_attackSpeed != 0)
        {
            bool isBuff;
            string newString = "모험가 최대체력 " + ConvertValue(table.enemy_attackSpeed, out isBuff, true);
            SetText(newString, isBuff);
        }

        if (table.enemy_damageRate != 0)
        {
            bool isBuff;
            string newString = "모험가 공격력 " + ConvertValue(table.enemy_damageRate, out isBuff, true);
            SetText(newString, isBuff);
        }

        if (table.enemy_raise[0] != "")
        {
            foreach(string target in table.enemy_raise)
            {
                bool isBuff;
                string[] split = target.Split('+');
                string _name = DataManager.Instance.GetDescription(split[0]);
                int number = Convert.ToInt32(split[1]);
                string newString = "매 라운드마다 침입하는\n" + _name + " " + ConvertValue(number, out isBuff, true);
                SetText(newString, isBuff);
            }
        }

        bool isNoChange = !buffTexts[0].gameObject.activeSelf && !debuffTexts[0].gameObject.activeSelf;
        border.SetActive(!isNoChange);
        noChangeText.SetActive(isNoChange);
    }
}
