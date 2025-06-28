using Godot;
using System;

public partial class PencilHead : Node2D
{
    public Vector2 Target;
    public Player TargetPlayer;
    public float Speed = 800f;

    public override void _Process(double delta)
    {
        Position = Position.MoveToward(Target, Speed * (float)delta);

        if (Position.DistanceTo(Target) < 10f)
        {
            if (TargetPlayer.IsJumping)
            {
                GD.Print($"{TargetPlayer.Name} 跳跃中，免疫攻击！");
                // 无伤害
            }
            else if (TargetPlayer.isDodging)
            {
                GD.Print($"{TargetPlayer.Name} 躲避中，受到 200 伤害！");
                TargetPlayer.TakeDamage(200);
            }
            else
            {
                GD.Print($"{TargetPlayer.Name} 被直接命中，受到 400 伤害！");
                TargetPlayer.TakeDamage(400);
            }

            QueueFree();
        }
    }
}
