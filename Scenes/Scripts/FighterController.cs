using Godot;
using System;

public abstract partial class FighterController : Node
{
    protected Player player;

    protected bool inputEnabled = true;

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    public void Init(Player p)
    {
        player = p;
    }

    public abstract void Tick(double delta);
}
