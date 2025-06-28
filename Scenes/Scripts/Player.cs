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

    private int currentHealth;

    public Vector2 Velocity = Vector2.Zero;
    public bool IsJumping = false;

    private Area2D? attackArea;
    private FighterController? controller;
    private CollisionShape2D? attackShape;

    private bool isAttacking = false;
    private bool isHeavyAttacking = false;
    private bool isDodging = false;
    private bool isUlting = false;
    private bool isHit = false;


    public override void _Ready()
    {
        attackArea = GetNode<Area2D>("AttackArea");
        attackArea!.Monitoring = false;
        attackArea.BodyEntered += OnAttackHit;
        attackShape = attackArea.GetNode<CollisionShape2D>("CollisionShape2D");
        attackShape!.Disabled = true;

        GD.Print("AttackArea Layer: " + attackArea.CollisionLayer);
        GD.Print("AttackArea Mask: " + attackArea.CollisionMask);



        controller = GetNode<FighterController>("FighterController");
        controller!.Init(this);

        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.AnimationFinished += OnAnimationFinished;

        currentHealth = MaxHealth;
    }

    public override void _Process(double delta)
    {
        float deltaF = (float)delta;

        controller!.Tick(delta);

        // Gravity with fall boost
        if (Velocity.Y > 0)
            Velocity.Y += GravityForce * FallMultiplier * deltaF;
        else
            Velocity.Y += GravityForce * deltaF;

        Position += Velocity * deltaF;

        // Landing
        if (Position.Y >= GroundY)
        {
            Position = new Vector2(Position.X, GroundY);
            Velocity.Y = 0;
            IsJumping = false;
        }

        // Animation
        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (isHit)
        {
            anim.Animation = "GotHit";
            anim.Play();
            return;
        }

        if (isAttacking)
        {
            // 播放攻击动画，不做其他切换
            anim.Animation = "Attack";
            anim.Play();
            return;
        }

        if (isDodging)
        {
            anim.Animation = "Dodge";
            anim.Play();
            return;
        }

        if (IsJumping)
        {
            anim.Animation = "Jump";
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

        UpdateAttackAreaDirection();
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

    private void UpdateAttackAreaDirection()
    {
        var flip = GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH;
        var pos = attackArea!.Position;

        // 假设默认朝左攻击，翻转时朝右
        pos.X = Math.Abs(pos.X) * (flip ? -1 : 1);
        attackArea.Position = pos;
    }

    public void Attack()
    {
        GD.Print("攻击动作！");
        isAttacking = true;

        attackArea!.Monitoring = true;
        attackShape!.Disabled = false; // ✅ 启用攻击判定区域

        GetTree().CreateTimer(0.15f).Timeout += () =>
        {
            attackArea.Monitoring = false;
            attackShape!.Disabled = true; // ✅ 自动隐藏攻击区域
        };

        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.Animation = "Attack";
        anim.Play();
    }

    public void TakeDamage(int amount)
    {
        if (isDodging || isHit) return; // 躲避时不会受伤

        currentHealth -= amount;
        GD.Print($"受到伤害！当前血量：{currentHealth}");

        if (currentHealth <= 0)
        {
            Lives--;
            if (Lives > 0)
            {
                GD.Print("损失一命！复活！");
                currentHealth = MaxHealth;
                // 可以播放倒地再起动画
            }
            else
            {
                GD.Print("游戏结束！");
                QueueFree(); // 角色死亡
            }
        }

        GotHit(); // 播放受击动画
    }

    private void OnAttackHit(Node body)
    {
        GD.Print($"命中：{body.Name}");

        // 如果是双人模式，对方是 Player 并且不是自己
        if (!IsAI && body is Player target && target != this)
        {
            target.TakeDamage(80);
        }
    }

    private void OnAnimationFinished()
    {
        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (anim.Animation == "Attack" && isAttacking)
        {
            isAttacking = false;
        }
        if (anim.Animation == "Dodge" && isDodging)
        {
            isDodging = false;
        }
        if (anim.Animation == "GotHit" && isHit)
        {
            isHit = false;
        }
    }

    public void Dodge()
    {
        if (isAttacking || isHeavyAttacking || isDodging || isUlting)
            return;

        GD.Print("躲避！");
        isDodging = true;

        // 向后退一步（根据当前朝向）
        float dodgeDistance = 100f;
        float direction = GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH ? 1f : -1f;

        Position += new Vector2(dodgeDistance * direction, 0);

        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.Animation = "Dodge";
        anim.Play();
    }

    public void GotHit()
    {
        if (isHit || isDodging) return; // 躲避中不会受击

        GD.Print("被击中！");
        isHit = true;

        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.Animation = "GotHit";
        anim.Play();
    }
}
