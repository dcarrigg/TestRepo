using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Direction = GameManager.Direction;
using SensorData = GameManager.SensorData;

public static class MovementScript {

    /// <summary>
    /// Returns surrounding tile information from the given AI 
    /// </summary>
    /// <param name="m_AI">AIBaseEnt to search surrounding tiles with</param>
    /// <returns>Surrounding tile data</returns>
    public static Dictionary<Direction, TileInfo> SearchImmediate (AIBaseEnt m_AI) {
        Dictionary<Direction, TileInfo> data = new Dictionary<Direction, TileInfo>();
        /*
         try {
            data[Direction.Current] = new TileInfo(m_AI.GetSensorData(Direction.Current));
            m_AI.DebugMsg("data[Direction.Current] = " + data[Direction.Current].GetTileData());

            data[Direction.Up] = new TileInfo(m_AI.GetSensorData(Direction.Up));
            m_AI.DebugMsg("data[Direction.Up] = " + data[Direction.Up].GetTileData());

            data[Direction.Down] = new TileInfo(m_AI.GetSensorData(Direction.Down));
            m_AI.DebugMsg("data[Direction.Down] = " + data[Direction.Down].GetTileData());

            data[Direction.Left] = new TileInfo(m_AI.GetSensorData(Direction.Left));
            m_AI.DebugMsg("data[Direction.Left] = " + data[Direction.Left].GetTileData());

            data[Direction.Right] = new TileInfo(m_AI.GetSensorData(Direction.Right));
            m_AI.DebugMsg("data[Direction.Right] = " + data[Direction.Right].GetTileData());
        }
        catch (System.Exception e) {
            m_AI.DebugMsg("MovementScript.Search() error: " + e.Message + "\n" + e.StackTrace);
        }
        */

        data[Direction.Current] = new TileInfo(m_AI.GetSensorData(Direction.Current));
        data[Direction.Up] = new TileInfo(m_AI.GetSensorData(Direction.Up));
        data[Direction.Down] = new TileInfo(m_AI.GetSensorData(Direction.Down));
        data[Direction.Left] = new TileInfo(m_AI.GetSensorData(Direction.Left));
        data[Direction.Right] = new TileInfo(m_AI.GetSensorData(Direction.Right));

        foreach (KeyValuePair<Direction, TileInfo> tdata in data) {
            m_AI.DebugMsg("MovementScript.SearchImmediate(): data[Direction." + tdata.Key.ToString() + "] = " + tdata.Value.GetTileData().ToString());
        }

        return data;
    }

    public static bool CheckMovement (AIBaseEnt m_AI, Direction m_Direction) {

        m_AI.DebugMsg("Checking movement in " + m_Direction + " direction...");

        bool isSafe = false;
        Dictionary<Direction, TileInfo> moveData = SearchImmediate(m_AI);

        if (moveData.ContainsKey(m_Direction) && (moveData[m_Direction].GetTileData() & SensorData.Wall) != 0)
            isSafe = false;
        else if (moveData.ContainsKey(m_Direction) && (moveData[m_Direction].GetTileData() & SensorData.OffGrid) != 0)
            isSafe = false;
        else if (moveData.ContainsKey(m_Direction) && (moveData[m_Direction].GetTileData() & SensorData.Enemy) != 0)
            isSafe = false;
        else
            isSafe = true;

        if (!isSafe)
            m_AI.DebugMsg("MovementScript.CheckMovement() " + m_Direction + " is not safe; SensorData = " + moveData[m_Direction].GetTileData());

        return isSafe;
    }

    public static bool CheckForBomb (AIBaseEnt m_AI, Direction m_Direction) {
        m_AI.DebugMsg("Checking movement in " + m_Direction + " direction...");

        bool isBomb = false;
        Dictionary<Direction, TileInfo> moveData = SearchImmediate(m_AI);

        if (moveData.ContainsKey(m_Direction) && (moveData[m_Direction].GetTileData() & SensorData.Bomb) != 0)
            isBomb = true;
        else
            isBomb = false;

        if (isBomb)
            m_AI.DebugMsg("MovementScript.CheckMovement() " + m_Direction + " is not safe; SensorData = " + moveData[m_Direction].GetTileData());

        return isBomb;
    }
}
