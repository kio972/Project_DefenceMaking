using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float wheelSpeed = 2f;
    public float minZoom = 2.5f;
    public float maxZoom = 8f;
    [SerializeField]
    private int cameraZoom_Level = 1;
    public int Camera_Level { get => cameraZoom_Level; }

    [SerializeField]
    private GameObject cam_Index0;
    [SerializeField]
    private GameObject cam_Index1;
    [SerializeField]
    private GameObject cam_Index2;
    [SerializeField]
    private GameObject cam_Index3;

    [SerializeField]
    private Transform guideObject;

    private float mouseMult = 1f;

    public void ResetCamPos(bool isStartPoint = false)
    {
        Vector3 position = NodeManager.Instance.endPoint.transform.position;
        if (isStartPoint)
            position = NodeManager.Instance.startPoint.transform.position;
        guideObject.transform.position = position;
    }

    public void SetCamZoom(int level)
    {
        cameraZoom_Level = level;
        SetCam();
    }

    private void SetCam()
    {
        cam_Index0.gameObject.SetActive(false);
        cam_Index1.gameObject.SetActive(false);
        cam_Index2.gameObject.SetActive(false);
        cam_Index3.gameObject.SetActive(false);

        switch (cameraZoom_Level)
        {
            case 0:
                cam_Index0.gameObject.SetActive(true);
                mouseMult = 1.2f;
                break;
            case 1:
                cam_Index1.gameObject.SetActive(true);
                mouseMult = 1f;
                break;
            case 2:
                cam_Index2.gameObject.SetActive(true);
                mouseMult = 0.8f;
                break;
            case 3:
                cam_Index3.gameObject.SetActive(true);
                mouseMult = 0.6f;
                break;
        }
    }

    private bool MouseWheelCheck()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta > 0f)
        {
            // 휠을 위로 스크롤할 때
            cameraZoom_Level = Mathf.Clamp(cameraZoom_Level + 1, 0, 3);
            return true;
        }
        else if (scrollDelta < 0f)
        {
            // 휠을 아래로 스크롤할 때
            cameraZoom_Level = Mathf.Clamp(cameraZoom_Level - 1, 0, 3);
            return true;
        }

        return false;
    }

    private void ZoomInOut()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * wheelSpeed;
        Vector3 targetPos = transform.position + new Vector3(0, -scroll, 0);
        targetPos.y = Mathf.Clamp(targetPos.y, minZoom, maxZoom);
        transform.position = targetPos;
    }

    private Vector3 ModifyMaxPosition(Vector3 position)
    {
        if(NodeManager.Instance == null)
            return position;

        // 노드 중 row col값
        position.x = Mathf.Clamp(position.x, NodeManager.Instance.MinCol, NodeManager.Instance.MaxCol);
        position.z = Mathf.Clamp(position.z, NodeManager.Instance.MinRow, NodeManager.Instance.MaxRow);

        return position;
    }

    private void WeelMove()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector3 targetPos = guideObject.position + (new Vector3(-mouseX, 0, -mouseY) * mouseMult * SettingManager.Instance.mouseSensitivity);
        targetPos = ModifyMaxPosition(targetPos);
        guideObject.position = targetPos;
    }

    private void KeyBoardMove()
    {
        float mouseX = 0;
        float mouseY = 0;

        if (Input.GetKey(SettingManager.Instance.key_Camera_MoveLeft._CurKey))
            mouseX = -1f;
        else if (Input.GetKey(SettingManager.Instance.key_Camera_MoveRight._CurKey))
            mouseX = 1f;

        if (Input.GetKey(SettingManager.Instance.key_Camera_MoveDown._CurKey))
            mouseY = -1f;
        else if (Input.GetKey(SettingManager.Instance.key_Camera_MoveUp._CurKey))
            mouseY = 1f;

        if (mouseX == 0 && mouseY == 0)
            return;
        Vector3 targetPos = guideObject.position + (new Vector3(mouseX, 0, mouseY) * 0.1f * mouseMult * SettingManager.Instance.mouseSensitivity);
        targetPos = ModifyMaxPosition(targetPos);
        guideObject.position = targetPos;
    }

    public void CamMoveToPos(Vector3 position)
    {
        guideObject.position = position;
    }

    private void CamMove()
    {
        if (Input.GetKey(KeyCode.Mouse2))
            WeelMove();
        else
            KeyBoardMove();
    }

    private void Update()
    {
        if (GameManager.Instance.isPause)
            return;

        CamMove();

        if (MouseWheelCheck())
            SetCam();
    }
}
