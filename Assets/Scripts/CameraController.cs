using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float wheelSpeed = 2f;
    public float minZoom = 2.5f;
    public float maxZoom = 8f;
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
            Vector3 targetPos = transform.position + new Vector3(-mouseX, 0, -mouseY);
            transform.position = targetPos; // ReturnMaxRangePos(targetPos);
        }
    }

    private void Update()
    {
        WheelMove();
        ZoomInOut();
    }
}
