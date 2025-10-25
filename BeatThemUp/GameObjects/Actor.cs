using Microsoft.Xna.Framework;

namespace BeatThemUp.GameObjects;

public abstract class Actor
{
    /// <summary>
    /// Gets or Sets the position of the actor.
    /// </summary>
    public Vector2 Position;
    
    // The velocity that defines the direction and how much in that
    // direction to update the actor position each update cycle.
    public Vector2 Velocity;

    public Vector2 Size;
    
    public float Rotation { get; set; }
    
    public bool IsActive { get; set; } = true;

    public virtual void Initialize()
    {
        Position = Vector2.Zero;
        Velocity = Vector2.Zero;
        Size = Vector2.One;
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