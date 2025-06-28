using Godot;

namespace Cstationery.Scenes.Scripts;

public partial class CstationeryCharacter : Node2D
{
    [Export] public Vector2 Direction { get; set; } = Vector2.One;

    public override void _PhysicsProcess(double delta)
    {
        SetPosition(GetPosition() + Direction * (float)delta * 50);
    }
}