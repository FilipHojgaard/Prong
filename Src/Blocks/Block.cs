using Godot;
using Prong.Shared;

namespace Prong.Src.Blocks;

public partial class Block : RigidBody2D
{

    [Export]
    public Prong Owner { get; set; }
    private int _hp { get; set; }

    private AudioStreamPlayer _hitSfx;
    

    public override void _Ready()
    {
        _hitSfx = GetNode<AudioStreamPlayer>("HitSfx");

        CollisionLayer = 4;
        CollisionMask = 3 | 4;

        GravityScale = 0;
        Mass = 1000;
        LockRotation = true;
    }

    public void Initialize(int hp, Prong player)
    {
        _hp = hp;
        UpdateSprite();
        Owner = player;
    }

    public void TakeHit()
    {
        _hp--;
        if (_hp < 1)
        {
            _hitSfx.Reparent(GetTree().Root);
            _hitSfx.Finished += () => _hitSfx.QueueFree();
            _hitSfx.Play();
            QueueFree();
            return;
        }
        else
        {
            _hitSfx.Play();
        }
        UpdateSprite();

    }

    private void UpdateSprite()
    {
        Sprite2D oneHealthSprite = GetNode<Sprite2D>("OneHealth");
        Sprite2D twoHealthSprite = GetNode<Sprite2D>("TwoHealth");
        
        if (_hp < 2)
        {
            oneHealthSprite.Visible = true;
            twoHealthSprite.Visible = false;
        }
        else
        {
            oneHealthSprite.Visible = false;
            twoHealthSprite.Visible = true;
        }
    }
}
