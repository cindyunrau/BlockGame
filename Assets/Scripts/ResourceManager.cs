using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public Sprite rawSteakSprite;
    public Sprite cookedSteakSprite;
    public Sprite rawCornSprite;
    public Sprite cookedCornSprite;
    public Sprite rawOnionSprite;
    public Sprite cookedOnionSprite;
    public Sprite rawBaconSprite;
    public Sprite cookedBaconSprite;
    public Sprite rawSalmonSprite;
    public Sprite cookedSalmonSprite;
    public Sprite rawBeanSprite;
    public Sprite cookedBeanSprite;
    public Sprite rawGreenPepperSprite;
    public Sprite cookedGreenPepperSprite;
    public Sprite rawRedPepperSprite;
    public Sprite cookedRedPepperSprite;
    public Sprite rawYellowPepperSprite;
    public Sprite cookedYellowPepperSprite;

    public struct Tile
    {
        public Tile(string n, Sprite rSprite, Sprite cSprite, bool meat)
        {
            name = n; 
            rawSprite = rSprite;
            cookedSprite = cSprite;
            isMeat = meat;
        }

        public string name;
        public Sprite rawSprite;
        public Sprite cookedSprite;
        public bool isMeat;

        public override string ToString() => $"(Food: {name})";
    }

    public Tile steak, corn, onion, bacon, salmon, bean, greenPepper, yellowPepper, redPepper;
    public List<Tile> foods;

    public void Start()
    {
        steak = new Tile("steak", rawSteakSprite, cookedSteakSprite, true);
        corn = new Tile("corn", rawCornSprite, cookedCornSprite, false);
        onion = new Tile("onion", rawOnionSprite, cookedOnionSprite, false);
        bacon = new Tile("bacon", rawBaconSprite, cookedBaconSprite, true);
        salmon = new Tile("salmon", rawSalmonSprite, cookedSalmonSprite, true);
        bean = new Tile("bean", rawBeanSprite, cookedBeanSprite, false);
        greenPepper = new Tile("greenPepper", rawGreenPepperSprite, cookedGreenPepperSprite, false);
        yellowPepper = new Tile("yellowPepper", rawYellowPepperSprite, cookedYellowPepperSprite, false);
        redPepper = new Tile("redPepper", rawRedPepperSprite, cookedRedPepperSprite, false);

        foods = new List<Tile>();
        foods.Add(steak);
        foods.Add(corn);
        foods.Add(onion);
        foods.Add(bacon);
        foods.Add(salmon);
        foods.Add(bean);
        foods.Add(greenPepper);
        foods.Add(yellowPepper);
        foods.Add(redPepper);
    }

    public Tile GetTile(string name)
    {
        return foods.Find(x => x.name == name);
    }

    public Sprite GetDefaultSprite(string name)
    {
        return GetTile(name).rawSprite;
    }

    public Sprite GetCookedSprite(string name)
    {
        return GetTile(name).cookedSprite;
    }
}
