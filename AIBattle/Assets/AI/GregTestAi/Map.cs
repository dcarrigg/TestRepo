/*
Greg St. Angelo IV
4.13.2017
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds nodes to make a map of where you have been.
/// </summary>
public class Map
{
    LinkedList<Node> m_Nodes = new LinkedList<Node>();
    public LinkedList<Node> nodes { get { return m_Nodes; } }

    public void AddNode(Node aNode)
    {
        if(m_Nodes.Count == 0)
        {
            m_Nodes.AddFirst(aNode);
            return;
        }

        m_Nodes.AddLast(aNode);
    }
}
