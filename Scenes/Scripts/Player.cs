using Godot;
using System;

public partial class Player : Area2D
{
    [Export] public int Speed { get; set; } = 400;
    [Export] public int JumpForce { get; set; } = 1200;
    [Export] public int GravityForce { get; set; } = 3000;
    [Export] public float GroundY { get; set; } = 750f;
    [Export] public float FallMultiplier { get; set; } = 1.5f;
    [Export] public bool IsAI = false;
    [Export] public int MaxHealth = 1000;
    [Export] public int Lives = 3;
    [Export] public float AttackOffsetX = 250f;
    [Export] public float BodyOffsetX = 30f;
    [Export] public TextureProgressBar HealthBar;
    [Export] public TextureProgressBar EnergyBar;
    [Export] public float MinX = 100f;
    [Export] public float MaxX = 1800f;


    private int currentHealth;

    public Vector2 Velocity = Vector2.Zero;
    public bool IsJumping = false;

    public static bool IsAnyUlting = false;

    protected int energy = 0;
    public const int MaxEnergy = 100;

    protected Sprite2D heart1;
    protected Sprite2D heart2;
    protected Sprite2D heart3;

    protected Node2D hearts;
    protected Vector2 heartsInitialPosition;


    protected Area2D bodyArea;
    protected CollisionShape2D bodyShape;
    protected Area2D attackArea;
    protected CollisionShape2D attackShape;
    protected FighterController controller;

    protected bool isAttacking = false;
    protected bool isHeavyAttacking = false;
    protected bool isDodging = false;
    protected bool isUlting = false;
    protected bool isHit = false;
    public bool isDying = false;
    public bool isDead = false;

    private Player ultTarget;
    private int ultProjectilesArrived = 0;
    private int totalUltProjectiles = 10;

    private bool isInvincible = false;

    public override void _Ready()
    {
        AddToGroup("player");

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

        if (EnergyBar != null)
            EnergyBar.Value = energy;

        heart1 = GetNode<Sprite2D>("Hearts/Heart1");
        heart2 = GetNode<Sprite2D>("Hearts/Heart2");
        heart3 = GetNode<Sprite2D>("Hearts/Heart3");

        UpdateHearts();

        hearts = GetNode<Node2D>("Hearts");
        heartsInitialPosition = hearts.Position;
        AddToGroup("player");
    }

    public void Heal(int amount)
    {
        // 确保不会超过最大生命值
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        GD.Print($"{Name} 恢复 {amount} 点生命值! 当前生命: {currentHealth}");
    
        if (HealthBar != null)
        {
            HealthBar.Value = currentHealth;
        }
    }
    public override void _Process(double delta)
    {
        UpdateAttackAreaPosition();
        UpdateBodyPosition();
        UpdateHeartsPosition();

        float deltaF = (float)delta;

        controller.Tick(delta);

        Velocity.Y += (Velocity.Y > 0 ? GravityForce * FallMultiplier : GravityForce) * deltaF;
        Position += Velocity * deltaF;

        // 限制 X 在边界内
        Position = new Vector2(
            Mathf.Clamp(Position.X, MinX, MaxX),
            Position.Y
        );

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
        else if (isDying)
        {
            anim.Animation = "AbilityAttack";
        }
        else if (isDead)
        {
            anim.Animation = "Die";
            anim.Stop();
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

    public void SetHealthBar(TextureProgressBar bar)
    {
        HealthBar = bar;
        if (HealthBar != null)
            HealthBar.Value = currentHealth;
    }

    public void SetEnergyBar(TextureProgressBar bar)
    {
        EnergyBar = bar;
        if (EnergyBar != null)
            EnergyBar.Value = energy;
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

    protected virtual void UpdateAttackAreaPosition()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        bool facingLeft = sprite.FlipH;

        Vector2 pos = attackArea.Position;
        pos.X = facingLeft ? -AttackOffsetX : 0;
        attackArea.Position = pos;
    }

    protected virtual void UpdateBodyPosition()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        bool facingLeft = sprite.FlipH;

        Vector2 pos = bodyArea.Position;
        pos.X = facingLeft ? BodyOffsetX : 0;
        bodyArea.Position = pos;
    }

    protected virtual void UpdateHeartsPosition()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        bool facingLeft = sprite.FlipH;

        Vector2 pos = heartsInitialPosition;
        if (facingLeft)
        {
            pos.X += 30f; // 向右移动 100
        }
        hearts.Position = pos;
    }


    public void Attack()
    {
        if (isAttacking || isHit || isDodging || isUlting || isHeavyAttacking)
            return;

        GD.Print($"{Name} 发起攻击！");
        isAttacking = true;

        attackArea.Monitoring = true;
        attackShape.Disabled = false;
    }

    private void OnAttackHit(Area2D area)
    {
        if (area == attackArea) return;

        GD.Print($"{Name} 检测命中：{area.Name}");

        if (area.Owner is Area2D other && other != this && other.HasMethod("TakeDamage"))
        {
            GD.Print($"{Name} 命中 {other.Name}");
            other.Call("TakeDamage", 80);
            GainEnergy(10);
        }

    }

    public virtual void TakeDamage(int amount)
    {
        if (isDodging || isHit || isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        GD.Print($"{Name} 受到伤害，当前血量：{currentHealth}");

        if (HealthBar != null)
        {
            HealthBar.Value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            if (!isDying)
            {
                isDying = true;
            }
            return;
        }

        GotHit();
    }

    public void GotHit()
    {
        if (isHit || isDodging) return;

        GD.Print($"{Name} 被击中！");
        isHit = true;
    }

    public void Dodge()
    {
        if (isAttacking || isHeavyAttacking || isDodging || isUlting)
            return;

        isDodging = true;

        float dodgeDistance = 100f;
        float direction = GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH ? 1f : -1f;
        Position += new Vector2(dodgeDistance * direction, 0);
    }

    public virtual void TriggerUlt()
    {
        if (isUlting || isAttacking || isHit || isDodging) return;

        if (energy < MaxEnergy)
        {
            GD.Print($"{Name} 能量不足，无法释放终极技能！");
            return;
        }

        GD.Print($"{Name} 释放终极技能！");
        isUlting = true;
        IsAnyUlting = true;

        // 立即清空能量
        energy = 0;
        if (EnergyBar != null)
            EnergyBar.Value = energy;
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

        if (isDying && anim.Animation == "Dead")
        {
            Lives--;
            if (Lives > 0)
            {
                GD.Print($"{Name} 损失一命，复活！");
                currentHealth = MaxHealth;
                HealthBar.Value = currentHealth;
                UpdateHearts();

                isDying = false;

                isInvincible = true;
                Modulate = new Color(1, 1, 1, 0.5f); // 半透明闪烁

                var timer = GetTree().CreateTimer(1f);
                timer.Timeout += () =>
                {
                    isInvincible = false;
                    Modulate = new Color(1, 1, 1, 1f); // 恢复可见
                    GD.Print($"{Name} 无敌状态结束");
                };
            }
            else
            {
                isDead = true;
                UpdateHearts();
                GD.Print($"{Name} 游戏结束！");
                controller.SetInputEnabled(false);

                //通知game manager死啦
                var gameManager = GetNode<GameManager>("/root/GameManager");
                gameManager.OnPlayerDeath(this);
            }
        }
    }

    private void SpawnUltProjectiles()
    {
        var scene = GD.Load<PackedScene>("res://Scenes/GameObject/PencilHead.tscn");
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

    private void GainEnergy(int amount)
    {
        if (energy >= MaxEnergy) return;

        energy += amount;
        energy = Mathf.Min(energy, MaxEnergy);

        if (EnergyBar != null)
            EnergyBar.Value = energy;
    }
    
    private void UpdateHearts()
    {
        heart1.Visible = Lives >= 1;
        heart2.Visible = Lives >= 2;
        heart3.Visible = Lives >= 3;
    }


}
