using UnityEngine;
using UnityEditor;

[DisallowMultipleComponent]
public class RoadEmptyMaterialProperties : MonoBehaviour
{
    static int baseColorId = Shader.PropertyToID("_BaseColor");

    [SerializeField]
    public Color baseColor = Color.white;

    [Range(0, 1)]
    public float bumpScale = 1;

    static MaterialPropertyBlock block;

    void Awake()
    {
        OnValidate();
    }

    public void SetColor(Color color) //�Լ��� �����
    {
        this.baseColor = color * bumpScale;

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