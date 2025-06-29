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

    public void OnPlayerDeath(Player deadPlayer)
    {
        deadPlayer.isDying = false;

        //暂停每个玩家
        foreach (Node node in GetTree().GetNodesInGroup("player"))
        {
            if (node is Player player)
            {
                var ctrl = player.GetNode<FighterController>("FighterController");
                if (ctrl is InputController ic)
                    ic.SetInputEnabled(false);
            }
        }

        var controller = deadPlayer.GetNode<FighterController>("FighterController");
        string playerName = "";
        if (controller is InputController inputCtrl)
        {
            playerName = inputCtrl.InputPrefix;
        }

        bool isPlayerA = playerName.Contains("A");

        GD.Print(isPlayerA ? "Player A 输了" : "Player B 输了");

        // 加载 EndUI 场景
        PackedScene endUiScene = GD.Load<PackedScene>("res://Ending/end.tscn");
        Node uiInstance = endUiScene.Instantiate();
        GetTree().CurrentScene.AddChild(uiInstance);

        // 调用 SetResult
        if (uiInstance is End end)
        {
            end.SetResult(!isPlayerA);
        }
    }

}