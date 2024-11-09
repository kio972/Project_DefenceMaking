using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Linq;

public static class UtilHelper
{
    public static List<Tile> GetPathCount(Tile startTile)
    {
        List<Tile> visited = new List<Tile> { startTile };
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(startTile);

        while (queue.Count != 0)
        {
            Tile next = queue.Dequeue();
            foreach (Direction dir in next.PathDirection)
            {
                TileNode targetNode = next.curNode.DirectionalNode(dir);
                // 대상 방향에 길 타일 없음
                if (targetNode == null || targetNode.curTile == null || targetNode.curTile._TileType != TileType.Path)
                    continue;

                // 이미 방문한 타일임
                if (visited.Contains(targetNode.curTile))
                    continue;

                // 대상 방향으로 길 연결되어있지 않음
                if (!targetNode.curTile.PathDirection.Contains(UtilHelper.ReverseDirection(dir)))
                    continue;

                queue.Enqueue(targetNode.curTile);
                visited.Add(targetNode.curTile);
            }
        }

        return visited;
    }

    public static bool ContainsParam(this Animator _Anim, string _ParamName)
    {
        foreach (AnimatorControllerParameter param in _Anim.parameters)
        {
            if (param.name == _ParamName) return true;
        }
        return false;
    }

    public static List<T> ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();
        List<T> values = new List<T>(list);
        for (int i = values.Count - 1; i > 0; i--)
        {
            int k = random.Next(i + 1);
            T value = values[k];
            values[k] = values[i];
            values[i] = value;
        }

        return values;
    }

    public static (int q, int r, int s) OffsetToCube(int row, int col)
    {
        int q = col - (row + (row & 1)) / 2;
        int r = row;
        int s = -q - r;
        return (q, r, s);
    }

    public static int HexDistanceFromOrigin(int q, int r, int s) => (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s)) / 2;
    public static int HexDistance(int q1, int r1, int s1, int q2, int r2, int s2) => (Mathf.Abs(q1 - q2) + Mathf.Abs(r1 - r2) + Mathf.Abs(s1 - s2)) / 2;

    public static int GetTileDistance(int row, int col)
    {
        var (q, r, s) = OffsetToCube(col, row);
        return HexDistanceFromOrigin(q, r, s);
    }

    public static int GetTileDistance(int originRow, int originCol, int targetRow, int targetCol)
    {
        var (originQ, originR, originS) = OffsetToCube(originRow, originCol);
        var (targetQ, targetR, targetS) = OffsetToCube(targetRow, targetCol);
        return HexDistance(originQ, originR, originS, targetQ, targetR, targetS);
    }

    public static IEnumerator MoveToTargetPos(Transform moveTarget, Vector3 targetPos, float lerpTime = 0.5f)
    {
        targetPos.y = moveTarget.position.y;
        targetPos.z = moveTarget.position.z;
        Vector3 startPos = moveTarget.position;
        float elapsedTime = 0f;
        while (elapsedTime < lerpTime)
        {
            moveTarget.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / lerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        moveTarget.transform.position = targetPos;
    }

    public static int GetConnectedCount(Tile tile)
    {
        if(tile.curNode == null)
            return -1;

        int count = 0;

        foreach(Direction direction in tile.PathDirection)
        {
            if (tile.curNode.neighborNodeDic[direction] == null || tile.curNode.neighborNodeDic[direction].curTile == null)
                continue;

            Direction reversedDir = ReverseDirection(direction);
            if (tile.curNode.neighborNodeDic[direction].curTile.PathDirection.Contains(reversedDir))
                count++;
        }

        foreach (Direction direction in tile.RoomDirection)
        {
            if (tile.curNode.neighborNodeDic[direction] == null || tile.curNode.neighborNodeDic[direction].curTile == null)
                continue;

            Direction reversedDir = ReverseDirection(direction);
            if(tile.curNode.neighborNodeDic[direction].curTile.RoomDirection.Contains(reversedDir))
                count++;
        }

        return count;
    }

    public static async UniTask ScaleEffect(Transform target, Vector3 targetScale, float lerpTime, CancellationTokenSource cancellationToken)
    {
        await UniTask.Yield(cancellationToken.Token);

        float elapsedTime = 0f;
        Vector3 originPositioin = target.transform.localScale;
        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);
            target.transform.localScale = Vector3.Lerp(originPositioin, targetScale, t * t * t * (t * (6f * t - 15f) + 10f));
            await UniTask.Yield(cancellationToken.Token);
        }

        target.transform.localScale = targetScale;
    }

    public static async UniTask MoveEffect(Transform target, Vector3 targetPosition, float lerpTime, CancellationTokenSource cancellationToken)
    {
        await UniTask.Yield(cancellationToken.Token);

        float elapsedTime = 0f;
        Vector3 originPositioin = target.transform.position;
        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);
            target.position = Vector3.Lerp(originPositioin, targetPosition, t * t * t * (t * (6f * t - 15f) + 10f));
            await UniTask.Yield(cancellationToken.Token);
        }

        target.position = targetPosition;
    }


    public static IEnumerator IMoveEffect(RectTransform transform, Vector3 targetPosition, float lerpTime, System.Action callback = null)
    {
        //targetPosition = originPositioin + targetPosition;
        float elapsedTime = 0f;
        Vector3 originPositioin = transform.anchoredPosition;
        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);

            transform.anchoredPosition = Vector3.Lerp(originPositioin, targetPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }
        yield return null;
        callback?.Invoke();
    }

    public static IEnumerator IMoveEffect(Transform transform, Vector3 targetPosition, float lerpTime, System.Action callback = null)
    {
        //targetPosition = originPositioin + targetPosition;
        float elapsedTime = 0f;
        Vector3 originPositioin = transform.position;
        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);

            transform.position = Vector3.Lerp(originPositioin, targetPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }
        yield return null;
        callback?.Invoke();
    }

    public static IEnumerator IMoveEffect(Transform transform, Vector3 originPositioin, Vector3 targetPosition, float lerpTime, System.Action callback = null)
    {
        //targetPosition = originPositioin + targetPosition;
        float elapsedTime = 0f;
        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);

            transform.position = Vector3.Lerp(originPositioin, targetPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }
        yield return null;
        callback?.Invoke();
    }

    public static async UniTask IMoveEffect(Transform transform, Vector3 originPositioin, Vector3 targetPosition, float lerpTime, CancellationToken cancellationToken, System.Action callback = null)
    {
        //targetPosition = originPositioin + targetPosition;
        await UniTask.Yield();
        float elapsedTime = 0f;
        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);

            transform.position = Vector3.Lerp(originPositioin, targetPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
            await UniTask.Yield(cancellationToken);
        }
        await UniTask.Yield(cancellationToken);
        callback?.Invoke();
    }

    public static async UniTask IColorEffect(Transform transform, Color startColor, Color targetColor, float lerpTime, System.Action callback = null)
    {
        GameObject gameObject = transform.gameObject;
        Image[] cardImgs = transform.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] texts = transform.GetComponentsInChildren<TextMeshProUGUI>();
        float elapsedTime = 0f;
        foreach (Image temp1 in cardImgs)
            temp1.color = startColor;
        foreach (TextMeshProUGUI temp2 in texts)
            temp2.color = startColor;

        await UniTask.Yield();

        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);

            // cardImgs와 texts의 color의 알파값 조정
            foreach (Image img in cardImgs)
            {
                Color currentColor = Color.Lerp(startColor, targetColor, 1f - Mathf.Cos(t * Mathf.PI * 0.5f));
                img.color = currentColor;
            }

            foreach (TextMeshProUGUI text in texts)
            {
                Color currentColor = Color.Lerp(startColor, targetColor, 1f - Mathf.Cos(t * Mathf.PI * 0.5f));
                text.color = currentColor;
            }

            await UniTask.Yield(cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }
        await UniTask.Yield();

        callback?.Invoke();
    }

    public static async UniTask IScaleEffect(Transform transform, Vector3 startScale, Vector3 targetScale, float lerpTime, CancellationToken cancellationToken, System.Action callback = null)
    {
        //뽑히는 카드의 스케일 조정
        //lerpTime에 걸쳐 transform.scale을 0에서 1로 변경
        float elapsedTime = 0f;
        transform.localScale = startScale;

        await UniTask.Yield();

        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);
            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, t);
            transform.localScale = currentScale;

            await UniTask.Yield(cancellationToken);
        }
        await UniTask.Yield(cancellationToken);

        callback?.Invoke();
    }

    public static IEnumerator IScaleEffect(Transform transform, Vector3 startScale, Vector3 targetScale, float lerpTime = 0.5f, System.Action callback = null)
    {
        //뽑히는 카드의 스케일 조정
        //lerpTime에 걸쳐 transform.scale을 0에서 1로 변경
        float elapsedTime = 0f;
        transform.localScale = startScale;

        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpTime);
            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, t);
            transform.localScale = currentScale;

            yield return null;
        }
        yield return null;

        callback?.Invoke();
    }

    public static int GetDirectionUnClosed(TileNode curNode, List<Direction> directions, bool isRoom = false)
    {
        int count = 0;
        foreach (Direction direction in directions)
        {
            TileNode target = curNode.neighborNodeDic[direction];
            if (target == null || target.curTile == null)
                count++;
            else
            {
                if (!isRoom && IsTileConnected(curNode, directions, target.curTile.PathDirection))
                    count++;
                else if (isRoom && IsTileConnected(curNode, directions, target.curTile.RoomDirection))
                    count++;
            }
        }

        return count;
    }

    public static GameObject GetCardPrefab(CardType type, string targetName)
    {
        string targetPrefabPath = "Prefab/";
        switch (type)
        {
            case CardType.PathTile:
                targetPrefabPath += "Tile";
                break;
            case CardType.RoomTile:
                targetPrefabPath += "Tile";
                break;
            case CardType.Monster:
                targetPrefabPath += "Monster";
                break;
            case CardType.Spawner:
                targetPrefabPath += "Monster";
                break;
            case CardType.Trap:
                targetPrefabPath += "Trap";
                break;
            case CardType.Environment:
                targetPrefabPath += "Environment";
                break;
            default:
                targetPrefabPath += "Objects";
                break;
        }
        targetPrefabPath = targetPrefabPath + "/" + targetName;
        return Resources.Load<GameObject>(targetPrefabPath);
    }

    public static void SetUIColor(Color color, GameObject ui)
    {
        Image[] images = ui.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] texts = ui.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (Image image in images)
            image.color = color;
        foreach (TextMeshProUGUI text in texts)
            text.color = color;
    }

    public static float CalCulateDistance(Transform origin, Transform target)
    {
        Vector3 originPos = origin.position;
        Collider targetCol = target.GetComponent<Collider>();
        if (targetCol == null)
            return 0f;
        Vector3 targetPos = targetCol.ClosestPoint(originPos);

        return Vector3.Distance(originPos, targetPos);
    }

    public static Direction CheckClosestDirection(Vector3 directionVector)
    {
        Dictionary<Direction, Vector3> directionVectors = new Dictionary<Direction, Vector3>()
        {
            { Direction.LeftUp, new Vector3(-1f, 1f, 0f).normalized },
            { Direction.RightUp, new Vector3(1f, 1f, 0f).normalized },
            { Direction.Left, new Vector3(-1f, 0f, 0f).normalized },
            { Direction.Right, new Vector3(1f, 0f, 0f).normalized },
            { Direction.LeftDown, new Vector3(-1f, -1f, 0f).normalized },
            { Direction.RightDown, new Vector3(1f, -1f, 0f).normalized },
        };

        float minAngle = float.MaxValue;
        Direction closestDirection = Direction.None;

        foreach (KeyValuePair<Direction, Vector3> kvp in directionVectors)
        {
            float angle = Vector3.Angle(directionVector, kvp.Value);
            if (angle < minAngle)
            {
                minAngle = angle;
                closestDirection = kvp.Key;
            }
        }

        return closestDirection;
    }

    public static float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        else if (angle < -180f)
            angle += 360f;
        return angle;
    }

    public static Vector3 NormalizeEulerAngles(Vector3 eulerAngles)
    {
        eulerAngles.x = NormalizeAngle(eulerAngles.x);
        eulerAngles.y = NormalizeAngle(eulerAngles.y);
        eulerAngles.z = NormalizeAngle(eulerAngles.z);
        return eulerAngles;
    }

    public static Quaternion AlignUpWithVector(Vector3 direction)
    {
        Vector3 targetUp = direction.normalized;

        // 두 벡터 사이의 각도 계산
        float angle = Vector3.Angle(Vector3.up, targetUp) - 90;

        // 벡터의 외적을 사용하여 회전 축 계산
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, targetUp);

        // 회전 축과 각도를 사용하여 Quaternion 생성
        Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);

        return rotation;
    }

    public static List<TKey> GetKeyValues<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        List<TKey> keyValues = new List<TKey>(dictionary.Keys);
        return keyValues;
    }

    public static bool IsTileConnected(TileNode curNode, Direction direction, List<Direction> targetTile_Direction)
    {
        if (curNode.neighborNodeDic.ContainsKey(direction))
        {
            Tile targetTile = curNode.neighborNodeDic[direction].curTile;
            if (targetTile == null)
                return false;
            direction = ReverseDirection(direction);
            if (targetTile_Direction.Contains(direction))
                return true;
        }
        return false;
    }

    public static bool IsTileConnected(TileNode curNode, List<Direction> curTile_Direction, List<Direction> targetTile_Direction)
    {
        foreach (Direction direction in targetTile_Direction)
        {
            if (curNode.neighborNodeDic.ContainsKey(direction))
            {
                if (IsTileConnected(curNode, direction, targetTile_Direction))
                    return true;
            }
        }

        return false;
    }

    private static List<TileNode> GetRoomConnection(TileNode curNode)
    {
        List<TileNode> nodes = new List<TileNode>();

        foreach (Direction direction in curNode.curTile.RoomDirection)
        {
            TileNode tempNode = curNode.DirectionalNode(direction);
            if (tempNode == null || tempNode.curTile == null || !NodeManager.Instance._ActiveNodes.Contains(tempNode))
                continue;

            if (tempNode.curTile._TileType == TileType.Path)
                continue;

            if (tempNode.curTile.RoomDirection.Contains(ReverseDirection(direction)))
                nodes.Add(tempNode);
        }

        return nodes;
    }

    private static List<TileNode> GetPathConnection(TileNode curNode)
    {
        List<TileNode> nodes = new List<TileNode>();
        foreach (Direction direction in curNode.curTile.PathDirection)
        {
            TileNode tempNode = curNode.DirectionalNode(direction);
            if (tempNode == null || tempNode.curTile == null || !NodeManager.Instance._ActiveNodes.Contains(tempNode))
                continue;

            if (tempNode.curTile._TileType == TileType.Room)
                continue;

            if (tempNode.curTile.PathDirection.Contains(ReverseDirection(direction)))
                nodes.Add(tempNode);
        }

        return nodes;
    }

    public static List<TileNode> GetConnectedNodes(TileNode curNode)
    {
        List<TileNode> nodes = new List<TileNode>();
        if (curNode.curTile == null)
            return nodes;

        if (curNode.curTile._TileType == TileType.Room)
            return GetRoomConnection(curNode);
        else if (curNode.curTile._TileType == TileType.Door)
        {
            foreach (TileNode node in GetRoomConnection(curNode))
                nodes.Add(node);
            foreach (TileNode node in GetPathConnection(curNode))
                nodes.Add(node);

            return nodes;
        }
        else
            return GetPathConnection(curNode);
    }

    public static List<GameObject> RayCastLayer(HashSet<int> targetLayer)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return null;

        List<GameObject> result = new List<GameObject>();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray).OrderBy(h => h.distance).ToArray();

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            if (targetLayer.Contains(hitObject.layer))
                result.Add(hitObject);
        }

        return result;
    }

    public static TileNode RayCastTile()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;
            TileNode tile = hitObject.GetComponentInParent<TileNode>();
            if (tile != null)
                return tile;
        }

        return null;
    }

    public static void SetAvail(bool value, List<TileNode> targetNode)
    {
        foreach (TileNode node in targetNode)
        {
            node.SetAvail(value);
        }
    }

    public static Vector3 GetMouseWorldPosition(float yPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, -yPosition);
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            return point;
        }

        // groundPlane과 교차하지 않을 경우, playerPosition을 반환합니다.
        return Vector3.zero;
    }

    public static Dictionary<string, object> Get_ID_Data(object target, List<Dictionary<string, object>> targetDic, string key = "ID")
    {
        for (int i = 0; i < targetDic.Count; i++)
        {
            if (targetDic[i][key].ToString() == target.ToString())
            {
                return targetDic[i];
            }
        }
        return null; // 일치하는 데이터가 없을 경우 null 반환
    }

    public static int Find_Data_Index(object target, List<Dictionary<string, object>> targetDic, string key = "ID")
    {
        for (int i = 0; i < targetDic.Count; i++)
        {
            if (targetDic[i][key].ToString() == target.ToString())
            {
                return i;
            }
        }
        return -1; // 일치하는 데이터가 없을 경우 -1 반환
    }

    public static T Find_Prefab<T>(int id, List<Dictionary<string, object>> dataDic) where T : UnityEngine.Object
    {
        int index = Find_Data_Index(id, dataDic);
        string prefabPath = dataDic[index]["Prefab"].ToString();
        T prefab = Resources.Load<T>(prefabPath);
        return prefab;
    }

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
