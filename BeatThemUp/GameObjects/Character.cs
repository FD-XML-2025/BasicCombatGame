using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Character : Actor
{
    // The AnimatedSprite used when drawing each slime segment
    private float _health = 100f;
    
    private float _maxHealth = 100f;

    private Weapon _weapon;

    private bool _isParrying;
    
    private bool _wasMoving;

    public AnimatedSprite Sprite { get; set; }

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
        Sprite = sprite;
        Scale = new Vector2(2f, 2f);
    }
    
    // Determine wether the character is idle
    public bool IsIdle()
    {
        return Velocity ==  Vector2.Zero;
    }

    // Determine wether the character is walking
    public bool IsWalking()
    {
        return Velocity.Length() > 0;
    }

    /// <summary>
    /// Updates the character.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Call Start/Move events
        if (_wasMoving != IsWalking())
        {
            if (_wasMoving)
                OnStopMove();
            else
                OnStartMove();
        }
        
        // Update the animated sprite.
        Sprite.Scale = Scale;
        Sprite.Update(gameTime);
    }

    /// <summary>
    /// Draws the character.
    /// </summary>
    public override void Draw()
    {
        base.Draw();
        
        Sprite.Draw(Core.SpriteBatch, Position);
    }

    /// <summary>
    /// Returns a Circle value that represents collision bounds of the slime.
    /// </summary>
    /// <returns>A Circle value.</returns>
    public Circle GetBounds()
    {
        // Create the bounds.
        Circle bounds = new Circle(
            (int)(Position.X + (Sprite.Width * 0.5f)),
            (int)(Position.Y + (Sprite.Height * 0.5f)),
            (int)(Sprite.Width * 0.5f)
        );

        return bounds;
    }
    
    public virtual void OnStartMove()
    {
        if (_wasMoving) return;
        
        _wasMoving = true;
    }

    public virtual void OnStopMove()
    {
        if (!_wasMoving) return;
        
        _wasMoving = false;

        Sprite = new AnimatedSprite();
    }
}