using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVLoader
{
    private static bool IsNeedCorrect(int index, string[] col, int indexSize)
    {
        if (col.Length == indexSize)
            return false;

        if (index == col.Length - 1)
            return false;

        string val1 = col[index];
        if (val1.Length == 0)
            return false;
        if (val1[0] != '\"')
            return false;
        if (val1[val1.Length - 1] == '\"')
        {
            if (val1.Length > 2 && val1[val1.Length - 2] == '\"')
            {
                if (val1[val1.Length - 3] == '\"')
                    return false;
                else
                    return true;
            }
            return false;
        }

        return true;
    }

    private static bool IsNeedCorrectContinuous(int nextIndex, string[] col)
    {
        if (nextIndex >= col.Length)
            return false;

        if (col[nextIndex].Length == 0)
            return true;

        if (col[nextIndex][col[nextIndex].Length - 1] == '\"')
            return false;
        else
            return true;
    }

    private static int CorrectCells(StringBuilder sb, int index, string[] col, int indexSize, int loopTime)
    {
        sb.Append("," + col[index + 1]);
        if (IsNeedCorrectContinuous(index + 1, col))
            loopTime = CorrectCells(sb, index + 1, col, indexSize, loopTime);

        return loopTime + 1;
    }

    public static List<Dictionary<string, object>> LoadCSV(TextAsset textFile)
    {
        string str = textFile.text;

        str = str.Replace("\r\n", "\n");
        var fields = new List<string>();
        bool inQuotes = false;
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];

            if (c == '"')
            {
                // 다음 문자가 따옴표일 경우(즉, 이중 따옴표), 그것은 따옴표 자체를 의미
                if (inQuotes && i + 1 < str.Length && str[i + 1] == '"')
                {
                    sb.Append('"');
                    i++; // 따옴표를 스킵
                }
                else
                    inQuotes = !inQuotes; // 따옴표 상태를 반전
            }
            else if (c == ',' && !inQuotes)
            {
                // 쉼표를 만났고 따옴표 안이 아닐 경우 필드가 끝났음을 의미
                fields.Add(sb.ToString());
                sb.Clear();
            }
            else if (c == '\n' && !inQuotes)
            {
                // 엔터를 만났고 따옴표 안이 아닐 경우 필드가 끝났음을 의미
                fields.Add(sb.ToString());
                sb.Clear();
            }
            else
                sb.Append(c); // 문자를 필드에 추가
        }

        fields.Add(sb.ToString()); // 마지막 필드를 추가

        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        int headCount = str.Split('\n')[0].Split(',').Length;
        int curCount = 0;
        List<string> heads = new List<string>();
        Dictionary<string, object> dic = new Dictionary<string, object>();
        foreach (var field in fields)
        {
            if (heads.Count < headCount)
                heads.Add(field);
            else
                dic.Add(heads[curCount], field);

            curCount++;
            if (curCount == headCount)
            {
                curCount = 0;
                list.Add(dic);
                dic = new Dictionary<string, object>();
            }
        }

        list.RemoveAt(0);
        return list;
    }
}
