using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mino : MonoBehaviour
{
    // The number of turns this mino has been on the board
    public int currentAge = 0;
    // The number of turns before this mino starts decaying
    public int cookedAge = 3;
    public int burntAge = 6;

    public string status = "raw";

    public bool inShadow;
    public string foodName;

    //private Renderer renderer;
    //private Shader glow;
    //private Shader spriteLit;

    public Material defaultRef;
    public Material glowRef;


    void Start()
    {
        //renderer = GetComponent<Renderer>();
        //glow = Shader.Find("Assets/Shaders/Food.mat");
        //spriteLit = Shader.Find("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Lit-Default.mat");
    }

    public void Init(string food, Sprite spr)
    {
        foodName = food;
        SetSprite(spr);
    }

    public void SetSprite(Sprite spr)
    {
        GetComponent<SpriteRenderer>().sprite = spr;
    }

    public void SetGlow(bool value)
    {
        Debug.Log($"Set glow {value}");
        if (value){
            GetComponent<Renderer>().material = glowRef;
        }
        else
        {
            GetComponent<Renderer>().material = defaultRef;
        }
    }

    public int TurnsUntilCooked()
    {
        return cookedAge - currentAge;
    }

    public bool IsCookedNextTurn()
    {
        if (TurnsUntilCooked() <= 1) return true;
        return false;
    }

    public int TurnsUntilBurnt()
    {
        return burntAge - currentAge;
    }

    public bool IsBurntNextTurn()
    {
        if (TurnsUntilBurnt () == 1) return true;
        return false;
    }

    // Returns true if increasing the age passes a threshold
    public bool IncreaseAge(int inc)
    {
        currentAge += inc;

        if ((currentAge - inc < cookedAge && currentAge >= cookedAge) || (currentAge - inc < burntAge && currentAge >= burntAge))
        {
            if (currentAge >= burntAge) status = "burnt";
            else if (currentAge >= cookedAge) status = "cooked";
            return true;
        }
        return false;
    }


    public bool IsCooked()
    {
        return (currentAge >= cookedAge);
    }

    //public void setCooked()
    //{
    //    sr.sprite = rm.GetCookedSprite(foodName);
    //}

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
