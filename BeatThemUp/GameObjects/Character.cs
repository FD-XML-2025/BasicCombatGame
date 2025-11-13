using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Character : Actor
{
    // The AnimatedSprite used when drawing each slime segment
    private AnimatedSprite _sprite;

    private float _health;
    
    private float _maxHealth;

    private Weapon _weapon;

    private bool _isParrying;

    public Weapon Weapon
    {
        get => _weapon;
        set => _weapon = value;
    }
    
    public bool HasWeapon()
    {
        return Weapon != null;
    }

    public float Health
    {
        get => _health;
        set
        {
            _health = float.Clamp(value, 0, MaxHealth);
            if (_health <= 0)
                OnDeath();
        }
    }
    
    public float MaxHealth
    {
        get => _maxHealth;
        set
        {
            if (value <= 0)
                throw new ArgumentException("MaxHealth must be greater than 0");
            _maxHealth = value;
        }
    }

    // Heal character with x amount of points
    public void Heal(float amount)
    {
        Health += amount;
    }

    // Kill the character, means set health to 0
    public void Kill()
    {
        Health = 0;
    }

    // Character t
    public void TakeDamage(float amount)
    {
        Health -= amount;
    }

    protected void OnDeath()
    {
        
    }

    public bool IsParrying
    {
        get => _isParrying;
        set => _isParrying = value;
    }

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