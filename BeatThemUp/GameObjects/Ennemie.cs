using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Ennemie
{
    private AnimatedSprite _walkSprite, _aboutToHitSprite, _hittingSprite;
    private AnimatedSprite _aboutToKickSprite, _kickingSprite, _defeatedSprite;
    private AnimatedSprite _currentSprite;
    private Vector2 position;
    private Vector2 velocity;
    private Character _player;
    private float speed;
    private double hp;
    private string type;
    private string currentState; // Tracks the current animation state
    private float stateTimer; // Timer to switch states
    private const float StateDuration = 0.5f; // Duration 0.5secs
    private readonly Vector2 origin = new Vector2(64, 128); // Bottom-center (feet!)
    private bool facingLeft = true; // Tiger starts looking left toward player
    public bool IsDead
    {
        get { return hp <= 0; }
    }
    public Vector2 Position
    {
        get => position; 
        set => position = value;
    }
    public bool IsAlive
    {
        get => !IsDead; 
    }

    public Ennemie(float hp, string type, 
        AnimatedSprite walk, AnimatedSprite aboutToHit, AnimatedSprite hitting,
        AnimatedSprite aboutToKick, AnimatedSprite kicking, AnimatedSprite defeated,
        Vector2 position, Character player)
    {
        this.hp = hp;
        this.type = type;
        _walkSprite = walk;
        _aboutToHitSprite = aboutToHit;
        _hittingSprite = hitting;
        _aboutToKickSprite = aboutToKick;
        _kickingSprite = kicking;
        _defeatedSprite = defeated;
        this.position = position;
        this._player = player ?? throw new ArgumentNullException(nameof(player));
        this.speed = 0.8f;
        this.velocity = Vector2.Zero;
        this.currentState = "walk"; // Resting
        this.stateTimer = 0f;
        UpdateCurrentSprite();
    }

    private void UpdateCurrentSprite()
    {
        _currentSprite = currentState switch
        {
            "walk"           => _walkSprite,
            "hit"            => _aboutToHitSprite,     // reuse "about to punch" as flinch/hit
            "about_to_hit"   => _aboutToHitSprite,
            "hitting"        => _hittingSprite,
            "about_to_kick"  => _aboutToKickSprite,
            "kicking"        => _kickingSprite,
            "defeated"       => _defeatedSprite,
            _                => _walkSprite
        };
    }

    public void Update(GameTime gameTime)
    {
        stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        facingLeft = _player.Position.X < position.X;
        // Only check for state changes every StateDuration
        if (stateTimer >= StateDuration)
        {
            stateTimer = 0f;

            float distance = Vector2.Distance(position, _player.Position);

            if (IsDead)
            {
                currentState = "defeated";
            }
            else if (distance < 50) // player is near, start attack sequence
            {
                switch (currentState)
                {
                    case "walk":
                        currentState = (Random.Shared.Next(2) == 0) ? "about_to_hit" : "about_to_kick";
                        break;
                    case "about_to_hit":
                        currentState = "hitting";
                        Attack();
                        break;
                    case "about_to_kick":
                        currentState = "kicking";
                        Attack();
                        break;
                    case "hitting":
                    case "kicking":
                        currentState = "walk"; // back to walk after attack
                        break;
                }
            }
            else
            {
                currentState = "walk"; // player is far
            }

            UpdateCurrentSprite();
        }

        // Move toward player only if walking
        if (currentState == "walk")
        {
            Vector2 direction = _player.Position - position;
            if (direction.LengthSquared() > 0)
            {
                direction.Normalize();
                velocity = direction * speed;
            }
            else
            {
                velocity = Vector2.Zero;
            }
        }
        else
        {
            velocity = Vector2.Zero; // stop moving during attack or hit
        }

        // Apply movement
        position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

        // Update current animation
        _currentSprite.Update(gameTime);
    }



    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsDead && currentState != "defeated") return;

        // Set the properties ONCE per frame – this is how AnimatedSprite works
        _currentSprite.Origin      = new Vector2(64, 128);   // feet locked!
        _currentSprite.Effects     = facingLeft 
            ? SpriteEffects.FlipHorizontally 
            : SpriteEffects.None;

        // Simple draw – uses the Origin and Effects you just set
        _currentSprite.Draw(spriteBatch, position);
        
        
    }

    public void Attack()
    {
        _player.TakeDamage(10); // need to adjust damage separate for kicks and hits
    }

    public void TakeDamage(double damage)
    {
        hp -= damage;
        if (hp > 0 && currentState != "hit") // Only change to hit state if not already dead just in case
        {
            currentState = "hit";
            stateTimer = 0f;
            UpdateCurrentSprite();
        }
    }
}