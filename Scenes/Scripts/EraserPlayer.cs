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
        base._Ready();

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
    public override void TriggerUlt()
    {
        if (isUlting || isAttacking || isHit || isDodging) return;

        // if (energy < MaxEnergy)
        // {
        //     GD.Print($"{Name} 能量不足，无法进入霸体状态！");
        //     return;
        // }

        GD.Print($"{Name} 进入霸体状态！");
        isUlting = true;

        // energy = 0;
        // if (EnergyBar != null)
        //     EnergyBar.Value = energy;

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
