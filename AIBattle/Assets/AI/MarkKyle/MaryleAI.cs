using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaryleAI", menuName = "AI/Maryle AI")]
public class MaryleAI : BaseAI
{
    //public Vector2 position;
    bool diamond = false;

    class World
    {
        public GameManager.SensorData state;
        
        public World up;
        public World down;
        public World left;
        public World right;
        public Vector2 position;
    }
    Vector2 pos = new Vector2(0f, 0f);
    //Dictionary<Vector2, World> globe = new Dictionary<Vector2, World>();

    public override CombatantAction GetAction(ref List<GameManager.Direction> aMoves, ref int aBombTime)
    {

        //Picks up the diamond if it is adjacent
        if ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Diamond) != 0)
        {
            aMoves.Add(GameManager.Direction.Up);
            diamond = true;
            return CombatantAction.Move;
        }
        else if ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Diamond) != 0)
        {
            aMoves.Add(GameManager.Direction.Down);
            diamond = true;
            return CombatantAction.Move;
        }
        else if ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Diamond) != 0)
        {
            aMoves.Add(GameManager.Direction.Left);
            diamond = true;
            return CombatantAction.Move;
        }
        else if ((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.Diamond) != 0)
        {
            aMoves.Add(GameManager.Direction.Right);
            diamond = true;
            return CombatantAction.Move;
        }

        //Enters the goal if it is adjacent and the diamond is in hand
        if (((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Goal) != 0) && diamond)
        {
            aMoves.Add(GameManager.Direction.Up);
            return CombatantAction.Move;
        }
        else if (((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Goal) != 0) && diamond)
        {
            aMoves.Add(GameManager.Direction.Down);
            return CombatantAction.Move;
        }
        else if (((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Goal) != 0) && diamond)
        {
            aMoves.Add(GameManager.Direction.Left);
            return CombatantAction.Move;
        }
        else if (((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.Goal) != 0) && diamond)
        {
            aMoves.Add(GameManager.Direction.Right);
            return CombatantAction.Move;
        }

        if (((UseSensor(GameManager.Direction.Current) & GameManager.SensorData.Bomb) != 0) || ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Bomb) != 0) || ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Bomb) != 0) ||
            ((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.Bomb) != 0) || ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Bomb) != 0))
        {

            while (true)
            {

                int dir = Random.Range(0, 3);
                if (dir == 0 && !(((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Wall) != 0) || ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.OffGrid) != 0)))
                {
                    aMoves.Add(GameManager.Direction.Up);
                    return CombatantAction.Move;
                }
                else if (dir == 1 && !(((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Wall) != 0) || ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.OffGrid) != 0)))
                {
                    aMoves.Add(GameManager.Direction.Left);
                    return CombatantAction.Move;
                }
                else if (dir == 2 && !(((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Wall) != 0) || ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.OffGrid) != 0)))
                {
                    aMoves.Add(GameManager.Direction.Down);
                    return CombatantAction.Move;
                }
                else if (!(((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Wall) != 0) || ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.OffGrid) != 0)))
                {
                    aMoves.Add(GameManager.Direction.Right);
                    return CombatantAction.Move;
                }

            }

        }

        else
        {

            aBombTime = UnityEngine.Random.Range(6, 13);
            return CombatantAction.DropBomb;

        }

    }

}
/*
        // Check if there is a wall above us
        if ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Clear) != 0)
        {
            aMoves.Add(GameManager.Direction.Up);
            return CombatantAction.Move;
        }
        else
        {
            aMoves.Add(GameManager.Direction.Down);
            return CombatantAction.Move;
        }*/
/*
World currPos = new World();
World upPos = new World();


currPos.position = pos;
currPos.state = UseSensor(GameManager.Direction.Current);
globe.Add(pos, currPos);

upPos.state = UseSensor(GameManager.Direction.Up);
globe.Add((pos + new Vector2(0f ,1f)), upPos);

if((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Clear) != 0)
{
    aMoves.Add(GameManager.Direction.Up);
    currPos.position = upPos.position;
    upPos.state = UseSensor(GameManager.Direction.Up);
    globe.Add((pos + new Vector2(0f, 1f)), upPos);
    return CombatantAction.Move;
}
else
{
    return CombatantAction.Move;
}*/
