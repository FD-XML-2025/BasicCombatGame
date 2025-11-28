using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Player : Character
{
    // Speed pixels/s
    private float _moveSpeed = 250f;

    private AnimatedSprite _idleSprite;
    
    private AnimatedSprite _walkSprite;

    public Player(AnimatedSprite sprite, AnimatedSprite walkSprite) : base(sprite)
    {
        // Store the used sprites
        _idleSprite = sprite;
        _walkSprite = walkSprite;
        
        // Default the scale to x2 because 128x128 was too little
        Scale = new Vector2(2f, 2f);
        Sprite = _idleSprite;
    }

    public void Initialize(Vector2 startingPosition)
    {
        // Setup the player position
        Position = startingPosition;
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Handle player inputs
        HandleInput();
    }

    // Override OnSartMove event to set the walk sprite
    public override void OnStartMove()
    {
        base.OnStartMove();

        Sprite = _walkSprite;
    }

    // Override OnStopMove event to set the idle sprite
    public override void OnStopMove()
    {
        base.OnStopMove();
        
        Sprite = _idleSprite;
    }

    public override void Draw()
    {
        base.Draw();
    }

    private void HandleInput()
    {
        // Reset velocity (prevent from auto move)
        Velocity = Vector2.Zero;

        // Handle backward movement
        if (GameController.MoveBackward())
        {
            Velocity.X = -_moveSpeed;
        }

        // Handle forward movement
        if (GameController.MoveForward())
        {
            Velocity.X = _moveSpeed;
        }

        // Make the player attack
        if (GameController.Action())
        {
            Attack();
        }
    }

    private void Attack()
    {
        
    }
}