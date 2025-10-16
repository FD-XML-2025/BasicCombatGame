using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;

namespace BasicCombatGame;

public class Character : ICollisionActor
{
    public Vector2 Velocity;
    
    public Character()
    {
        Bounds = new RectangleF();
    }

    public IShapeF Bounds { get; set; }

    public String LayerName { get; set; }

    public void OnCollision(CollisionEventArgs collisionInfo)
    {
        Velocity.X *= -1;
        Velocity.Y *= -1;
        Bounds.Position -= collisionInfo.PenetrationVector;
    }
    
    /*private AnimatedSprite _adventurer;

    public void LoadContent(SpriteSheet spriteSheet)
    {
        TimeSpan duration = TimeSpan.FromSeconds(0.1);
        
        spriteSheet.DefineAnimation("idle", builder =>
        {
            builder.IsLooping(true)
                .AddFrame("yakuza-male-idle-01", duration)
                .AddFrame("yakuza-male-idle-02", duration)
                .AddFrame("yakuza-male-idle-03", duration)
                .AddFrame("yakuza-male-idle-04", duration)
                .AddFrame("yakuza-male-idle-05", duration)
                .AddFrame("yakuza-male-idle-06", duration)
                .AddFrame("yakuza-male-idle-07", duration)
                .AddFrame("yakuza-male-idle-08", duration);
        });

        _adventurer = new AnimatedSprite(spriteSheet, "idle");
    }*/
}