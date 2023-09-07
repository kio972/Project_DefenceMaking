using UnityEngine;
using UnityEditor;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    static int baseColorId = Shader.PropertyToID("_HologramColor");
    static int effectId = Shader.PropertyToID("_Effect");

    [SerializeField]
    public Color baseColor = Color.white;
    [SerializeField]
    bool effect = false;

    static MaterialPropertyBlock block;

    void Awake()
    {
        OnValidate();
    }

    public void SetColor(Color color)
    {
        this.baseColor = color;

        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorId, baseColor);
        block.SetFloat(effectId, BoolToFloat(effect));
        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorId, baseColor);
        block.SetFloat(effectId, BoolToFloat(effect));
        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    float BoolToFloat(bool booleanValue) {
        if (booleanValue)
            return 1f;
        else return 0f;
    }

}