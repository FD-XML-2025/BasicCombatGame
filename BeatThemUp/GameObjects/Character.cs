using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Character : Actor
{
    // The AnimatedSprite used when drawing each slime segment
    private AnimatedSprite _sprite;

    /// <summary>
    /// Event that is raised if it is detected that the head segment of the slime
    /// has collided with a body segment.
    /// </summary>
    public event EventHandler BodyCollision;
    
    public Character() {}

    /// <summary>
    /// Creates a new Character using the specified animated sprite.
    /// </summary>
    /// <param name="sprite">The AnimatedSprite to use when drawing the character.</param>
    public Character(AnimatedSprite sprite)
    {
        _sprite = sprite;
    }

    /// <summary>
    /// Updates the slime.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Update the animated sprite.
        _sprite.Update(gameTime);
    }

    /// <summary>
    /// Draws the character.
    /// </summary>
    public override void Draw()
    {
        base.Draw();
        
        _sprite.Draw(Core.SpriteBatch, Position);
    }

    /// <summary>
    /// Returns a Circle value that represents collision bounds of the slime.
    /// </summary>
    /// <returns>A Circle value.</returns>
    public Circle GetBounds()
    {
        // Create the bounds.
        Circle bounds = new Circle(
            (int)(Position.X + (_sprite.Width * 0.5f)),
            (int)(Position.Y + (_sprite.Height * 0.5f)),
            (int)(_sprite.Width * 0.5f)
        );

        return bounds;
    }
}