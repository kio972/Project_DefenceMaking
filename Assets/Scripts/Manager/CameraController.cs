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
    private List<GameObject> virtualCameraGroup;
    [SerializeField]
    private List<float> mouseMultVals;

    [SerializeField]
    private Transform guideObject;
    public Vector3 _GuidePos { get => guideObject.position; }

    private float mouseMult = 1f;

    public bool useScreenMove = true;

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
        foreach (GameObject cam in virtualCameraGroup)
            cam.SetActive(false);

        virtualCameraGroup[cameraZoom_Level].SetActive(true);
        mouseMult = mouseMultVals[cameraZoom_Level];
    }

    private bool MouseWheelCheck()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta > 0f)
        {
            // 휠을 위로 스크롤할 때
            cameraZoom_Level = Mathf.Clamp(cameraZoom_Level + 1, 0, virtualCameraGroup.Count - 1);
            return true;
        }
        else if (scrollDelta < 0f)
        {
            // 휠을 아래로 스크롤할 때
            cameraZoom_Level = Mathf.Clamp(cameraZoom_Level - 1, 0, virtualCameraGroup.Count - 1);
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

    private bool KeyBoardMove()
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
            return false;
        Vector3 targetPos = guideObject.position + (new Vector3(mouseX, 0, mouseY) * 0.1f * mouseMult * SettingManager.Instance.mouseSensitivity);
        targetPos = ModifyMaxPosition(targetPos);
        guideObject.position = targetPos;

        return true;
    }

    float speed = 0.1f;
    float maxSpeed = 1f;

    private void ScreenMove()
    {
        if (!useScreenMove)
            return;

        Vector3 targetPos = guideObject.position;
        int isizeX = Screen.width;
        int isizeY = Screen.height;
        float mouseAccel = 1f;
        if (Input.mousePosition.x < 5)
            targetPos.x -= 0.1f * speed;
        else if (Input.mousePosition.x > isizeX - 5)
            targetPos.x += 0.1f * speed;

        if (Input.mousePosition.y < 5)
            targetPos.z -= 0.1f * speed;
        else if (Input.mousePosition.y > isizeY - 5)
            targetPos.z += 0.1f * speed;

        if (guideObject.position != targetPos)
        {
            speed += Time.deltaTime * mouseAccel;
            if (speed > maxSpeed)
                speed = maxSpeed;
            guideObject.position = ModifyMaxPosition(targetPos);
        }
        else
            speed = 0.1f;
    }

    public void CamMoveToPos(Vector3 position)
    {
        guideObject.position = position;
    }

    private bool applicationFocusState = true;

    private void OnApplicationFocus(bool focus)
    {
        applicationFocusState = focus;
    }

    private void CamMove()
    {
        if (!applicationFocusState)
            return;

        if (Input.GetKey(KeyCode.Mouse2))
            WeelMove();
        else
        {
            KeyBoardMove();
            ScreenMove();
        }
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
