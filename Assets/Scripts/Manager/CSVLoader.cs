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

        string[] lines = str.Split('\r');
        string[] heads = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
            lines[i] = lines[i].Remove(0, 1);

        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        StringBuilder sb = new StringBuilder();
        for (int i = 1; i < lines.Length; i++)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string[] col = lines[i].Split(',');
            col[col.Length - 1] = col[col.Length - 1].Replace("\r", "");
            int headIndex = 0;
            for (int j = 0; j < col.Length; j++)
            {
                sb.Clear();
                sb.Append(col[j]);
                bool isNeedCorrect = IsNeedCorrect(j, col, heads.Length);
                int loopTime = 0;
                if (isNeedCorrect)
                    loopTime = CorrectCells(sb, j, col, heads.Length, loopTime);

                string value = sb.ToString();
                value = value.Replace("\"\"", "<DQ>");
                value = value.Replace("\"", "");
                value = value.Replace("<DQ>", "\"");

                if(!dic.ContainsKey(heads[headIndex]))
                    dic.Add(heads[headIndex], value);

                if (isNeedCorrect)
                    j += loopTime;
                headIndex++;
            }

            list.Add(dic);
        }

        return list;
    }
}
