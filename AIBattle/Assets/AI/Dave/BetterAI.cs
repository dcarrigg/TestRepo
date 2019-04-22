using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestAI", menuName = "AI/Dave's AI")]
public class BetterAI : BaseAI
{
    // Data about the world
    public class WorldData
    {
        public GameManager.SensorData state;
        public int bombTime;
        public WorldData up;
        public WorldData down;
        public WorldData left;
        public WorldData right;
    }
    public WorldData world = new WorldData();
    // Note: this WorldData should always be our current position

    // How far away are we from the edges of the world?
    int? rightEdgeOffset = null;
    int? leftEdgeOffset = null;
    int? topEdgeOffset = null;
    int? bottomEdgeOffset = null;

    enum AIState
    {
        Search, // Search the map
        GetDiamond, // Get the diamond
        GoToGoal, // Go to the goal asap
        HuntForTarget, // Hunt for a target
        DropBomb, // Drop a bomb
        BombSweepPlaceBomb, // Sweep the entire map with bombs
        BombSweepMove, // Sweep the entire map with bombs
    }
    AIState myState = AIState.Search;

    // This function uses a BFS to find the furthest position away from where we currently are
    // It also loads up the movement array with how we get to that position
    WorldData FindBestLeaf(ref List<GameManager.Direction> aMoves)
    {
        Queue<WorldData> frontier = new Queue<WorldData>();
        frontier.Enqueue(world);
        Dictionary<WorldData, GameManager.Direction> cameFrom = new Dictionary<WorldData, GameManager.Direction>();
        cameFrom[world] = GameManager.Direction.Current;

        WorldData current = null;
        WorldData bestNode = null;
        int mostNulls = 0;
        bool foundLeaf = false;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current.up != null && !cameFrom.ContainsKey(current.up) && (current.up.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.up);
                cameFrom[current.up] = GameManager.Direction.Up;
            }
            if (current.down != null && !cameFrom.ContainsKey(current.down) && (current.down.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.down);
                cameFrom[current.down] = GameManager.Direction.Down;
            }
            if (current.left != null && !cameFrom.ContainsKey(current.left) && (current.left.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.left);
                cameFrom[current.left] = GameManager.Direction.Left;
            }
            if (current.right != null && !cameFrom.ContainsKey(current.right) && (current.right.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.right);
                cameFrom[current.right] = GameManager.Direction.Right;
            }

            // We want to find a "leaf" node, which is a node that we know is Clear, but has some null data inside of it
            // Find the first leaf node
            if ((current.state & GameManager.SensorData.Clear) != 0)
            {
                int numberOfNulls = 0;
                if (current.left == null) numberOfNulls++;
                if (current.right == null) numberOfNulls++;
                if (current.up == null) numberOfNulls++;
                if (current.down == null) numberOfNulls++;

                if (numberOfNulls > mostNulls)
                {
                    mostNulls = numberOfNulls;
                    bestNode = current;
                    foundLeaf = true;
                }
            }
        }
        // If we can't find any leaf nodes
        if (!foundLeaf)
        {
            return null;
        }
        //Debug.Log("Found node with " + mostNulls);
        // Create the path to current (the last searched node)
        WorldData targetPostion = bestNode;
        while (bestNode != world)
        {
            aMoves.Insert(0, cameFrom[bestNode]);

            if (cameFrom[bestNode] == GameManager.Direction.Up)
                bestNode = bestNode.down;
            else if (cameFrom[bestNode] == GameManager.Direction.Down)
                bestNode = bestNode.up;
            else if (cameFrom[bestNode] == GameManager.Direction.Left)
                bestNode = bestNode.right;
            else if (cameFrom[bestNode] == GameManager.Direction.Right)
                bestNode = bestNode.left;
        }
        return targetPostion;
    }

    class SearchData
    {
        public SearchData(WorldData d, int x, int y) { worldData = d; xoffset = x; yoffset = y; }
        public WorldData worldData;
        public int xoffset;
        public int yoffset;
    }
    // Find the worlddata with the given offset from the current world position
    // Used to connect new nodes into the world
    WorldData FindNode(int aXOffset, int aYOffset)
    {
        Queue<SearchData> frontier = new Queue<SearchData>();
        SearchData d = new SearchData(world, 0, 0);
        frontier.Enqueue(d);
        List<WorldData> visited = new List<WorldData>();
        visited.Add(world);

        SearchData current = null;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current.worldData.up != null && !visited.Contains(current.worldData.up) && (current.worldData.up.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(new SearchData(current.worldData.up, current.xoffset, current.yoffset + 1));
                visited.Add(current.worldData.up);
            }
            if (current.worldData.down != null && !visited.Contains(current.worldData.down) && (current.worldData.down.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(new SearchData(current.worldData.down, current.xoffset, current.yoffset - 1));
                visited.Add(current.worldData.down);
            }
            if (current.worldData.left != null && !visited.Contains(current.worldData.left) && (current.worldData.left.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(new SearchData(current.worldData.left, current.xoffset - 1, current.yoffset));
                visited.Add(current.worldData.left);
            }
            if (current.worldData.right != null && !visited.Contains(current.worldData.right) && (current.worldData.right.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(new SearchData(current.worldData.right, current.xoffset + 1, current.yoffset));
                visited.Add(current.worldData.right);
            }

            // Find the node with the given offset from the initial world node
            if (current.xoffset == aXOffset && current.yoffset == aYOffset)
            {
                return current.worldData;
            }
        }

        // Check if this is an edge of the world
        if (aXOffset == rightEdgeOffset || aXOffset == leftEdgeOffset || aYOffset == topEdgeOffset || aYOffset == bottomEdgeOffset)
        {
            WorldData newEdge = new WorldData();
            newEdge.state = GameManager.SensorData.OffGrid;
            //Debug.Log("Adding assumed edge at " + aXOffset + " " + aYOffset + " " + rightEdgeOffset + " " + leftEdgeOffset + " " + topEdgeOffset + " " + bottomEdgeOffset);
            return newEdge;
        }

        return null;
    }

    // Search through all nodes, add new edge data nodes if they don't exist
    void FillEdgeData()
    {
        Queue<SearchData> frontier = new Queue<SearchData>();
        SearchData d = new SearchData(world, 0, 0);
        frontier.Enqueue(d);
        List<WorldData> visited = new List<WorldData>();
        visited.Add(world);

        SearchData current = null;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current.worldData.up != null && !visited.Contains(current.worldData.up) && (current.worldData.up.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(new SearchData(current.worldData.up, current.xoffset, current.yoffset + 1));
                visited.Add(current.worldData.up);
            }
            if (current.worldData.down != null && !visited.Contains(current.worldData.down) && (current.worldData.down.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(new SearchData(current.worldData.down, current.xoffset, current.yoffset - 1));
                visited.Add(current.worldData.down);
            }
            if (current.worldData.left != null && !visited.Contains(current.worldData.left) && (current.worldData.left.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(new SearchData(current.worldData.left, current.xoffset - 1, current.yoffset));
                visited.Add(current.worldData.left);
            }
            if (current.worldData.right != null && !visited.Contains(current.worldData.right) && (current.worldData.right.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(new SearchData(current.worldData.right, current.xoffset + 1, current.yoffset));
                visited.Add(current.worldData.right);
            }

            // If this node is adjacent to an edge, make sure it has it's offgrid edge nodes
            if (current.xoffset == rightEdgeOffset - 1 && current.worldData.right == null)
            {
                current.worldData.right = new WorldData();
                current.worldData.right.state = GameManager.SensorData.OffGrid;
            }
            if (current.xoffset == leftEdgeOffset + 1 && current.worldData.left == null)
            {
                current.worldData.left = new WorldData();
                current.worldData.left.state = GameManager.SensorData.OffGrid;
            }
            if (current.yoffset == topEdgeOffset - 1 && current.worldData.up == null)
            {
                current.worldData.up = new WorldData();
                current.worldData.up.state = GameManager.SensorData.OffGrid;
            }
            if (current.yoffset == bottomEdgeOffset + 1 && current.worldData.down == null)
            {
                current.worldData.down = new WorldData();
                current.worldData.down.state = GameManager.SensorData.OffGrid;
            }
        }
    }

    // Find the diamond
    WorldData FindDiamond(ref List<GameManager.Direction> aMoves)
    {
        Queue<WorldData> frontier = new Queue<WorldData>();
        frontier.Enqueue(world);
        Dictionary<WorldData, GameManager.Direction> cameFrom = new Dictionary<WorldData, GameManager.Direction>();
        cameFrom[world] = GameManager.Direction.Current;

        WorldData current = null;
        WorldData bestNode = null;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current.up != null && !cameFrom.ContainsKey(current.up) && (current.up.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.up);
                cameFrom[current.up] = GameManager.Direction.Up;
            }
            if (current.down != null && !cameFrom.ContainsKey(current.down) && (current.down.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.down);
                cameFrom[current.down] = GameManager.Direction.Down;
            }
            if (current.left != null && !cameFrom.ContainsKey(current.left) && (current.left.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.left);
                cameFrom[current.left] = GameManager.Direction.Left;
            }
            if (current.right != null && !cameFrom.ContainsKey(current.right) && (current.right.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.right);
                cameFrom[current.right] = GameManager.Direction.Right;
            }

            // We want to find a "leaf" node, which is a node that we know is Clear, but has some null data inside of it
            // Find the first leaf node
            if ((current.state & GameManager.SensorData.Diamond) != 0)
            {
                bestNode = current;
                break;
            }
        }
        if (bestNode == null)
        {
            return null;
        }
        
        // Create the path to bestNode (the last searched node)
        WorldData targetPostion = bestNode;
        while (bestNode != world)
        {
            aMoves.Insert(0, cameFrom[bestNode]);

            if (cameFrom[bestNode] == GameManager.Direction.Up)
                bestNode = bestNode.down;
            else if (cameFrom[bestNode] == GameManager.Direction.Down)
                bestNode = bestNode.up;
            else if (cameFrom[bestNode] == GameManager.Direction.Left)
                bestNode = bestNode.right;
            else if (cameFrom[bestNode] == GameManager.Direction.Right)
                bestNode = bestNode.left;
        }
        return targetPostion;
    }

    // Find the goal
    WorldData FindGoal(ref List<GameManager.Direction> aMoves)
    {
        Queue<WorldData> frontier = new Queue<WorldData>();
        frontier.Enqueue(world);
        Dictionary<WorldData, GameManager.Direction> cameFrom = new Dictionary<WorldData, GameManager.Direction>();
        cameFrom[world] = GameManager.Direction.Current;

        WorldData current = null;
        WorldData bestNode = null;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current.up != null && !cameFrom.ContainsKey(current.up) && (current.up.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.up);
                cameFrom[current.up] = GameManager.Direction.Up;
            }
            if (current.down != null && !cameFrom.ContainsKey(current.down) && (current.down.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.down);
                cameFrom[current.down] = GameManager.Direction.Down;
            }
            if (current.left != null && !cameFrom.ContainsKey(current.left) && (current.left.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.left);
                cameFrom[current.left] = GameManager.Direction.Left;
            }
            if (current.right != null && !cameFrom.ContainsKey(current.right) && (current.right.state & GameManager.SensorData.Clear) != 0)
            {
                frontier.Enqueue(current.right);
                cameFrom[current.right] = GameManager.Direction.Right;
            }

            // We want to find a "leaf" node, which is a node that we know is Clear, but has some null data inside of it
            // Find the first leaf node
            if ((current.state & GameManager.SensorData.Goal) != 0)
            {
                bestNode = current;
                break;
            }
        }
        if (bestNode == null)
        {
            return null;
        }

        // Create the path to bestNode (the last searched node)
        WorldData targetPostion = bestNode;
        while (bestNode != world)
        {
            aMoves.Insert(0, cameFrom[bestNode]);

            if (cameFrom[bestNode] == GameManager.Direction.Up)
                bestNode = bestNode.down;
            else if (cameFrom[bestNode] == GameManager.Direction.Down)
                bestNode = bestNode.up;
            else if (cameFrom[bestNode] == GameManager.Direction.Left)
                bestNode = bestNode.right;
            else if (cameFrom[bestNode] == GameManager.Direction.Right)
                bestNode = bestNode.left;
        }
        return targetPostion;
    }

    public override CombatantAction GetAction(ref List<GameManager.Direction> aMoves, ref int aBombTime)
    {
        // Initialize world data for anything we don't currently have
        if (world.up == null)
        {
            world.up = new WorldData();
            world.up.down = world;
            world.up.right = FindNode(1, 1);
            if (world.up.right != null) world.up.right.left = world.up;
            world.up.left = FindNode(-1, 1);
            if (world.up.left != null) world.up.left.right = world.up;
            world.up.up = FindNode(0, 2);
            if (world.up.up != null) world.up.up.down = world.up;
        }
        if (world.down == null)
        {
            world.down = new WorldData();
            world.down.up = world;
            world.down.right = FindNode(1, -1);
            if (world.down.right != null) world.down.right.left = world.down;
            world.down.left = FindNode(-1, -1);
            if (world.down.left != null) world.down.left.right = world.down;
            world.down.down = FindNode(0, -2);
            if (world.down.down != null) world.down.down.up = world.down;
        }
        if (world.left == null)
        {
            world.left = new WorldData();
            world.left.right = world;
            world.left.left = FindNode(-2, 0);
            if (world.left.left != null) world.left.left.right = world.left;
            world.left.up = FindNode(-1, 1);
            if (world.left.up != null) world.left.up.down = world.left;
            world.left.down = FindNode(-1, -1);
            if (world.left.down != null) world.left.down.up = world.left;
        }
        if (world.right == null)
        {
            world.right = new WorldData();
            world.right.left = world;
            world.right.right = FindNode(2, 0);
            if (world.right.right != null) world.right.right.left = world.right;
            world.right.down = FindNode(1, -1);
            if (world.right.down != null) world.right.down.up = world.right;
            world.right.up = FindNode(1, 1);
            if (world.right.up != null) world.right.up.down = world.right;
        }

        // Update our scans
        world.state = UseSensor(GameManager.Direction.Current);
        world.up.state = UseSensor(GameManager.Direction.Up);
        world.down.state = UseSensor(GameManager.Direction.Down);
        world.left.state = UseSensor(GameManager.Direction.Left);
        world.right.state = UseSensor(GameManager.Direction.Right);

        // Do any of the scans show us where the edge of the world is?
        if ((world.up.state & GameManager.SensorData.OffGrid) != 0)
        {
            //Debug.Log("Found top edge at 1");
            topEdgeOffset = 1;
            FillEdgeData();
        }
        if ((world.down.state & GameManager.SensorData.OffGrid) != 0)
        {
            //Debug.Log("Found bottom edge at -1");
            bottomEdgeOffset = -1;
            FillEdgeData();
        }
        if ((world.left.state & GameManager.SensorData.OffGrid) != 0)
        {
            //Debug.Log("Found left edge at -1");
            leftEdgeOffset = -1;
            FillEdgeData();
        }
        if ((world.right.state & GameManager.SensorData.OffGrid) != 0)
        {
            //Debug.Log("Found right edge at 1");
            rightEdgeOffset = 1;
            FillEdgeData();
        }

        // Check if we've found the diamond, and no one is carrying it
        if ((world.up.state & GameManager.SensorData.Diamond) != 0 && (world.up.state & GameManager.SensorData.Enemy) == 0)
        {
            myState = AIState.GetDiamond;
        }
        if ((world.down.state & GameManager.SensorData.Diamond) != 0 && (world.up.state & GameManager.SensorData.Enemy) == 0)
        {
            myState = AIState.GetDiamond;
        }
        if ((world.left.state & GameManager.SensorData.Diamond) != 0 && (world.up.state & GameManager.SensorData.Enemy) == 0)
        {
            myState = AIState.GetDiamond;
        }
        if ((world.right.state & GameManager.SensorData.Diamond) != 0 && (world.up.state & GameManager.SensorData.Enemy) == 0)
        {
            myState = AIState.GetDiamond;
        }

        
        // What action are we taking this turn?
        CombatantAction action = CombatantAction.Pass;
        
        // Which state are we in?
        if (myState == AIState.GoToGoal)
        {
            WorldData newPosition = FindGoal(ref aMoves);
            if (newPosition != null)
            {
                world = newPosition;
                return CombatantAction.Move;
            }
            myState = AIState.Search;
        }

        if (myState == AIState.Search)
        {
            // To search, first find the best leaf node and move there
            WorldData newPosition = FindBestLeaf(ref aMoves);
            if (newPosition != null)
            {
                world = newPosition;
                action = CombatantAction.Move;
            }

            if (newPosition == null)
            {
                myState = AIState.DropBomb;
            }
        }
        if (myState == AIState.DropBomb)
        {
            aBombTime = 2;
            myState = AIState.HuntForTarget;
            return CombatantAction.DropBomb;
        }
        if (myState == AIState.HuntForTarget)
        {
            WorldData newPosition = null; //FindRandom(ref aMoves);
            if (newPosition != null)
            {
                myState = AIState.DropBomb;
                world = newPosition;
                return CombatantAction.Move;
            }
        }
        if (myState == AIState.GetDiamond)
        {
            WorldData newPosition = FindDiamond(ref aMoves);
            if (newPosition != null)
            {
                myState = AIState.GoToGoal;
                world = newPosition;
                return CombatantAction.Move;
            }
        }
        
        if (myState == AIState.BombSweepPlaceBomb)
        {
            
        }
        if (myState == AIState.BombSweepMove)
        {
            
        }

        // Logic depending on which action we've decided to take
        if (action == CombatantAction.Move)
        {
            // Update the edge offsets based on our movement
            foreach (GameManager.Direction dir in aMoves)
            {
                if (dir == GameManager.Direction.Up)
                {
                    topEdgeOffset -= 1;
                    bottomEdgeOffset -= 1;
                }
                else if (dir == GameManager.Direction.Down)
                {
                    bottomEdgeOffset += 1;
                    topEdgeOffset += 1;
                }
                else if (dir == GameManager.Direction.Left)
                {
                    leftEdgeOffset += 1;
                    rightEdgeOffset += 1;
                }
                else if (dir == GameManager.Direction.Right)
                {
                    leftEdgeOffset -= 1;
                    rightEdgeOffset -= 1;
                }
            }
        }
        else if (action == CombatantAction.DropBomb)
        {
            world.state |= GameManager.SensorData.Bomb;
            world.bombTime = aBombTime;
        }
        //CountdownBombTimes();
        return action;
    }


    public void DrawData(Vector3 aPosition, WorldData aData, ref List<WorldData> visited)
    {
        if (aData == null || visited.Contains(aData)) return;
        visited.Add(aData);

        if (aData == world)
        {
            Gizmos.color = Color.red;
            // Top
            if (topEdgeOffset != null)
                Gizmos.DrawCube(aPosition + Vector3.up * (int)topEdgeOffset, Vector3.one);

            Gizmos.color = Color.blue;

            if (bottomEdgeOffset != null)
                Gizmos.DrawCube(aPosition + Vector3.up * (int)bottomEdgeOffset, Vector3.one);

            Gizmos.color = Color.magenta;

            if (leftEdgeOffset != null)
                Gizmos.DrawCube(aPosition + Vector3.right * (int)leftEdgeOffset, Vector3.one);

            Gizmos.color = Color.yellow;

            if (rightEdgeOffset != null)
                Gizmos.DrawCube(aPosition + Vector3.right * (int)rightEdgeOffset, Vector3.one);
        }

        if ((aData.state & GameManager.SensorData.OffGrid) != 0)
        {
            Gizmos.color = Color.blue;
        }
        else if ((aData.state & GameManager.SensorData.Goal) != 0)
        {
            Gizmos.color = Color.yellow;
        }
        else if ((aData.state & GameManager.SensorData.Clear) != 0)
        {
            Gizmos.color = Color.green;
        }
        else if ((aData.state & GameManager.SensorData.Wall) != 0)
        {
            Gizmos.color = Color.white;
        }
        else
        {
            //Debug.Log(aData.state);
            Gizmos.color = Color.magenta;
        }
        Gizmos.DrawWireSphere(aPosition, 0.5f);

        DrawData(aPosition + Vector3.up, aData.up, ref visited);
        DrawData(aPosition + Vector3.down, aData.down, ref visited);
        DrawData(aPosition + Vector3.left, aData.left, ref visited);
        DrawData(aPosition + Vector3.right, aData.right, ref visited);
    }
}
