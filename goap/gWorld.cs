using UnityEngine;

public sealed class gWorld  {
    private static readonly gWorld instance = new gWorld();
    private static worldStates world;

    static gWorld()
    {
        world = new worldStates();
    }

    private gWorld()
    { }

    public static gWorld Instance
    {
        get {return instance;}
    }
    public worldStates GetWorld()
    {
        return world;
    }
}