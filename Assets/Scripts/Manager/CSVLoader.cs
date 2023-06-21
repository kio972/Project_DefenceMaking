using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVLoader
{
    public static List<Dictionary<string, object>> LoadCSV(TextAsset textFile)
    {
        string str = textFile.text;

        string[] lines = str.Split('\n');

        string[] heads = lines[0].Split(',');

        heads[heads.Length - 1] = heads[heads.Length - 1].Replace("\r", "");

        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

        for (int i = 1; i < lines.Length; i++)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string[] col = lines[i].Split(',');
            col[col.Length - 1] = col[col.Length - 1].Replace("\r", "");
            for (int j = 0; j < heads.Length; j++)
            {
                string value = col[j];

                dic.Add(heads[j], col[j]);
            }

            list.Add(dic);
        }

        return list;
    }
}
