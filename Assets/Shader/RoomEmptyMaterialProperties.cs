using UnityEngine;
using UnityEditor;

[DisallowMultipleComponent]
public class RoomEmptyMaterialProperties : MonoBehaviour
{
    static int baseColorId = Shader.PropertyToID("_BaseColor");
    static int Root0Id = Shader.PropertyToID("_Root0");
    static int Root1Id = Shader.PropertyToID("_Root1");
    static int Root2Id = Shader.PropertyToID("_Root2");
    static int Root3Id = Shader.PropertyToID("_Root3");
    static int Root4Id = Shader.PropertyToID("_Root4");
    static int Root5Id = Shader.PropertyToID("_Root5");

    [SerializeField]
    public Color baseColor = Color.white;
    [SerializeField]
    bool Root0 = false;
    [SerializeField]
    bool Root1 = false;
    [SerializeField]
    bool Root2 = false;
    [SerializeField]
    bool Root3 = false;
    [SerializeField]
    bool Root4 = false;
    [SerializeField]
    bool Root5 = false;

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
        block.SetFloat(Root0Id, BoolToFloat(Root0));
        block.SetFloat(Root1Id, BoolToFloat(Root1));
        block.SetFloat(Root2Id, BoolToFloat(Root2));
        block.SetFloat(Root3Id, BoolToFloat(Root3));
        block.SetFloat(Root4Id, BoolToFloat(Root4));
        block.SetFloat(Root5Id, BoolToFloat(Root5));
        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorId, baseColor);
        block.SetFloat(Root0Id, BoolToFloat(Root0));
        block.SetFloat(Root1Id, BoolToFloat(Root1));
        block.SetFloat(Root2Id, BoolToFloat(Root2));
        block.SetFloat(Root3Id, BoolToFloat(Root3));
        block.SetFloat(Root4Id, BoolToFloat(Root4));
        block.SetFloat(Root5Id, BoolToFloat(Root5));
        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    float BoolToFloat(bool booleanValue) {
        if (booleanValue)
            return 1f;
        else return 0f;
    }

}