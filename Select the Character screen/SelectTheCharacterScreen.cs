using Godot;
using System;

public partial class SelectTheCharacterScreen : Node2D
{
    private bool _bIsSelectA = true;

    public void SelectsE()
    {
        var gameManager = GetNode<GameManager>("/root/GameManager");
        if (_bIsSelectA)
            gameManager.PlayerAChoice = CharacterType.Eraser;
        else
        {
            gameManager.PlayerBChoice = CharacterType.Eraser;
            ChangeScene();
        }

        _bIsSelectA = false;
    }

    public void SelectsP()
    {
        var gameManager = GetNode<GameManager>("/root/GameManager");
        if (_bIsSelectA)
            gameManager.PlayerAChoice = CharacterType.Pencil;
        else
        {
            gameManager.PlayerBChoice = CharacterType.Pencil;
            ChangeScene();
        }

        _bIsSelectA = false;
    }

    public void ChangeScene()
    {
        // 切换到游戏场景
        GetTree().ChangeSceneToFile("res://Scenes/BattleScene.tscn");
    }
}