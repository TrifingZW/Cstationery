using Godot;
using System;

public partial class Player : Area2D
{
    [Export] public int Speed { get; set; } = 400;
    [Export] public int JumpForce { get; set; } = 1200;
    [Export] public int GravityForce { get; set; } = 3000;
    [Export] public float GroundY { get; set; } = 600f;
    [Export] public float FallMultiplier { get; set; } = 1.5f;
    [Export] public bool IsAI = false;
    [Export] public int MaxHealth = 1000;
    [Export] public int Lives = 3;
    [Export] public float AttackOffsetX = 250f;
    [Export] public float BodyOffsetX = 30f;



    private int currentHealth;

    public Vector2 Velocity = Vector2.Zero;
    public bool IsJumping = false;

    public static bool IsAnyUlting = false;

    private Area2D bodyArea;
    private CollisionShape2D bodyShape;

    private Area2D attackArea;
    private CollisionShape2D attackShape;
    private FighterController controller;

    private bool isAttacking = false;
    private bool isHeavyAttacking = false;
    public bool isDodging = false;
    private bool isUlting = false;
    private bool isHit = false;

   private Player ultTarget;
    private int ultProjectilesArrived = 0;
    private int totalUltProjectiles = 10;
    public override void _Ready()
    {
        bodyArea = GetNode<Area2D>("CollisionBody");
        bodyShape = bodyArea.GetNode<CollisionShape2D>("CollisionShape2D");
        attackArea = GetNode<Area2D>("AttackArea");
        attackShape = attackArea.GetNode<CollisionShape2D>("CollisionShape2D");

        attackArea.Monitoring = false;
        attackShape.Disabled = true;

        attackArea.AreaEntered += OnAttackHit;

        controller = GetNode<FighterController>("FighterController");
        controller.Init(this);

        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.AnimationFinished += OnAnimationFinished;

        currentHealth = MaxHealth;
    }

    public override void _Process(double delta)
    {
        UpdateAttackAreaPosition();
        UpdateBodyPosition();

        float deltaF = (float)delta;

        controller.Tick(delta);

        Velocity.Y += (Velocity.Y > 0 ? GravityForce * FallMultiplier : GravityForce) * deltaF;
        Position += Velocity * deltaF;

        if (Position.Y >= GroundY)
        {
            Position = new Vector2(Position.X, GroundY);
            Velocity.Y = 0;
            IsJumping = false;
        }

        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (isHit)
        {
            anim.Animation = "GotHit";
        }
        else if (isAttacking)
        {
            anim.Animation = "Attack";
        }
        else if (isDodging)
        {
            anim.Animation = "Dodge";
        }
        else if (IsJumping)
        {
            anim.Animation = "Jump";
        }
        else if (isUlting)
        {
            anim.Animation = "AbilityAttack";
        }
        else if (Mathf.Abs(Velocity.X) > 0.1f)
        {
            anim.Animation = "Walk";
            anim.FlipH = Velocity.X < 0;
        }
        else
        {
            anim.Animation = "Idle";
        }

        anim.Play();
    }

    public void Move(Vector2 input)
    {
        Velocity.X = input.X * Speed;
    }

    public void TryJump()
    {
        if (!IsJumping)
        {
            Velocity.Y = -JumpForce;
            IsJumping = true;
        }
    }

    private void UpdateAttackAreaPosition()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        bool facingLeft = sprite.FlipH;

        Vector2 pos = attackArea.Position;
        pos.X = facingLeft ? -AttackOffsetX : 0;
        attackArea.Position = pos;
    }

    private void UpdateBodyPosition()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        bool facingLeft = sprite.FlipH;

        Vector2 pos = bodyArea.Position;
        pos.X = facingLeft ? BodyOffsetX : 0;
        bodyArea.Position = pos;
    }


    public void Attack()
    {
        if (isAttacking || isHit || isDodging || isUlting || isHeavyAttacking)
            return;

        GD.Print($"{Name} 发起攻击！");
        isAttacking = true;

        attackArea.Monitoring = true;
        attackShape.Disabled = false;

        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.Animation = "Attack";
        anim.Play();
    }

    private void OnAttackHit(Area2D area)
    {
        if (area == attackArea) return;

        GD.Print($"{Name} 检测命中：{area.Name}");

        if (area.Owner is Player target && target != this)
        {
            GD.Print($"{Name} 命中 {target.Name}");
            target.TakeDamage(80);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDodging || isHit) return;

        currentHealth -= amount;
        GD.Print($"{Name} 受到伤害，当前血量：{currentHealth}");

        if (currentHealth <= 0)
        {
            Lives--;
            if (Lives > 0)
            {
                GD.Print($"{Name} 损失一命，复活！");
                currentHealth = MaxHealth;
            }
            else
            {
                GD.Print($"{Name} 游戏结束！");
                QueueFree();
                return;
            }
        }

        GotHit();
    }

    public void GotHit()
    {
        if (isHit || isDodging) return;

        GD.Print($"{Name} 被击中！");
        isHit = true;

        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.Animation = "GotHit";
        anim.Play();
    }

    public void Dodge()
    {
        if (isAttacking || isHeavyAttacking || isDodging || isUlting)
            return;

        isDodging = true;

        float dodgeDistance = 100f;
        float direction = GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH ? 1f : -1f;
        Position += new Vector2(dodgeDistance * direction, 0);

        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.Animation = "Dodge";
        anim.Play();
    }

    public void TriggerUlt()
    {
        if (isUlting || isAttacking || isHit || isDodging) return;

        GD.Print($"{Name} 释放终极技能！");
        isUlting = true;
        IsAnyUlting = true;

        // var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        // anim.Animation = "Ulting";
        // anim.Play();
    }

    private void OnAnimationFinished()
    {
        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (isAttacking && anim.Animation == "Attack")
        {
            isAttacking = false;
            attackArea.Monitoring = false;
            attackShape.Disabled = true;
        }

        if (isDodging && anim.Animation == "Dodge")
        {
            isDodging = false;
        }

        if (isHit && anim.Animation == "GotHit")
        {
            isHit = false;
        }

        if (isUlting && anim.Animation == "AbilityAttack")
        {
            isUlting = false;
            IsAnyUlting = false;
            GD.Print($"{Name} Ulting 播放完毕，生成弹幕！");
            SpawnUltProjectiles();
        }
    }

    private void SpawnUltProjectiles()
    {
        var scene = GD.Load<PackedScene>("res://Scenes/PencilHead.tscn");
        var root = GetTree().CurrentScene;

        // 找到目标玩家
        ultTarget = null;
        foreach (var child in root.GetChildren())
        {
            if (child is Player p && p != this)
            {
                ultTarget = p;
                break;
            }
        }

        if (ultTarget == null) return;

        for (int i = 0; i < totalUltProjectiles; i++)
        {
            var bullet = scene.Instantiate() as PencilHead;
            bullet.Position = this.Position + new Vector2(0, -i * 20);
            bullet.Target = new Vector2(ultTarget.Position.X, GroundY);
            bullet.TargetPlayer = ultTarget;
            bullet.UltOwner = this;
            bullet.IsLast = (i == totalUltProjectiles - 1);
            GetParent().AddChild(bullet);
        }
    }

    public void NotifyUltFinalHit()
    {
        if (ultTarget == null) return;

        if (ultTarget.IsJumping)
        {
            GD.Print($"{ultTarget.Name} 跳跃中，免疫大招！");
        }
        else if (ultTarget.isDodging)
        {
            ultTarget.TakeDamage(200);
        }
        else
        {
            ultTarget.TakeDamage(400);
        }
    }
}
