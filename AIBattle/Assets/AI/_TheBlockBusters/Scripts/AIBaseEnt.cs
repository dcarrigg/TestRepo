using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = GameManager.Direction;
using SensorData = GameManager.SensorData;
using TilePos = TileInfo.TilePos;

[CreateAssetMenu(fileName = "AIBaseEnt", menuName = "AI/Best of the 90s and 00s/AIBaseEnt")]
public class AIBaseEnt : BaseAI {

    public bool enableDebugging = true;

    public class TurnData {
        public List<Direction> moveSet;
        public int bombTimer = 1;
        public TurnData () { }
        public TurnData (List<Direction> aMoveSet) { moveSet = aMoveSet; }
        public TurnData (int aBombTimer) { bombTimer = aBombTimer; }
    }

    public Dictionary<TilePos, TileInfo> adjacentTileData = new Dictionary<TilePos, TileInfo>();

    public TilePos currentPos = new TilePos(0, 0);

    public override CombatantAction GetAction (ref List<Direction> aMoves, ref int aBombTime) {
        return CombatantAction.Pass;
    }

    public SensorData GetSensorData (Direction m_Direction) {
        // Used for MovementScript
        return UseSensor(m_Direction);
    }

    public void UpdateTileMap () {
        DebugMsg("Updating tile map...");
        Dictionary<Direction, TileInfo> initAdjacent = MovementScript.SearchImmediate(this);

        if (initAdjacent != null || initAdjacent.Count != 0) {
            adjacentTileData[currentPos] = initAdjacent[Direction.Current];
            // DebugMsg("Added/updated entry for " + currentPos.ToString() + " >> " + adjacentTileData[currentPos].GetTileData().ToString());
            adjacentTileData[currentPos.Up()] = initAdjacent[Direction.Up];
            // DebugMsg("Added/updated entry for " + currentPos.Up().ToString() + " >> " + adjacentTileData[currentPos.Up()].GetTileData().ToString());
            adjacentTileData[currentPos.Down()] = initAdjacent[Direction.Down];
            // DebugMsg("Added/updated entry for " + currentPos.Down().ToString() + " >> " + adjacentTileData[currentPos.Down()].GetTileData().ToString());
            adjacentTileData[currentPos.Left()] = initAdjacent[Direction.Left];
            // DebugMsg("Added/updated entry for " + currentPos.Left().ToString() + " >> " + adjacentTileData[currentPos.Left()].GetTileData().ToString());
            adjacentTileData[currentPos.Right()] = initAdjacent[Direction.Right];
            // DebugMsg("Added/updated entry for " + currentPos.Right().ToString() + " >> " + adjacentTileData[currentPos.Right()].GetTileData().ToString());

            /*
            foreach (KeyValuePair<TilePos, TileInfo> tiledata in adjacentTileData) {
                DebugMsg(tiledata.Key.ToString() + " > " + tiledata.Value.GetTileData().ToString());
            }
            */
        }
        else
            DebugMsg("Error updating tile map; MovementScript.SearchImmediate() returned null or no adjacent positions.");

        DebugMsg("Done updating tile map!");
    }

    public void DebugMsg (string msg) {
        if (enableDebugging)
            Debug.Log(msg);
    }
}