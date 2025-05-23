using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObjectHelper : MonoBehaviour
{
    [SerializeField]
    Transform roationTarget;

    private float zOffset = -0.1f;

    // Update is called once per frame
    void Update()
    {
        float offset = roationTarget.rotation.eulerAngles.y < 90f ? zOffset : -zOffset;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, offset);
    }
}
