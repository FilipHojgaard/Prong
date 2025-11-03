using Godot;

namespace Prong.Src.Blocks;

public partial class Block : RigidBody2D
{

    private int _hp { get; set; }
    public override void _Ready()
    {
        GravityScale = 0;
        Mass = 1000;
    }

    public void Initialize(int hp)
    {
        _hp = hp;
        UpdateSprite();
    }

    public void TakeHit()
    {
        _hp--;
        if (_hp < 1)
        {
            QueueFree();
            return;
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
