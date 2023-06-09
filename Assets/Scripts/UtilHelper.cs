using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public static class UtilHelper
{
    //public static Direction ModifyDirection(Direction direction)
    //{
    //    switch (direction)
    //    {
    //        case Direction.Left:
    //            break;
    //        case Direction.LeftUp:
    //            break;
    //        case Direction.LeftDown:
    //            break;
    //        case Direction.Right:
    //            break;
    //        case Direction.RightUp:
    //            break;
    //        case Direction.RightDown:
    //            break;
    //    }
    //}

    public static Direction ReverseDirection(Direction direction)
    {
        switch(direction)
        {
            case Direction.Left:
                return Direction.Right;
            case Direction.LeftDown:
                return Direction.RightUp;
            case Direction.LeftUp:
                return Direction.RightDown;
            case Direction.Right:
                return Direction.Left;
            case Direction.RightDown:
                return Direction.LeftUp;
            case Direction.RightUp:
                return Direction.LeftDown;
        }

        //코드탈일 없음
        return Direction.None;
    }

    public static Vector3 GetGridPosition(Vector3 curPos, Direction direction, float distance)
    {
        float angle = 0f;
        switch (direction)
        {
            case Direction.LeftUp:
                angle = 120f;
                break;
            case Direction.RightUp:
                angle = 60f;
                break;
            case Direction.Left:
                angle = 180f;
                break;
            case Direction.LeftDown:
                angle = -120f;
                break;
            case Direction.RightDown:
                angle = -60f;
                break;
        }

        float angleInRadians = angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
        dir *= distance;
        return curPos + dir;
    }

    public static IEnumerator ScaleLerp(Transform target, float startScale, float endScale, float lerpTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float nextScale = Mathf.Lerp(startScale, endScale, elapsedTime / lerpTime);
            target.localScale = new Vector3(nextScale, nextScale, nextScale);
            yield return null;
        }
        target.localScale = new Vector3(endScale, endScale, endScale);
    }

    public static void ActiveTrigger(Transform transform, string triggerName)
    {
        Animator animator = GetComponetInChildren<Animator>(transform);
        if (animator != null)
            animator.SetTrigger(triggerName);
    }

    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static IEnumerator DelayedFunctionCall(UnityAction func, float delayTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < delayTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        func();
    }

    public static IEnumerator ReActiveCollider(Collider2D collider, float delayTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < delayTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        collider.isTrigger = false;
    }

    public static T Find<T>(Transform transform, string path, bool init = false) where T : Component
    {
        Transform trans = transform.Find(path);
        T findObject = null;
        if (trans != null)
        {
            findObject = trans.GetComponent<T>();
            if (init)
                trans.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
        }

        return findObject;
    }

    public static T GetComponetInChildren<T>(Transform transform, bool init = false) where T : Component
    {
        T t = transform.GetComponentInChildren<T>();
        if (t != null && init)
            t.SendMessage("Init", SendMessageOptions.DontRequireReceiver);

        return t;
    }

    public static T FindobjectOfType<T>(bool init = false) where T : Component
    {
        T t = GameObject.FindObjectOfType<T>();
        if (t != null)
        {
            if (init)
                t.transform.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
        }
        return t;
    }

    // 타겟이 되는 transform / 타겟으로부터의 경로 / 연결할 함수
    public static Button BindingFunc(Transform transform, string path, UnityAction action)
    {
        Button button = Find<Button>(transform, path);
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
        return button;
    }

    public static Button BindingFunc(Transform transform, UnityAction action)
    {
        Button button = transform.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
        return button;
    }

    public static T Instantiate<T>(string path, Transform parent, bool init = false, bool active = true) where T : UnityEngine.Component
    {
        T objectType = Resources.Load<T>(path);
        if (objectType != null)
        {
            objectType = Object.Instantiate(objectType);
            if (objectType != null)
            {
                if (init)
                    objectType.SendMessage("Init", SendMessageOptions.DontRequireReceiver);

                objectType.gameObject.SetActive(active);
            }
        }
        return objectType;

    }

    public static T CreateObject<T>(Transform parent, bool init = false) where T : Component
    {
        GameObject obj = new GameObject(typeof(T).Name, typeof(T));
        obj.transform.SetParent(parent);
        T t = obj.GetComponent<T>();
        if (init)
            t.SendMessage("Init", SendMessageOptions.DontRequireReceiver);

        return t;
    }
}
