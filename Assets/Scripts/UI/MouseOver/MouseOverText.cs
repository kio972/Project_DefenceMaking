using System.Collections;
using System.Collections.Generic;
using Spine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MouseOverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textMeshPro;
    private MouseOverTooltip tooltipPanel;

    [SerializeField]
    private bool isTextOnOverlayCanvas = true;
    [SerializeField]
    private bool isOnUI = true;
    private Camera mainCamera;
    private bool isPointerOverText = false;
    private int lastHoveredLinkIndex = -1;

    [SerializeField]
    private Vector2 targetPivot = new Vector2(0.5f, 0.5f);

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        // Canvas�� Overlay�� ��� mainCamera�� null�� �ص� ��
        mainCamera = isTextOnOverlayCanvas ? null : Camera.main;
        // RichText �� ��ũ ���� ���� ������Ʈ
        textMeshPro.ForceMeshUpdate();
        tooltipPanel = GameManager.Instance._InGameUI.mouseOverTooltip;
    }

    void Update()
    {
        if (!isPointerOverText) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, mainCamera);

        if (linkIndex != -1 && linkIndex != lastHoveredLinkIndex)
        {
            // ��ũ ID ��������
            string linkId = textMeshPro.textInfo.linkInfo[linkIndex].GetLinkID();

            var tooltipKey = DataManager.Instance.GetTooltipKey(linkId);
            if (!string.IsNullOrEmpty(tooltipKey.name))
            {
                var linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
                int targetIndex = linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength - 1;
                var charInfo = textMeshPro.textInfo.characterInfo[targetIndex];
                Vector3 pos = charInfo.bottomRight;
                string header = DataManager.Instance.GetDescription(tooltipKey.name);
                string desc = DataManager.Instance.GetDescription(tooltipKey.desc);
                tooltipPanel.SetActive(true);
                tooltipPanel.SetMesseage(transform.position + pos, targetPivot, isOnUI, header, desc);
            }

            lastHoveredLinkIndex = linkIndex;
        }
        else if (linkIndex == -1)
        {
            tooltipPanel.SetActive(false);
            lastHoveredLinkIndex = -1;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOverText = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOverText = false;
        tooltipPanel.SetActive(false);
        lastHoveredLinkIndex = -1;
    }

    private void OnDisable()
    {
        OnPointerExit(null);
    }
}
