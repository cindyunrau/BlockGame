using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mino : MonoBehaviour
{
    // The number of turns this mino has been on the board
    public int currentAge = 0;
    // The number of turns before this mino starts decaying
    public int primeAge = 3;



    public bool inShadow;
    public void IncreaseAge()
    {
        currentAge += 1;
        //SetColour(new Color(1f, 0f, 0f, (currentAge+1/primeAge)));
    }
    public void SetInShadow(bool value)
    {
        inShadow = value;
        if (inShadow)
        {
            SetColour(new Color(1f, 1f, 1f, 0.7f));
        }
        else
        {
            SetColour(new Color(1f, 1f, 1f, 1.0f));
        }
    }
    public void SetColour(Color colour)
    {
        gameObject.GetComponent<SpriteRenderer>().color = colour; 
    }

    public void MultiplyColour(Color colour)
    {
        gameObject.GetComponent<SpriteRenderer>().color *= colour;
    }

    public override string ToString()
    {
        return $"Mino: currentAge={currentAge}";
    }
}
