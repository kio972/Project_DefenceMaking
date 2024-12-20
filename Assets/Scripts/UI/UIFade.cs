using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    private Image image;
    private float elapsed = 0;
    private float speed = 1;
    private static Color black = Color.black;
    private static Color blackAlpha = new Color(0, 0, 0, 0);
    private bool isUpdate = false;
    private Color start;
    private Color end;

    private static UIFade instance;

    public static UIFade Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIFade>();

                if (instance == null)
                {
                    UIFade uiFadePrefab = Resources.Load<UIFade>("Prefab/UI/UIFade");
                    UIFade obj = Instantiate(uiFadePrefab);
                    obj.name = typeof(UIFade).Name;
                    instance = obj;
                    instance.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeIn(float speed)
    {
        image.gameObject.SetActive(true);
        image.color = black;
        isUpdate = true;
        this.speed = speed;
        start = black;
        end = blackAlpha;
        elapsed = 0;
    }

    public void FadeOut(float speed)
    {
        image.gameObject.SetActive(true);
        image.color = blackAlpha;
        isUpdate = true;
        this.speed = speed;
        start = blackAlpha;
        end = black;
        elapsed = 0;
    }

    void Init()
    {
        image = GetComponentInChildren<Image>(true);
        FadeIn(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isUpdate == false)
            return;

        elapsed += Time.deltaTime / speed;
        elapsed = Mathf.Clamp01(elapsed);
        Color color = Color.Lerp(start, end, elapsed);
        image.color = color;
        if (elapsed >= 1.0f)
        {
            if (color.Equals(blackAlpha))
            {
                image.gameObject.SetActive(false);
            }

            isUpdate = false;
        }
    }
}
