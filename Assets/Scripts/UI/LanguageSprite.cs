using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Image))]
public class LanguageSprite : MonoBehaviour, ILanguageChange
{
    [SerializeField]
    private List<Sprite> sprites;

    private Image _image;

    public Image image
    {
        get
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            return _image;
        }
    }

    private void Awake()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        SettingManager.Instance._language.Subscribe(_ =>
        {
            ChangeLangauge(_);
        });

        ChangeLangauge(SettingManager.Instance.language);
    }

    public void ChangeLangauge(Languages language, string key = null)
    {
        int index = (int)language;
        if (index < 0 || index >= sprites.Count)
            index = 1;

        image.sprite = sprites[index];
    }
}
