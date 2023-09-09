using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlKey
{
    None,
    CameraMoveUp,
    CameraMoveDown,
    CameraMoveLeft,
    CameraMoveRight,
    CameraReset,
    CameraResetAssist,

    SpeedControl0,
    SpeedControl1,
    SpeedControl2,

    ControlBasic,
    ControlCancel,
    RotateTile,
}

public class InputManager : IngameSingleton<InputManager>
{
    #region InputKeys
    public KeyCode[] nextMessageKeys = { KeyCode.Mouse0 };
    public KeyCode key_Camera_MoveUp = KeyCode.W;
    public KeyCode key_Camera_MoveDown = KeyCode.S;
    public KeyCode key_Camera_MoveLeft = KeyCode.A;
    public KeyCode key_Camera_MoveRight = KeyCode.D;
    public KeyCode key_Camera_Reset = KeyCode.KeypadEnter;
    public KeyCode key_Camera_ResetAssist = KeyCode.LeftShift;

    public KeyCode key_SpeedControl_Zero = KeyCode.Space;
    public KeyCode key_SpeedControl_One = KeyCode.Alpha1;
    public KeyCode key_SpeedControl_Double = KeyCode.Alpha2;

    public KeyCode key_BasicControl = KeyCode.Mouse0;
    public KeyCode key_CancelControl = KeyCode.Mouse1;

    public KeyCode key_RotateTile = KeyCode.R;

    private Dictionary<ControlKey, KeyCode> controlKeys = new Dictionary<ControlKey, KeyCode>();

    private List<KeyCode> invalidKeys = new List<KeyCode>()
    {
        KeyCode.Escape,
    };

    public bool IsValidKey(KeyCode value)
    {
        foreach (KeyCode key in invalidKeys)
        {
            if (key == value)
                return false;
        }

        return true;
    }

    public KeyCode GetValue(ControlKey key)
    {
        if (controlKeys.ContainsKey(key))
            return controlKeys[key];
        else
            return KeyCode.None;
    }

    public ControlKey GetKey(KeyCode value)
    {
        foreach (var kvp in controlKeys)
        {
            if (kvp.Value == value)
            {
                return kvp.Key;
            }
        }
        // 해당 value를 가진 ControlKey 키가 없는 경우 기본값인 ControlKey.None 반환
        return ControlKey.None;
    }

    public void SetKey(ControlKey targetKey, KeyCode value)
    {
        if (controlKeys.ContainsKey(targetKey))
        {
            controlKeys[targetKey] = value;
        }
        else
        {
            controlKeys.Add(targetKey, value);
        }
    }
    #endregion

    public TileNode testTile;

    public bool settingCard = false;


    private GameObject endPointPrefab = null;
    public TileNode curMoveObject = null;

    private void ReadySetTile(TileNode curTile)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 충돌된 오브젝트 처리
                GameObject hitObject = hit.collider.gameObject;
                //Debug.Log("Hit object: " + hitObject.name);
                print(hitObject.name);
            }

        }

    }

    void Init()
    {
        if (endPointPrefab == null)
            endPointPrefab = Resources.Load<GameObject>("Prefab/Tile/EndTile");
    }


    public void Call()
    {

    }

    private void InputCheck()
    {

    }

    private Tile curTile = null;

    private void TileMoveCheck()
    {
        if (curTile != null)
        {
            TileNode node = curTile.TileMoveCheck();
            if (Input.GetKeyDown(key_RotateTile))
                curTile.twin.RotateTile();
            if (!curTile.Movable || Input.GetKeyUp(key_CancelControl))
            {
                curTile.EndMoveing();
                curTile = null;
            }
            else if (Input.GetKeyUp(key_BasicControl))
            {
                curTile.EndMove(node);
                curTile = null;
            }
        }
        else if (Input.GetKey(key_BasicControl))
        {
            TileNode node = UtilHelper.RayCastTile();
            if (node != null && node.curTile != null && node.curTile.Movable)
            {
                node.curTile.ReadyForMove();
                curTile = node.curTile;
            }
        }
    }

    private void CameraResetCheck()
    {
        if (Input.GetKeyDown(key_Camera_Reset))
        {
            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController != null)
                cameraController.ResetCamPos(Input.GetKey(key_Camera_ResetAssist));
        }
    }

    private void SpeedControlCheck()
    {
        if(Input.GetKeyDown(key_SpeedControl_Zero))
        {
            if (GameManager.Instance.timeScale == 0)
                GameManager.Instance.speedController.SetSpeedPrev();
            else
                GameManager.Instance.speedController.SetSpeedZero();
        }
        else if(Input.GetKeyDown(key_SpeedControl_One))
        {
            GameManager.Instance.speedController.SetSpeedNormal();
        }
        else if(Input.GetKeyDown(key_SpeedControl_Double))
        {
            GameManager.Instance.speedController.SetSpeedFast();
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpeedControlCheck();
        CameraResetCheck();
        TileMoveCheck();
    }
}
