using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Hadran", menuName = "Hadran")]
public class Hadran : BaseAI
{

    // this node class keeps track of connected nodes as well as the direction they came in. It also keeps track of their x and y values.
    public class Node
    {
        public Dictionary<GameManager.Direction, Node> connectedNodes = new Dictionary<GameManager.Direction, Node>();

        public GameManager.SensorData myTile = GameManager.SensorData.Clear;

        public int x = 0;
        public int y = 0;

        public bool explored = false;
    }
    List<Node> corners = new List<Node>();

    Node currentNode = new Node();

    public bool start = true;

    public override CombatantAction GetAction(ref List<GameManager.Direction> aMoves, ref int aBombTime)
    {

        currentNode.explored = true;
        Debug.Log(currentNode.explored);
        // this initialises all the nodes and the diractions on those nodes.
       // currentNode.x = 0;
        //currentNode.y = 0;
        currentNode.myTile = UseSensor(GameManager.Direction.Current);

        if (start)
        {
            Debug.Log("In the start");
            currentNode.connectedNodes.Add(GameManager.Direction.Up, NewNode());
            currentNode.connectedNodes.Add(GameManager.Direction.Down, NewNode());
            currentNode.connectedNodes.Add(GameManager.Direction.Left, NewNode());
            currentNode.connectedNodes.Add(GameManager.Direction.Right, NewNode());
            currentNode.connectedNodes.Add(GameManager.Direction.Current, NewNode());
            start = false;
        }
        if (currentNode.connectedNodes.ContainsKey(GameManager.Direction.Up))
        {
            currentNode.connectedNodes[GameManager.Direction.Up].myTile = UseSensor(GameManager.Direction.Up);
        }
        if (currentNode.connectedNodes.ContainsKey(GameManager.Direction.Down))
        {
            currentNode.connectedNodes[GameManager.Direction.Down].myTile = UseSensor(GameManager.Direction.Down);
        }
        if (currentNode.connectedNodes.ContainsKey(GameManager.Direction.Left))
        {
            currentNode.connectedNodes[GameManager.Direction.Left].myTile = UseSensor(GameManager.Direction.Left);
        }
        if (currentNode.connectedNodes.ContainsKey(GameManager.Direction.Right))
        {
            currentNode.connectedNodes[GameManager.Direction.Right].myTile = UseSensor(GameManager.Direction.Right);
        }


        // this adds nodes if the direction index in the connected nodes of the current node is null.
        if (currentNode.connectedNodes.ContainsKey(GameManager.Direction.Up))
            {
            Debug.Log("Making new node!");
            if (FindNode(currentNode.x, currentNode.y + 1) == null)
            {
                currentNode.connectedNodes.Add(GameManager.Direction.Up, NewNode());
            }
        }
        if (currentNode.connectedNodes.ContainsKey(GameManager.Direction.Down))
        {
            Debug.Log("Making new node!");
            if (FindNode(currentNode.x, currentNode.y - 1) == null)
            {
                currentNode.connectedNodes.Add(GameManager.Direction.Down, NewNode());
            }
        }
        if (currentNode.connectedNodes.ContainsKey(GameManager.Direction.Left))
        {
            Debug.Log("Making new node!");
            if (FindNode(currentNode.x - 1, currentNode.y) == null)
            {
                currentNode.connectedNodes.Add(GameManager.Direction.Left, NewNode());
            }
        }
        if (currentNode.connectedNodes.ContainsKey(GameManager.Direction.Right))
        {
            Debug.Log("Making new node!");
            if (FindNode(currentNode.x + 1, currentNode.y) == null)
            {
                currentNode.connectedNodes.Add(GameManager.Direction.Right, NewNode());
            }
        }

        if ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Clear) != 0)
        //(currentNode.connectedNodes[GameManager.Direction.Up].myTile == GameManager.SensorData.Clear & currentNode.connectedNodes[GameManager.Direction.Up].explored == false)
        {
            Debug.Log("Can Move up");
           // Debug.Log( " Up explored : "  + currentNode.connectedNodes[GameManager.Direction.Up].explored);
            aMoves.Add(GameManager.Direction.Up);
           // currentNode.connectedNodes[GameManager.Direction.Up].y = currentNode.y + 1;
           // currentNode = currentNode.connectedNodes[GameManager.Direction.Up];
        }
        else if ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Clear) != 0)
        //(currentNode.connectedNodes[GameManager.Direction.Down].myTile == GameManager.SensorData.Clear && currentNode.connectedNodes[GameManager.Direction.Down].explored == false)
        {
            Debug.Log("CanMove down");
          //  Debug.Log(" Down  explored : " + currentNode.connectedNodes[GameManager.Direction.Down].explored);
            aMoves.Add(GameManager.Direction.Down);
            //currentNode.connectedNodes[GameManager.Direction.Down].y = currentNode.y - 1;
            //currentNode = currentNode.connectedNodes[GameManager.Direction.Down];
        }
        else if ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Clear) != 0)
        //(currentNode.connectedNodes[GameManager.Direction.Left].myTile == GameManager.SensorData.Clear && currentNode.connectedNodes[GameManager.Direction.Left].explored == false)
        {
            Debug.Log("CanMove left");
           // Debug.Log(" Left explored : " + currentNode.connectedNodes[GameManager.Direction.Left].explored);
            aMoves.Add(GameManager.Direction.Left);
           // currentNode.connectedNodes[GameManager.Direction.Left].y = currentNode.x - 1;
           // currentNode = currentNode.connectedNodes[GameManager.Direction.Left];
        }
        else if ((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.Clear) != 0)
        //(currentNode.connectedNodes[GameManager.Direction.Right].myTile == GameManager.SensorData.Clear && currentNode.connectedNodes[GameManager.Direction.Right].explored == false)
        {
            Debug.Log("Can Move Right");
           // Debug.Log(" Right explored : " + currentNode.connectedNodes[GameManager.Direction.Right].explored);
            aMoves.Add(GameManager.Direction.Right);
           // currentNode.connectedNodes[GameManager.Direction.Right].y = currentNode.x + 1;
           // currentNode = currentNode.connectedNodes[GameManager.Direction.Right];
        }
        return CombatantAction.Move;
    }

    // this makes a new node
    public Node NewNode()
    {
        Node aNode = new Node();
        return aNode;
    }

    public Node FindNode(int aX, int aY)
    {
        Queue<Node> frontier = new Queue<Node>();
        frontier.Enqueue(currentNode);

        int counter = 0;
        Node current = new Node();
        List<Node> visited = new List<Node>();
        while (frontier.Count != 0)
        {
            counter++;
            visited.Add(current);

            current = frontier.Dequeue();

            foreach (KeyValuePair<GameManager.Direction, Node> next in current.connectedNodes)
            {
                if (!visited.Contains(next.Value))
                {
                    frontier.Enqueue(next.Value);
                }
                if (next.Value.x == aX && next.Value.y  == aY)
                {
                    return next.Value;
                }
                return current;
            }
        }

        return null;
    }
}
