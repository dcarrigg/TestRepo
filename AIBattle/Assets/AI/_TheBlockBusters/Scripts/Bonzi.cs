using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = GameManager.Direction;

[CreateAssetMenu(fileName = "Bonzi", menuName = "AI/Best of the 90s and 00s/Bonzi")]
public class Bonzi : AIBaseEnt {

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

        CombatantAction returnAction = CombatantAction.Pass;

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

        if (MovementScript.CheckMovement(this, Direction.Current) && (MovementScript.CheckForBomb(this, Direction.Current) || MovementScript.CheckForBomb(this, Direction.Up) || MovementScript.CheckForBomb(this, Direction.Down) || MovementScript.CheckForBomb(this, Direction.Left) || MovementScript.CheckForBomb(this, Direction.Right))) {
            Direction dirToMove = (Direction)UnityEngine.Random.Range(1, 5);
            if (MovementScript.CheckMovement(this, dirToMove))
                aMoves.Add(dirToMove);
            else {
                Direction newDirToMove = (Direction)UnityEngine.Random.Range(1, 5);
                if (newDirToMove != dirToMove && MovementScript.CheckMovement(this, newDirToMove)) {
                    dirToMove = newDirToMove;
                    aMoves.Add(newDirToMove);
                }
            }

            if (MovementScript.CheckMovement(this, dirToMove) && (MovementScript.CheckForBomb(this, Direction.Current) || MovementScript.CheckForBomb(this, Direction.Up) || MovementScript.CheckForBomb(this, Direction.Down) || MovementScript.CheckForBomb(this, Direction.Left) || MovementScript.CheckForBomb(this, Direction.Right))) {
                // There's still a bomb near us, so let's move again
                Direction newNewDir = (Direction)UnityEngine.Random.Range(1, 5);
                if (MovementScript.CheckMovement(this, newNewDir) && !MovementScript.CheckForBomb(this, newNewDir))
                    aMoves.Add(newNewDir);
                else {
                    Direction nextNewDirToMove = (Direction)UnityEngine.Random.Range(1, 5);
                    if (nextNewDirToMove != newNewDir && MovementScript.CheckMovement(this, nextNewDirToMove) && !MovementScript.CheckForBomb(this, nextNewDirToMove)) {
                        dirToMove = nextNewDirToMove;
                        aMoves.Add(nextNewDirToMove);
                    }
                }
            }

            returnAction = CombatantAction.Move;
        }
        else if (MovementScript.CheckMovement(this, Direction.Current) && !MovementScript.CheckForBomb(this, Direction.Current)) {
            aBombTime = UnityEngine.Random.Range(2, 20);
            returnAction = CombatantAction.DropBomb;
        }

        return returnAction;
    }
}