using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Player : Character
{
    // Speed pixels/s
    private float _moveSpeed;
    
    public Player(AnimatedSprite sprite) : base(sprite) { }

    public void Initialize(Vector2 startingPosition, float moveSpeed = 250f)
    {
        Position = startingPosition;
        _moveSpeed = moveSpeed;
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Gérer les entrées utilisateur
        HandleInput();
    }

    private void HandleInput()
    {
        Velocity = Vector2.Zero;

        if (GameController.MoveBackward())
        {
            Velocity.X = -_moveSpeed;
        }

        if (GameController.MoveForward())
        {
            Velocity.X = _moveSpeed;
        }
    }
}