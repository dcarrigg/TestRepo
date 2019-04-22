using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "POWERAI", menuName = "AI/Power AI")]
public class PowerAi : BaseAI
{
    List<GameManager.Direction> wayToGoal = new List<GameManager.Direction>();

    public override CombatantAction GetAction(ref List<GameManager.Direction> aMoves, ref int aBombTime)
    {   
        bool droppedBomb = true;
        int turnNumber = 0;
        bool blockUP = false;
        bool blockDOWN = false;
        bool blockRIGHT = false;
        bool blockLEFT = false;
        bool hasDimond = false;
        bool knowGoal = false;
        int lastdirection =100;
        int section3sides =0;
        turnNumber +=1;


        // check crossroads
        //if  sees wall or end
        if ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Wall) != 0 | ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.OffGrid) != 0))
        {
            blockUP = true;
        }
        if ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Wall) != 0 | ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.OffGrid) != 0))
        {
            blockDOWN = true;
        }
        if ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Wall) != 0 | ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.OffGrid) != 0))
        {
            blockLEFT = true;
        }
        if ((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.Wall) != 0 | ((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.OffGrid) != 0))
        {
            blockRIGHT = true;
        }


        // dimond pick up
        if (hasDimond == false)
        {
            if ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Diamond) != 0)
            {
                aMoves.Add(GameManager.Direction.Up);
                wayToGoal.Add(GameManager.Direction.Down);
                hasDimond = true;
                return CombatantAction.Move;
            }
            if ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Diamond) != 0)
            {
                hasDimond = true;
                aMoves.Add(GameManager.Direction.Down);
                wayToGoal.Add(GameManager.Direction.Up);
                return CombatantAction.Move;
            }
            if ((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.Diamond) != 0)
            {
                hasDimond = true;
                aMoves.Add(GameManager.Direction.Right);
                wayToGoal.Add(GameManager.Direction.Left);
                return CombatantAction.Move;
            }
            if ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Diamond) != 0)
            {
                hasDimond = true;
                aMoves.Add(GameManager.Direction.Left);
                wayToGoal.Add(GameManager.Direction.Right);
                return CombatantAction.Move;
            }
        }

       
        // Find Other Goal 
        if (knowGoal == false)
        {
                if ((UseSensor(GameManager.Direction.Up) & GameManager.SensorData.Goal) != 0)
                {
                // add goal to list
                knowGoal = true;

                if ((UseSensor(GameManager.Direction.Down) & GameManager.SensorData.Goal) != 0)
                {
                    // add goal to list
                    knowGoal = true;
                }
                if ((UseSensor(GameManager.Direction.Right) & GameManager.SensorData.Goal) != 0)
                {
                    // add goal to list
                    knowGoal = true;
                }
                if ((UseSensor(GameManager.Direction.Left) & GameManager.SensorData.Goal) != 0)
                {
                    // add goal to list
                    knowGoal = true;
                }
            }
        }
        //had the dimond and know the goal 
        if (hasDimond == true && knowGoal== true)
        {
            for (int i = 0; i < wayToGoal.Count; i++)
                aMoves.Add(wayToGoal[i]);
        }


        //Drop bombs at crossroads
        // drop bomb

   
        if (turnNumber % 5 == 0)
        {
            droppedBomb = false;
        }

        if (droppedBomb == false)
        {

            droppedBomb = true;

            if (blockDOWN == true && blockUP == true)
            {

                aBombTime = 5;
           
                return CombatantAction.DropBomb;

            }
            if (blockRIGHT == true && blockLEFT == true)
            {
                // get number of players drop bomb timer+1
                aBombTime = 5;
          
                return CombatantAction.DropBomb;
            }
            // Drop bomb
            aBombTime = 5;

            return CombatantAction.DropBomb;

           
            }
        

        if (blockDOWN)
            section3sides++;

        if (blockLEFT)
            section3sides++;

        if (blockRIGHT)
            section3sides++;

        if (blockUP)
            section3sides++;
        

        if(section3sides == 3)
        {
            // move to random node
        }

        // moveaway
        //ensure that he will not move back on himself
        int choice;
        bool willmove= false;
        do { 


        do
        {
            choice = UnityEngine.Random.Range(0, 4);
        } while (choice == lastdirection);


            if (blockUP == false && choice == 0 )
            {
                aMoves.Add(GameManager.Direction.Up);
                wayToGoal.Add(GameManager.Direction.Down);
                lastdirection = 0;
                willmove = true;
            }
            else if (blockRIGHT == false && choice == 1 )
            {
                aMoves.Add(GameManager.Direction.Right);
                wayToGoal.Add(GameManager.Direction.Left);
                lastdirection = 1;
                willmove = true;
            }
            else if (blockLEFT == false && choice == 2 )
            {
                aMoves.Add(GameManager.Direction.Left);
                wayToGoal.Add(GameManager.Direction.Right);
                lastdirection = 2;
                willmove = true;
            }
            else if (blockDOWN == false && choice == 3)
            {
                aMoves.Add(GameManager.Direction.Down);
                wayToGoal.Add(GameManager.Direction.Up);
                lastdirection = 3;
                willmove = true;
            }
            else
            {
                willmove = false; 

            }
        } while (willmove == false);
    

        section3sides = 0;
        return CombatantAction.Move;
    }


}
