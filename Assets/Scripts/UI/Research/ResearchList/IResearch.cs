using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ResearchData
{
    public int researchIndex;
    public string researchName;
    public string researchDesc;
    public float requiredTime;
    public int requiredMoney;
    public int requiredherb1;
    public int requiredherb2;
    public int requiredherb3;

    public ResearchData(string id)
    {
        researchIndex = UtilHelper.Find_Data_Index(id, DataManager.Instance.research_Table, "id");
        Dictionary<string, object> data = DataManager.Instance.research_Table[researchIndex];

        researchName = data["Name_Key"].ToString();
        researchDesc = data["Name_Desc"].ToString();
        float.TryParse(data["RequiredTime"].ToString(), out requiredTime);
        requiredMoney = Convert.ToInt32(data["RequiredMoney"]);
        requiredherb1 = Convert.ToInt32(data["RequiredHerb1"]);
        requiredherb2 = Convert.ToInt32(data["RequiredHerb2"]);
        requiredherb3 = Convert.ToInt32(data["RequiredHerb3"]);
    }
}

public interface IResearch
{
    public void ActiveResearch();
}
