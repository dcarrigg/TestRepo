using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    // How many turns are left before this bomb explodes
    public int timeRemaining = 1;
    [SerializeField]
    Text timeRemainingText;
    Color color = Color.white;
    [SerializeField]
    SpriteRenderer sprite;
    float t = 0;

    public void SetTimeRemaining(int aTime)
    {
        timeRemaining = aTime;
        timeRemainingText.text = timeRemaining.ToString();
    }

    // Reduce the timer by one
    public void ReduceTime()
    {
        timeRemaining--;
        timeRemainingText.text = timeRemaining.ToString();
    }

    void Update()
    {
        float multiplier = 1;

        // Pulse the color based on how much time is remaining
        if (timeRemaining <= 1)
        {
            multiplier = 10;
        }
        else if (timeRemaining <= 2)
        {
            multiplier = 4;
        }

        t += Time.deltaTime * multiplier;
        color.g = (Mathf.Sin(t) + 1) / 2;
        color.b = (Mathf.Sin(t) + 1) / 2;

        sprite.color = color;
    }

    // Check if this bomb should explode this turn
    public bool ShouldExplode()
    {
        return timeRemaining <= 0;
    }
}
