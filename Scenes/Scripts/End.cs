using Godot;
using System;

public partial class End : Node2D
{
    [Export] public TextureRect PlayerA_Win;
    [Export] public TextureRect PlayerA_Loss;
    [Export] public TextureRect PlayerB_Win;
    [Export] public TextureRect PlayerB_Loss;

    public void SetResult(bool isPlayerAWinner)
    {
        PlayerA_Win.Visible = isPlayerAWinner;
        PlayerA_Loss.Visible = !isPlayerAWinner;
        PlayerB_Win.Visible = !isPlayerAWinner;
        PlayerB_Loss.Visible = isPlayerAWinner;
    }
}
