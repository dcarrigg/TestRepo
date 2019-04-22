/*
Greg St. Angelo IV
4.13.2017
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds tile info.
/// </summary>
public class Node
{
    /// <summary>
    /// Position and some data.
    /// </summary>
    public class Transform
    {
        int m_X = 0;
        int m_Y = 0;

        Data m_Data = new Data();

        public int x { get { return m_X; } set { m_X = value; } }
        public int y { get { return m_Y; } set { m_X = value; } }
        public Data data { get { return m_Data; } set { m_Data = value; } }
    }

    /// <summary>
    /// Holds data about bombs and what not.
    /// </summary>
    public class Data
    {
        int m_BombTime = 0;
        int m_TurnsSinceEnemyBomb = 0;
        bool m_IsBomb = false;
        bool m_IsGoal = false;
        bool m_IsEnemy = false;
        bool m_IsDiamond = false;
        GameManager.Direction m_FoundFrom;
        List<Node> m_ConnectedNodes = new List<Node>();

        public int bombTime { get { return m_BombTime; } set { m_BombTime = value; } }
        public int turnsSinceEnemyBomb { get { return m_TurnsSinceEnemyBomb; } set { m_TurnsSinceEnemyBomb = value; } }
        public bool isGoal { get { return m_IsGoal; } set { m_IsGoal = value; } }
        public bool isEnemy { get { return m_IsEnemy; } set { m_IsEnemy = value; } }
        public bool isBomb { get { return m_IsBomb; } set { m_IsBomb = value; } }
        public bool isDiamond { get { return m_IsDiamond; } set { m_IsDiamond = value; } }
        public GameManager.Direction foundFrom { get { return m_FoundFrom; } set { m_FoundFrom = value; } }
        public List<Node> connectedNodes { get { return m_ConnectedNodes; } set { m_ConnectedNodes = value; } }
    }

    public Transform transform = new Transform();
}
