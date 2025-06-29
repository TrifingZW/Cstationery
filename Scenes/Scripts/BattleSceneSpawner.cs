using Godot;
using System;

// This script dynamically spawns playerA and playerB in the battle scene
// based on the character choices stored in GameManager.

public partial class BattleSceneSpawner : Node
{

    private Node2D playerA;
    private Node2D playerB;

    public override void _Ready()
    {
        var healthBarA = GetNode<TextureProgressBar>("healthyA");
        var energyBarA = GetNode<TextureProgressBar>("energyA");
        var healthBarB = GetNode<TextureProgressBar>("healthyB");
        var energyBarB = GetNode<TextureProgressBar>("energyB");

        var playerA_PencilPfP = GetNode<TextureRect>("PlayerA_PencilPfP");
        var playerA_EraserPfP = GetNode<TextureRect>("PlayerA_EraserPfP");
        var playerB_PencilPfP = GetNode<TextureRect>("PlayerB_PencilPfP");
        var playerB_EraserPfP = GetNode<TextureRect>("PlayerB_EraserPfP");

        var global = GetNode<GameManager>("/root/GameManager");
        Vector2 spawnA = new Vector2(430, 750);
        Vector2 spawnB = new Vector2(1440, 750);

        playerA = SpawnPlayer(global.PlayerAChoice, spawnA, "playerA_");
        playerB = SpawnPlayer(global.PlayerBChoice, spawnB, "playerB_");

        // 绑定 UI 条件（无论是 PencilPlayer 还是 EraserPlayer，只要实现了方法）
        if (playerA is Node nodeA)
        {
            nodeA.Call("SetHealthBar", healthBarA);
            nodeA.Call("SetEnergyBar", energyBarA);
        }

        if (playerB is Node nodeB)
        {
            nodeB.Call("SetHealthBar", healthBarB);
            nodeB.Call("SetEnergyBar", energyBarB);
        }

        // 控制 PlayerA 头像显示
        playerA_PencilPfP.Visible = global.PlayerAChoice == CharacterType.Pencil;
        playerA_EraserPfP.Visible = global.PlayerAChoice == CharacterType.Eraser;

        // 控制 PlayerB 头像显示
        playerB_PencilPfP.Visible = global.PlayerBChoice == CharacterType.Pencil;
        playerB_EraserPfP.Visible = global.PlayerBChoice == CharacterType.Eraser;
        }

    private Node2D SpawnPlayer(CharacterType choice, Vector2 position, string inputPrefix)
    {
        GD.Print($"SpawnPlayer 被调用，角色是 {choice}，坐标是 {position}");

        PackedScene? scene = choice switch
        {
            CharacterType.Pencil => GD.Load<PackedScene>("res://Scenes/GameObject/PencilPlayer.tscn"),
            CharacterType.Eraser => GD.Load<PackedScene>("res://Scenes/GameObject/EraserPlayer.tscn"),
            _ => null
        };

        if (scene == null)
        {
            GD.PrintErr("PackedScene 加载失败！");
            return null;
        }

        var player = scene.Instantiate<Node2D>();
        player.Position = position;
        GetTree().CurrentScene.AddChild(player);

        if (player.HasNode("FighterController"))
        {
            var controller = player.GetNode<FighterController>("FighterController");
            if (controller is InputController inputCtrl)
            {
                inputCtrl.InputPrefix = inputPrefix;
            }
        }

        GD.Print($"生成 {choice} 于 {position}");

        return player;
    }
}
