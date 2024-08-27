using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class TileEnd : Tile
{
    TileNode prevTile = null;

    private void AutoRotate(TileNode curTile)
    {
        if (prevTile == curTile)
            return;
        prevTile = curTile;
        AudioManager.Instance.Play2DSound("Card_Tile_E", SettingManager.Instance._FxVolume);

        Direction targetDirection = Direction.None;
        foreach (var direction in curTile.connectionState.Keys)
        {
            if (curTile.connectionState[direction] == 1)
            {
                targetDirection = direction;
                break;
            }
        }

        if (twin.PathDirection[0] == targetDirection)
            return;

        while(twin.PathDirection[0] != targetDirection)
            twin.RotateTile();

    }

    protected override void Update()
    {
        if (isTwin)
            return;

        if (waitToMove)
        {
            TileNode curTile = TileMoveCheck();
            //자동 회전 로직 수행
            if (curTile != null && curTile.setAvail)
                AutoRotate(curTile);

            if (!MovableNow || Input.GetKeyUp(SettingManager.Instance.key_CancelControl._CurKey) || Input.GetKeyDown(KeyCode.Escape))
            {
                EndMoveing();
            }
            else if (Input.GetKeyUp(SettingManager.Instance.key_BasicControl._CurKey) && curTile != null)
            {
                EndMove(curTile);
            }
        }
    }
}
