using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = GameManager.Direction;

[CreateAssetMenu(fileName = "Clippy", menuName = "AI/Best of the 90s and 00s/Clippy")]
public class Clippy : AIBaseEnt {

    // Current tile pos -- set our current tile as the origin
    TileInfo.TilePos originTilePosition = new TileInfo.TilePos(0, 0);
    TileInfo.TilePos currentTilePosition = new TileInfo.TilePos(0, 0);

    [SerializeField] Direction lastMove;
    [SerializeField] Direction secondLastMove;

    // Get the action the combatant wants to take
    // If you are returning CombatantAction.Move:
    //      - You must specify how you would like to move by adding data to the aMoves list
    //      - You will move along the path until you bump into something or reach the end
    // If you are returning CombatantAction.DropBomb:
    //      - You must specify what the timer on the bomb should be set by setting the aBombTime argument
    public override CombatantAction GetAction (ref List<Direction> aMoves, ref int aBombTime) {

        // Do logic based on the current mood of the AI

        // If currentAIMood is passive, then map out the area

        /* 
         * =====================================
         * | NOTE                              |
         * |-----------------------------------|
         * | If we move to a new tile, we MUST |
         * | update currentPos!                |
         * |-----------------------------------|
         */

        // Check the surrounding tiles
        UpdateTileMap();

        // Now lets check adjacent tiles of immediate adjacent tiles
        secondLastMove = lastMove;
        if (MovementScript.CheckMovement(this, Direction.Up) && !MovementScript.CheckForBomb(this, Direction.Up) || (MovementScript.CheckMovement(this, Direction.Up) && lastMove != Direction.Up)) {
            aMoves.Add(Direction.Up);
            currentPos = currentPos.Up();
            lastMove = Direction.Up;
        }
        else if (MovementScript.CheckMovement(this, Direction.Right) && !MovementScript.CheckForBomb(this, Direction.Right) || (MovementScript.CheckMovement(this, Direction.Right) && lastMove != Direction.Right)) {
            aMoves.Add(Direction.Right);
            currentPos = currentPos.Right();
            lastMove = Direction.Right;
        }
        else if (MovementScript.CheckMovement(this, Direction.Down) && !MovementScript.CheckForBomb(this, Direction.Down) || (MovementScript.CheckMovement(this, Direction.Down) && lastMove != Direction.Down)) {
            aMoves.Add(Direction.Down);
            currentPos = currentPos.Down();
            lastMove = Direction.Down;
        }
        else if (MovementScript.CheckMovement(this, Direction.Left) && !MovementScript.CheckForBomb(this, Direction.Left) || (MovementScript.CheckMovement(this, Direction.Left) && lastMove != Direction.Left)) {
            aMoves.Add(Direction.Left);
            currentPos = currentPos.Left();
            lastMove = Direction.Left;
        }
        else
            aMoves.Add(Direction.Current);

        return CombatantAction.Move;
    }
}