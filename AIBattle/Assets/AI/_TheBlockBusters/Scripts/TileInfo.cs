using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = GameManager.Direction;
using SensorData = GameManager.SensorData;

[System.Serializable]
public class TileInfo {

    [System.Serializable]
    public class TilePos {
        public int x;
        public int y;
        public TilePos(int m_X, int m_Y) { x = m_X; y = m_Y; }

        public TilePos Left () { return new TilePos(x - 1, y); }
        public TilePos Right () { return new TilePos(x + 1, y); }
        public TilePos Up () { return new TilePos(x, y + 1); }
        public TilePos Down () { return new TilePos(x, y - 1); }

        public override string ToString() {
            return "(" + x.ToString() + ", " + y.ToString() + ")";
        }
    }

    Dictionary<Direction, TileInfo> adjacentTiles; // Dictionary of TileInfo in adjacent tiles
    SensorData tileData; // Current state of the tile (Clear, Bomb, etc)

    public bool isGoal = false;
    public bool isDiamond = false;

    public TileInfo(SensorData m_TileData) {
        tileData = m_TileData;

        if ((tileData & SensorData.Diamond) != 0) isDiamond = true;
        if ((tileData & SensorData.Goal) != 0) isGoal = true;
    }

    public SensorData GetTileData () { return tileData; }
    public void SetTileData(SensorData m_TileData) { tileData = m_TileData; }
    public TileInfo GetAdjacentTileInfo (Direction m_TileDirection) { return adjacentTiles[m_TileDirection]; }

    /// <summary>
    /// Updates information in the direction of an adjacent tile
    /// </summary>
    /// <param name="m_Direction">Direction of the tile to update (Up, Down, Left, Right)</param>
    /// <param name="m_AdjacentTileData">SensorData of the adjacent tile</param>
    public void UpdateAdjacentTileData(Direction m_Direction, SensorData m_AdjacentTileData) {
        adjacentTiles[m_Direction].SetTileData(m_AdjacentTileData);
    }
}
