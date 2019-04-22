using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAI : ScriptableObject {

    [SerializeField]
    public string AIName;

    public enum CombatantAction
    {
        Move,
        DropBomb,
        Pass
    }

    // Get the action the combatant wants to take
    // If you are returning CombatantAction.Move:
    //      - You must specify how you would like to move by adding data to the aMoves list
    //      - You will move along the path until you bump into something or reach the end
    // If you are returning CombatantAction.DropBomb:
    //      - You must specify what the timer on the bomb should be set by setting the aBombTime argument
    public abstract CombatantAction GetAction(ref List<GameManager.Direction> aMoves, ref int aBombTime);

    protected GameManager.SensorData UseSensor(GameManager.Direction aDirection)
    {
        return GameManager.instance.UseSensor(aDirection);
    }

    // Get the health of this player
    protected int GetHealth()
    {
        return GameManager.instance.GetHealth();
    }

    // Get the total number of players in this game
    protected int GetTotalPlayerCount()
    {
        return GameManager.instance.GetTotalPlayerCount();
    }
}
