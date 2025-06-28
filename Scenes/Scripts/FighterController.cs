using Godot;
using System;

public abstract partial class FighterController : Node
{
    protected Player player;

    public void Init(Player p)
    {
        player = p;
    }

    public abstract void Tick(double delta);
}
