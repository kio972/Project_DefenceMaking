using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteList : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    private Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();

    private static SpriteList instance;

    public static SpriteList Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SpriteList>();
                if(instance == null)
                {
                    SpriteList temp = Resources.Load<SpriteList>("Data/SpriteList");
                    temp = Instantiate(temp);
                    temp.name = typeof(SpriteList).Name;
                    instance = temp;
                    instance.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
                }
            }

            return instance;
        }
    }

    public void Init()
    {
        spriteDic = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in sprites)
        {
            spriteDic.Add(sprite.name, sprite);
        }

    }

    public Sprite LoadSprite(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
            return null;

        if (spriteDic.ContainsKey(spriteName))
            return spriteDic[spriteName];

        return null;
    }
}
