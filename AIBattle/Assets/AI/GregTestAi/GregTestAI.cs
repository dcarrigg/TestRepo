/*
Greg St. Angelo IV
4.13.2017
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GregTestAI", menuName = "AI/Greg Test AI")]
public class GregTestAI : BaseAI
{
    Map m_Map = new Map();
    GameManager.Direction m_CurrentDirection;
    GameManager.Direction m_LastDirection;
    List<GameManager.Direction> m_PriorMoves = new List<GameManager.Direction>();

    bool m_IsActionQueueable = true;
    bool m_IsMoving;
    bool m_IsBombing;
    bool m_IsHoldingDiamond;
    bool m_IsGoingToGoal;
    bool m_IsDodged;

    int m_NumOfSquaresScanned;
    int test = 0;

    Dictionary<Node, GameManager.Direction> m_Visited;

    public override CombatantAction GetAction(ref List<GameManager.Direction> aMoves, ref int aBombTime)
    {
        test++;
        m_NumOfSquaresScanned = 0;
        m_LastDirection = m_CurrentDirection;
        RunCheck(GameManager.Direction.Current);
        RunCheck(GameManager.Direction.Down);
        RunCheck(GameManager.Direction.Left);
        RunCheck(GameManager.Direction.Right);
        RunCheck(GameManager.Direction.Up);

        // get the first node we found this turn
        LinkedListNode<Node> currentLNode = m_Map.nodes.Last;
        int count = m_NumOfSquaresScanned;
        while (count > 1)
        {
            // two way connections
            //if (currentLNode.Previous.Value != null)
            //    currentLNode.Value.transform.data.connectedNodes.Add(currentLNode.Previous.Value);

            currentLNode = currentLNode.Previous;

            //if (currentLNode.Next.Value != null)
            //    currentLNode.Value.transform.data.connectedNodes.Add(currentLNode.Next.Value);

            count--;
        }

        Node currentNode = currentLNode.Value;

        // add nodes to the list of connected nodes
        currentLNode = currentLNode.Next;
        while (currentLNode != null)
        {
            currentNode.transform.data.connectedNodes.Add(currentLNode.Value);
            currentLNode = currentLNode.Next;
        }

        QueueAction(ref aMoves, ref aBombTime);

        // if we have the diamond
        if (m_IsHoldingDiamond)
        {
            // go through our list of nodes and see if the goal is in there
            LinkedListNode<Node> node = m_Map.nodes.First;
            while (node != null)
            {
                // if it is
                if (node.Value.transform.data.isGoal)
                {
                    // go to it
                    //MoveToPriorPosition(ref aMoves);
                    m_IsMoving = true;
                    m_IsGoingToGoal = true;
                    break;
                }
                node = node.Next;
            }
        }

        // do turn
        if (m_IsMoving)
        {
            Resets();

            //if (!m_IsGoingToGoal)
            //{
            //    m_PriorMoves.Add(m_LastDirection);
            //}
            //else
            //{
            //    m_IsGoingToGoal = false;
            //}

            //if (test > 14)
            //{
            //    m_IsGoingToGoal = true;
            //    aMoves = new List<GameManager.Direction>();
            //    MoveToPriorPosition(ref aMoves);
            //    QueueAction(ref aMoves, ref aBombTime);
            //    test = 0;
            //}
            //else
            //{
            //    if ((UseSensor(m_CurrentDirection) & GameManager.SensorData.Clear) != 0)
            //        m_PriorMoves.Add(m_CurrentDirection);
            //}


            return CombatantAction.Move;
        }
        else if (m_IsBombing)
        {
            Resets();
            return CombatantAction.DropBomb;
        }
        else
        {
            Resets();
            return CombatantAction.Pass;
        }
    }

    private void MoveToPriorPosition(ref List<GameManager.Direction> aMoves)
    {
        Debug.Log("going back");
        foreach (GameManager.Direction d in m_PriorMoves)
        {
            switch (d)
            {
                case GameManager.Direction.Up:
                    aMoves.Add(GameManager.Direction.Down);
                    break;
                case GameManager.Direction.Down:
                    aMoves.Add(GameManager.Direction.Up);
                    break;
                case GameManager.Direction.Left:
                    aMoves.Add(GameManager.Direction.Right);
                    break;
                case GameManager.Direction.Right:
                    aMoves.Add(GameManager.Direction.Left);
                    break;
                case GameManager.Direction.Current:
                    aMoves.Add(GameManager.Direction.Current);
                    break;
            }
        }
        m_PriorMoves = new List<GameManager.Direction>();
    }

    void Resets()
    {
        m_IsActionQueueable = true;
        m_IsBombing = false;
        m_IsMoving = false;
    }

    void QueueAction(ref List<GameManager.Direction> aMoves, ref int aBombTime)
    {
        // grab the last nodes we checked out
        List<Node> nodes = new List<Node>();
        LinkedListNode<Node> currentLNode = m_Map.nodes.Last;
        int count = m_NumOfSquaresScanned;
        while (count > 1)
        {
            nodes.Add(currentLNode.Value);
            currentLNode = currentLNode.Previous;
            count--;
        }

        // are any of these nodes the diamond?
        foreach (Node node in nodes)
        {
            if (node.transform.data.isDiamond)
            {
                aMoves.Add(node.transform.data.foundFrom);
                m_CurrentDirection = node.transform.data.foundFrom;
                m_IsMoving = true;
                m_IsHoldingDiamond = true; // log that we grabbed the diamond
                return;
            }

            if(node.transform.data.isGoal && m_IsHoldingDiamond)
            {
                aMoves.Add(node.transform.data.foundFrom);
                m_CurrentDirection = node.transform.data.foundFrom;
                m_IsMoving = true;
                m_IsGoingToGoal = true; // log that we grabbed the diamond
                return;
            }
        }

        foreach (Node node in nodes)
        {
            // if there is an enemy or a bomb move away if it is safe to
            if (node.transform.data.isEnemy || node.transform.data.isBomb)
            {
                // tell the ai we are moving
                m_IsMoving = true;

                // dodge stuff
                switch (RandomMove())
                {
                    case GameManager.Direction.Up:
                        if ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Clear) != 0
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Enemy) == 0)
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Bomb) == 0))
                        {
                            aMoves.Add(GameManager.Direction.Down);
                            m_CurrentDirection = GameManager.Direction.Down;
                            return;
                        }
                        break;
                    case GameManager.Direction.Down:
                        if ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Clear) != 0
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Enemy) == 0)
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Bomb) == 0))
                        {
                            aMoves.Add(GameManager.Direction.Up);
                            m_CurrentDirection = GameManager.Direction.Up;
                            return;
                        }
                        break;
                    case GameManager.Direction.Left:
                        if ((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.Clear) != 0
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Enemy) == 0)
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Bomb) == 0))
                        {
                            aMoves.Add(GameManager.Direction.Right);
                            m_CurrentDirection = GameManager.Direction.Right;
                            return;
                        }
                        break;
                    case GameManager.Direction.Right:
                        if ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Clear) != 0
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Enemy) == 0)
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Bomb) == 0))
                        {
                            aMoves.Add(GameManager.Direction.Left);
                            m_CurrentDirection = GameManager.Direction.Left;
                            return;
                        }
                        break;
                    case GameManager.Direction.Current:
                        if ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Clear) != 0
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Enemy) == 0)
                            && ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Bomb) == 0))
                        {
                            aMoves.Add(GameManager.Direction.Down);
                            m_CurrentDirection = GameManager.Direction.Down;
                            return;
                        }
                        break;
                }
            }
        }

        // if there wasn't any enemy ai or bombs near us continue on our wonderfully straight path
        if ((UseSensor(m_LastDirection) & GameManager.SensorData.Clear) != 0)
        {
            aMoves.Add(m_LastDirection);
        }
        else
        {
            aMoves.Add(RandomMove());
        }

        // tell the ai to move this turn
        m_IsMoving = true;
    }

    GameManager.Direction RandomMove()
    {
        m_CurrentDirection = (GameManager.Direction)UnityEngine.Random.Range(0, 4);
        if (m_CurrentDirection == m_LastDirection)
        {
            RandomMove();
        }
        // make sure it isn't a wall
        switch (m_CurrentDirection)
        {
            case GameManager.Direction.Up:
                if ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Clear) != 0)
                {
                    return m_CurrentDirection;
                }
                else
                {
                    RandomMove();
                }
                break;
            case GameManager.Direction.Down:
                if ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Clear) != 0)
                {
                    return m_CurrentDirection;
                }
                else
                {
                    RandomMove();
                }
                break;
            case GameManager.Direction.Left:
                if ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Clear) != 0)
                {
                    return m_CurrentDirection;
                }
                else
                {
                    RandomMove();
                }
                break;
            case GameManager.Direction.Right:
                if ((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.Clear) != 0)
                {
                    return m_CurrentDirection;
                }
                else
                {
                    RandomMove();
                }
                break;
            default:
                RandomMove();
                break;
        }

        // to be safe if recursion fails or switch gets broken out for some reason
        return GameManager.Direction.Current;
    }

    void RunCheck(GameManager.Direction aDirection)
    {
        // log the direction
        m_CurrentDirection = aDirection;

        // make sure the ai doesn't hit walls or falls off the map
        if ((UseSensor(aDirection) & (GameManager.SensorData.Wall | GameManager.SensorData.OffGrid)) != 0)
        {
            // TODO: maybe store these to build an actual x,y grid idk yet
            return;
        }

        // check to see if the space we are checking is clear
        if ((UseSensor(aDirection) & GameManager.SensorData.Clear) != 0)
        {
            Node node = new Node();

            // check for bombs
            if ((UseSensor(aDirection) & GameManager.SensorData.Bomb) != 0)
            {
                node.transform.data.isBomb = true;
                node.transform.data.turnsSinceEnemyBomb++;
            }
            // check for enemy ai
            if ((UseSensor(aDirection) & GameManager.SensorData.Enemy) != 0)
            {
                node.transform.data.isEnemy = true;
            }
            // check for a beautiful diamond
            if ((UseSensor(aDirection) & GameManager.SensorData.Diamond) != 0)
            {
                node.transform.data.isDiamond = true;
            }
            // check for goal
            if ((UseSensor(aDirection) & GameManager.SensorData.Goal) != 0)
            {
                node.transform.data.isGoal = true;
            }

            node.transform.data.foundFrom = m_CurrentDirection;
            m_Map.AddNode(node);
            m_NumOfSquaresScanned++;
        }
    }
}
