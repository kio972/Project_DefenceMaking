using UnityEngine;
using UnityEditor;

public class CustomShaderGUI : ShaderGUI
{
    Material target;
    MaterialEditor editor;
    MaterialProperty[] properties;


    MaterialProperty FindProperty(string name) //함수생성
    {
        return FindProperty(name, properties);
    }

    void SetKeyword(string keyword, bool state)
    {
        if (state)
        {
            target.EnableKeyword(keyword);
        }
        else
        {
            target.DisableKeyword(keyword);
        }
    }

    bool FloatToBool(float floatValue)
    {
        if (floatValue != 0)
            return true;
        else return false;
    }

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.target = editor.target as Material;
        this.editor = editor;
        this.properties = properties;
        DoMain();

    }

    void DoMain()
    {

        MaterialProperty hologramColor = FindProperty("_HologramColor", properties);
        editor.ColorProperty(hologramColor, "Hologram Color");
        MaterialProperty map = FindProperty("_Effect");
        editor.FloatProperty(map, "Effect");

        SetKeyword("_EFFECT", FloatToBool(map.floatValue));
    }


}
