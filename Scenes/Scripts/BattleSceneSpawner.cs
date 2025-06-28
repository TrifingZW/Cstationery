using Godot;
using System;

// This script dynamically spawns playerA and playerB in the battle scene
// based on the character choices stored in GameManager.

public partial class BattleSceneSpawner : Node
{
    // [Export] public Node2D PlayerASpawnPoint;
    // [Export] public Node2D PlayerBSpawnPoint;

    // private Node2D playerA;
    // private Node2D playerB;

    // public override void _Ready()
    // {
    //     var canvas = GetNode("CanvasLayer");

    //     var healthBarA = canvas.GetNode<TextureProgressBar>("HealthBarA");
    //     var energyBarA = canvas.GetNode<TextureProgressBar>("EnergyBarA");
    //     var healthBarB = canvas.GetNode<TextureProgressBar>("HealthBarB");
    //     var energyBarB = canvas.GetNode<TextureProgressBar>("EnergyBarB");

    //     playerA = SpawnPlayer(GameManager.PlayerAChoice, PlayerASpawnPoint.Position, "playerA_");
    //     playerB = SpawnPlayer(GameManager.PlayerBChoice, PlayerBSpawnPoint.Position, "playerB_");

    //     // 绑定 UI 条件（无论是 PencilPlayer 还是 EraserPlayer，只要实现了方法）
    //     if (playerA is Node nodeA)
    //     {
    //         nodeA.Call("SetHealthBar", healthBarA);
    //         nodeA.Call("SetEnergyBar", energyBarA);
    //     }

    //     if (playerB is Node nodeB)
    //     {
    //         nodeB.Call("SetHealthBar", healthBarB);
    //         nodeB.Call("SetEnergyBar", energyBarB);
    //     }
    // }

    // private Node2D SpawnPlayer(GameManager.CharacterType choice, Vector2 position, string inputPrefix)
    // {
    //     PackedScene scene = choice switch
    //     {
    //         GameManager.CharacterType.Pencil => GD.Load<PackedScene>("res://Characters/PencilPlayer.tscn"),
    //         GameManager.CharacterType.Eraser => GD.Load<PackedScene>("res://Characters/EraserPlayer.tscn"),
    //         _ => null
    //     };

    //     if (scene == null) return null;

    //     var player = scene.Instantiate<Node2D>();
    //     player.Position = position;
    //     GetParent().AddChild(player);

    //     if (player.HasNode("FighterController"))
    //     {
    //         var controller = player.GetNode<FighterController>("FighterController");
    //         if (controller is InputController inputCtrl)
    //         {
    //             inputCtrl.InputPrefix = inputPrefix;
    //         }
    //     }

    //     return player;
    // }
}
