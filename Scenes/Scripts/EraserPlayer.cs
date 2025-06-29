using Godot;
using System;

public partial class EraserPlayer : Player
{
    [Export] public new float AttackOffsetX = 330f;

    private float ultDuration = 2.0f; // 2 seconds of invulnerability
    private float ultTimer = 0f;
    private AnimatedSprite2D ultingEffect;

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

        // === Footstep sound setup ===
        walkSFX = GetNode<AudioStreamPlayer2D>("WalkSFX");
        rng.Randomize();
        footstepClips = new AudioStream[]
        {
            GD.Load<AudioStream>("res://音效/人物/人物-橡皮-footstep1.wav"),
            GD.Load<AudioStream>("res://音效/人物/人物-橡皮-footstep2.wav"),
            GD.Load<AudioStream>("res://音效/人物/人物-橡皮-footstep3.wav"),
            GD.Load<AudioStream>("res://音效/人物/人物-橡皮-footstep4.wav"),
        };

        hitSFX = GetNode<AudioStreamPlayer2D>("HitSFX");
        attackSFX = GetNode<AudioStreamPlayer2D>("AttackSFX");
        dodgeSFX = GetNode<AudioStreamPlayer2D>("DodgeSFX");
        ultSFX = GetNode<AudioStreamPlayer2D>("UltSFX");
        jumpSFX = GetNode<AudioStreamPlayer2D>("JumpSFX");

        ultingEffect = GetNode<AnimatedSprite2D>("Ulting");
        ultingEffect.Visible = false;
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
        else if (isDying)
        {
            anim.Animation = "Dead";
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

            footstepTimer -= deltaF; // 每帧递减

            if (footstepTimer <= 0f && footstepClips.Length > 0)
            {
                walkSFX.Stream = footstepClips[rng.RandiRange(0, footstepClips.Length - 1)];
                walkSFX.Play();
                footstepTimer = footstepCooldown; // 重置计时器
            }
        }
        else
        {
            anim.Animation = "Idle";

            if (walkSFX.Playing)
            {
                walkSFX.Stop();
            }
            
            footstepTimer = 0f;
        }

        anim.Play();
    }
    public override void TriggerUlt()
    {
        if (isUlting || isAttacking || isHit || isDodging) return;

        if (energy < MaxEnergy)
        {
            GD.Print($"{Name} 能量不足，无法进入霸体状态！");
            return;
        }

        GD.Print($"{Name} 进入霸体状态！");
        isUlting = true;

        ultSFX.Play();

        energy = 0;
        if (EnergyBar != null)
            EnergyBar.Value = energy;

        if (ultingEffect != null)
        {
            ultingEffect.Visible = true;
            ultingEffect.Play();
        }

        GetTree().CreateTimer(3f).Timeout += () =>
        {
            isUlting = false;
            if (ultingEffect != null)
            {
                ultingEffect.Visible = false;
                ultingEffect.Stop();
            }
            GD.Print($"{Name} 霸体结束");
        };
    }
  public override void TakeDamage(int amount)
    {
        if (isDodging || isHit || isUlting) return; // 霸体期间不受伤
        base.TakeDamage(amount);
    }

    protected override void UpdateAttackAreaPosition()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        bool facingLeft = sprite.FlipH;

        Vector2 pos = attackArea.Position;
        pos.X = facingLeft ? -AttackOffsetX : 0;
        attackArea.Position = pos;
    }
}
