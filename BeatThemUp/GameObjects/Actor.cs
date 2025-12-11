using Microsoft.Xna.Framework;

namespace BeatThemUp.GameObjects;

/// <summary>
/// Abstract base class for all game actors.
/// If you need to create an object that have a position in the game world,
/// this is the class to extend.
/// </summary>
public abstract class Actor
{
    /// <summary>
    /// Gets or Sets the position of the actor.
    /// </summary>
    public Vector2 Position;
    
    // The velocity that defines the direction and how much in that
    // direction to update the actor position each update cycle.
    public Vector2 Velocity;

    public Vector2 Scale;
    
    // Rotation of the actor in radians.
    public float Rotation { get; set; }
    
    // Whether the actor is active or not.
    public bool IsActive { get; set; } = true;

    public virtual void Initialize()
    {
        Position = Vector2.Zero;
        Velocity = Vector2.Zero;
        Scale = Vector2.One;
        Rotation = 0f;
    }

    public virtual void Update(GameTime gameTime)
    {
        // Update actor position based on velocity.
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * deltaTime;
    }
    
    public virtual void Draw() { }
}