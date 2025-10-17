using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace BasicCombatGame;

/*
 * Entity is an abstract class to create world actors like characters, objects...
 */
public abstract class Entity : IActor, ICollisionActor
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public string SpriteName { get; set; }
    
    public IShapeF Bounds { get; set; }

    public void Draw(SpriteBatch spriteBatch)
    {
        //throw new System.NotImplementedException();
    }

    public void Update(GameTime gameTime)
    {
        Bounds.Position += Velocity * gameTime.GetElapsedSeconds() * 30;
    }

    public void LoadContent(ContentManager content)
    {
        //throw new System.NotImplementedException();
    }

    public void OnCollision(CollisionEventArgs collisionInfo)
    {
        Bounds.Position -= collisionInfo.PenetrationVector;
    }
}