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
        var canvas = GetNode("CanvasLayer");

        var healthBarA = canvas.GetNode<TextureProgressBar>("healthyA");
        var energyBarA = canvas.GetNode<TextureProgressBar>("energyA");
        var healthBarB = canvas.GetNode<TextureProgressBar>("healthyB");
        var energyBarB = canvas.GetNode<TextureProgressBar>("energyB");

        var playerA_PencilPfP = canvas.GetNode<Sprite2D>("PlayerA_PencilPfP");
        var playerA_EraserPfP = canvas.GetNode<Sprite2D>("PlayerA_EraserPfP");
        var playerB_PencilPfP = canvas.GetNode<Sprite2D>("PlayerB_PencilPfP");
        var playerB_EraserPfP = canvas.GetNode<Sprite2D>("PlayerB_EraserPfP");

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
        PackedScene? scene = choice switch
        {
            CharacterType.Pencil => GD.Load<PackedScene>("res://Scenes/GameObject/PencilPlayer.tscn"),
            CharacterType.Eraser => GD.Load<PackedScene>("res://Scenes/GameObject/EraserPlayer.tscn"),
            _ => null
        };

        if (scene == null) return null;

        var player = scene.Instantiate<Node2D>();
        player.Position = position;
        GetParent().AddChild(player);

        if (player.HasNode("FighterController"))
        {
            var controller = player.GetNode<FighterController>("FighterController");
            if (controller is InputController inputCtrl)
            {
                inputCtrl.InputPrefix = inputPrefix;
            }
        }

        return player;
    }
}
