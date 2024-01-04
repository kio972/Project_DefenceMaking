using UnityEngine;
using UnityEditor;

[DisallowMultipleComponent]
public class RoadEmptyMaterialProperties : MonoBehaviour
{
    static int baseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    public Color baseColor = Color.white;


    static MaterialPropertyBlock block;

    void Awake()
    {
        OnValidate();
    }

    public void SetColor(Color color) //함수명 변경됨
    {
        this.baseColor = color;

        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorId, baseColor);

        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorId, baseColor);

        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    float BoolToFloat(bool booleanValue) {
        if (booleanValue)
            return 1f;
        else return 0f;
    }

}