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


    private void WheelMove()
    {
        if (Input.GetKey(KeyCode.Mouse2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            Vector3 targetPos = guideObject.position + (new Vector3(-mouseX, 0, -mouseY) * mouseMult * SettingManager.Instance.MouseSensitivity);
            guideObject.position = targetPos;
        }
    }

    private void Update()
    {
        WheelMove();

        if (MouseWheelCheck())
            SetCam();
    }
}
