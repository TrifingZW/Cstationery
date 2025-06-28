using Godot;
using System;

public partial class InputController : FighterController
{
    [Export] public string InputPrefix = "playerA_";

    public override void Tick(double delta)
    {
        if (Player.IsAnyUlting)
            return;
            
        Vector2 input = Vector2.Zero;

        if (Input.IsActionPressed(InputPrefix + "move_right"))
            input.X += 1;
        if (Input.IsActionPressed(InputPrefix + "move_left"))
            input.X -= 1;

        player.Move(input);

        if (Input.IsActionJustPressed(InputPrefix + "jump"))
            player.TryJump();

        if (Input.IsActionJustPressed(InputPrefix + "attack"))
            player.Attack();

        if (Input.IsActionJustPressed(InputPrefix + "dodge"))
            player.Dodge();

        if (Input.IsActionJustPressed(InputPrefix + "ulting"))
            player.TriggerUlt();
    }
}