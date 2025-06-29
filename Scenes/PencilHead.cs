using Godot;
using System;

public partial class PencilHead : Node2D
{
    public Vector2 Target;
    public Player TargetPlayer;
    public float Speed = 800f;

    public Player UltOwner;
    public bool IsLast = false;

    public override void _Process(double delta)
    {
        Position = Position.MoveToward(Target, Speed * (float)delta);
        if (Position.DistanceTo(Target) < 10f)
        {
            if (IsLast)
            {
                UltOwner?.NotifyUltFinalHit();
            }
            QueueFree();
        }
    }
}
