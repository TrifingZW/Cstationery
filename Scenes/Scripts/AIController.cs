using Godot;
using System;

public partial class AIController : FighterController
{
    private float timer = 0;

    public override void Tick(double delta)
    {
        timer += (float)delta;

        if (timer > 2f)
        {
            player.Attack();
            timer = 0;
        }

        // 简单逻辑：向左移动
        player.Move(new Vector2(-1, 0));
    }
}