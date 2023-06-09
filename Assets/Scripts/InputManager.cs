using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public TileNode testTile;

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

        // Update is called once per frame
        void Update()
        {

        }
    }
}
