using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using reference so we don't have to type GameManager.Whatever every time we want to access one of its enums
using SensorData = GameManager.SensorData;
using Direction = GameManager.Direction;

// Making my own namespace just in case somebody has an AI with the same name...
namespace CJ_Slav
{
    [CreateAssetMenu(fileName = "TestAI", menuName = "AI/Toaster")]
    public class AI_Character : BaseAI
    {
        [System.Serializable]
        public struct MinMax
        {
            public int min, max;

            public MinMax(int min, int max)
            {
                this.min = min;
                this.max = max;
            }
        }

        [SerializeField]
        MinMax m_BombTime = new MinMax(3, 6);

        private SensorData m_RightTile;
        private SensorData m_LeftTile;
        private SensorData m_UpTile;
        private SensorData m_DownTile;
        private SensorData m_CurrentTile;
        private SensorData[] m_DangerousAreas = { SensorData.Bomb, SensorData.OffGrid, SensorData.Wall };

        private Node m_CurrentNode;
        private Dictionary<int, Dictionary<int, Node>> m_Map = new Dictionary<int, Dictionary<int, Node>>();

        // Coordinates for the map
        private int m_Vertical = 0;
        private int m_Horizontal = 0;

        public override CombatantAction GetAction(ref List<Direction> aMoves, ref int aBombTime)
        {
            // Check all directions
            m_RightTile = UseSensor(Direction.Right);
            m_LeftTile = UseSensor(Direction.Left);
            m_UpTile = UseSensor(Direction.Up);
            m_DownTile = UseSensor(Direction.Down);
            m_CurrentTile = UseSensor(Direction.Current);

            // Update the surrounding tiles
            UpdateSurroundingTiles();

            aBombTime = Random.Range(m_BombTime.min, m_BombTime.max + 1);

            // Managing Movement

            // For each node, check to make sure that the node was not yet visited and is not dangerous, or 
            // check if the diamond is there
            // If so, then try to move to that position
            if (m_CurrentNode.left.visited == false && m_CurrentNode.left.tile.Contains(m_DangerousAreas) == false
                || m_CurrentNode.left.tile.Contains(SensorData.Diamond))
            {
                // If an enemy is there, drop a bomb
                if (m_CurrentNode.left.tile.Contains(SensorData.Enemy))
                    return CombatantAction.DropBomb;
                aMoves.Add(Direction.Left);
            }
            // Same logic as the first part, but for different nodes
            else if (m_CurrentNode.right.visited == false && m_CurrentNode.right.tile.Contains(m_DangerousAreas) == false
                || m_CurrentNode.right.tile.Contains(SensorData.Diamond))
            {
                if (m_CurrentNode.right.tile.Contains(SensorData.Enemy))
                    return CombatantAction.DropBomb;
                aMoves.Add(Direction.Right);
            }
            else if (m_CurrentNode.up.visited == false && m_CurrentNode.up.tile.Contains(m_DangerousAreas) == false
                || m_CurrentNode.up.tile.Contains(SensorData.Diamond))
            {
                if (m_CurrentNode.up.tile.Contains(SensorData.Enemy))
                    return CombatantAction.DropBomb;
                aMoves.Add(Direction.Up);
            }
            else if (m_CurrentNode.down.visited == false && m_CurrentNode.down.tile.Contains(m_DangerousAreas) == false
                || m_CurrentNode.down.tile.Contains(SensorData.Diamond))
            {
                if (m_CurrentNode.down.tile.Contains(SensorData.Enemy))
                    return CombatantAction.DropBomb;
                aMoves.Add(Direction.Down);
            }
            else
            {
                // Go into a random direction if all tiles are already visited
                // keep re-rolling until we get a proper direction, or until we've tried 10 more times
                Direction dir = (Direction)Random.Range(0, 4);
                SensorData targetTile = UseSensor(dir);
                for (int i = 0; i < 10; i++)
                {
                    if (targetTile.Contains(m_DangerousAreas))
                    {
                        dir = (Direction)Random.Range(0, 4);
                        targetTile = UseSensor(dir);
                    }
                    else break;
                }
                // just pass if for some reason, our AI wants to walk into a wall
                if (targetTile.Contains(SensorData.OffGrid, SensorData.Wall))
                {
                    return CombatantAction.Pass;
                }
                else if (targetTile.Contains(SensorData.Enemy))
                    return CombatantAction.DropBomb;

                // Just move to the position
                aMoves.Add(dir);
            }

            // This will update the coordinates and return a movement action
            CombatantAction action = MoveAndUpdateCoords(ref aMoves);
            Debug.LogFormat("[{0}][{1}]", m_Horizontal, m_Vertical);
            return action;
        }

        // Updates the tiles surrounding the AI
        private void UpdateSurroundingTiles()
        {
            // Check if the current column actually exists, and add it if it doesn't
            if (m_Map.ContainsKey(m_Horizontal) == false || m_Map[m_Horizontal] == null)
                m_Map[m_Horizontal] = new Dictionary<int, Node>();
            if (m_Map.ContainsKey(m_Horizontal + 1) == false || m_Map[m_Horizontal + 1] == null)
                m_Map[m_Horizontal + 1] = new Dictionary<int, Node>();
            if (m_Map.ContainsKey(m_Horizontal - 1) == false || m_Map[m_Horizontal - 1] == null)
                m_Map[m_Horizontal - 1] = new Dictionary<int, Node>();

            // Update the nodes
            // Make sure that they are actually present in the dictionary
            if (m_Map[m_Horizontal].ContainsKey(m_Vertical) == false || m_Map[m_Horizontal][m_Vertical] == null)
                m_Map[m_Horizontal][m_Vertical] = new Node();

            if (m_Map[m_Horizontal].ContainsKey(m_Vertical + 1) == false || m_Map[m_Horizontal][m_Vertical] == null)
                m_Map[m_Horizontal][m_Vertical + 1] = new Node();

            if (m_Map[m_Horizontal].ContainsKey(m_Vertical - 1) == false || m_Map[m_Horizontal][m_Vertical] == null)
                m_Map[m_Horizontal][m_Vertical - 1] = new Node();

            if (m_Map[m_Horizontal + 1].ContainsKey(m_Vertical) == false || m_Map[m_Horizontal][m_Vertical] == null)
                m_Map[m_Horizontal + 1][m_Vertical] = new Node();

            if (m_Map[m_Horizontal - 1].ContainsKey(m_Vertical) == false || m_Map[m_Horizontal][m_Vertical] == null)
                m_Map[m_Horizontal - 1][m_Vertical] = new Node();

            // Set the relative nodes (so we can specify them as just currentNode.up for example)
            m_CurrentNode = m_Map[m_Horizontal][m_Vertical];
            m_CurrentNode.up = m_Map[m_Horizontal][m_Vertical + 1];
            m_CurrentNode.down = m_Map[m_Horizontal][m_Vertical - 1];
            m_CurrentNode.right = m_Map[m_Horizontal + 1][m_Vertical];
            m_CurrentNode.left = m_Map[m_Horizontal - 1][m_Vertical];

            // Update the tiles surrounding us
            m_CurrentNode.tile = m_CurrentTile;
            m_CurrentNode.up.tile = m_UpTile;
            m_CurrentNode.down.tile = m_DownTile;
            m_CurrentNode.right.tile = m_RightTile;
            m_CurrentNode.left.tile = m_LeftTile;

            // Mark the current node as visited
            // NOTE: we could also have a visited count or priority
            m_CurrentNode.visited = true;
        }

        // Updates the coordinates and then makes a move
        CombatantAction MoveAndUpdateCoords(ref List<Direction> moves)
        {
            for (int i = 0; i < moves.Count; i++)
            {
                // check which direction the move is in
                // increase or decrease the distance based on the direction
                switch (moves[i])
                {
                    case Direction.Down:
                        m_Vertical--;
                        break;
                    case Direction.Up:
                        m_Vertical++;
                        break;
                    case Direction.Left:
                        m_Horizontal--;
                        break;
                    case Direction.Right:
                        m_Horizontal++;
                        break;
                }
            }
            return CombatantAction.Move;
        }
    }

    // Extensions to make things more convenient
    public static class Extensions
    {
        /// <summary>
        /// Does the tile we just looked at contain the <see cref="data"/> passed in?
        /// </summary>
        /// <param name="self"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Contains(this SensorData self, params SensorData[] data)
        {
            // If self contains a bit from ANY of the values passed in, return true
            for (int i = 0; i < data.Length; i++)
            {
                if ((self & data[i]) != 0)
                    return true;
            }
            return false;
        }
    }
}
