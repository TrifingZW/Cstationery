using Godot;

public enum CharacterType
{
    Pencil,
    Eraser
}

public partial class GameManager : Node
{
    public CharacterType PlayerAChoice { get; set; } = CharacterType.Pencil;
    public CharacterType PlayerBChoice { get; set; } = CharacterType.Eraser;
}