using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Ennemie
{
    // State and animation data
    private float attackStartX;
    private float playerStartX;
    private AnimatedSprite _walkSprite, _aboutToHitSprite, _hittingSprite;
    private AnimatedSprite _aboutToKickSprite, _kickingSprite, _defeatedSprite;
    private AnimatedSprite _currentSprite;

    // Positioning and movement
    private Vector2 position;
    private Vector2 velocity;
    private Character _player;
    private float speed;

    // Attributes
    private double hp;
    private string type;
    private string currentState;
    private float stateTimer;
    private const float StateDuration = 0.5f;
    private bool facingLeft = true;

    public bool IsDead => hp <= 0;
    public bool IsAlive => !IsDead;

    public Vector2 Position
    {
        get => position;
        set => position = value - new Vector2(_currentSprite.Width / 2, _currentSprite.Height / 2);
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
        this._player = player;

        speed = 100f;
        velocity = Vector2.Zero;
        currentState = "walk";
        stateTimer = 0f;

        UpdateCurrentSprite();
    }

    // Switch to the correct animation for the current state
    private void UpdateCurrentSprite()
    {
        _currentSprite = currentState switch
        {
            "walk" => _walkSprite,
            "hit" => _aboutToHitSprite,
            "about_to_hit" => _aboutToHitSprite,
            "hitting" => _hittingSprite,
            "about_to_kick" => _aboutToKickSprite,
            "kicking" => _kickingSprite,
            "defeated" => _defeatedSprite,
            _ => _walkSprite
        };
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        stateTimer += delta;

        // If dead, only update the defeated animation
        if (hp <= 0)
        {
            currentState = "defeated";
            UpdateCurrentSprite();
            _currentSprite.Update(gameTime);
            return;
        }

        float playerX = GetPlayerCenterX();
        float tigerX = GetTigerCenterX();

        // Enemy should always face the player
        float attackOffset = 200f;
        if (currentState == "walk" || currentState == "hit")
            facingLeft = playerX < tigerX - attackOffset;

        // Decide whether to start an attack
        if (currentState == "walk")
        {
            bool facingPlayer =
                (facingLeft && playerX < tigerX) ||
                (!facingLeft && playerX > tigerX);

            float dist = tigerX - playerX;
            float attackRange = facingLeft ? 380f : -180f;

            bool inAttackRange = facingLeft
                ? dist > 0 && dist <= attackRange
                : dist < 0 && dist >= attackRange;

            if (facingPlayer && inAttackRange)
            {
                if (stateTimer > 1.0f)
                {
                    stateTimer = 0f;
                    currentState = Random.Shared.Next(2) == 0
                        ? "about_to_hit"
                        : "about_to_kick";
                }
            }
        }

        // Wind-up transitions
        if (currentState == "about_to_hit" && stateTimer > 0.3f)
        {
            currentState = "hitting";
            Attack();
        }

        if (currentState == "about_to_kick" && stateTimer > 0.3f)
        {
            currentState = "kicking";
            Attack();
        }

        // End attack animation
        if ((currentState == "hitting" || currentState == "kicking") && stateTimer > 0.7f)
        {
            currentState = "walk";
        }

        // Move towards player when walking
        if (currentState == "walk")
        {
            float dist = tigerX - playerX;
            float stopDistance = facingLeft ? 380f : -50f;

            if (!facingLeft)
            {
                if (dist < -stopDistance)
                    position.X += speed * delta;
            }
            else
            {
                if (dist > stopDistance)
                    position.X -= speed * delta;
            }
        }

        // Recover from hit animation
        if (currentState == "hit" && stateTimer > 0.4f)
            currentState = "walk";

        UpdateCurrentSprite();
        _currentSprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsDead && currentState != "defeated")
            return;

        _currentSprite.Effects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        float originX = _currentSprite.Width / 2f;
        float originY = 62f;

        _currentSprite.Origin = new Vector2(originX, originY);
        _currentSprite.Scale = new Vector2(2f, 2f);

        float visualOffsetY = GetAnimationOffsetY() * _currentSprite.Scale.Y;
        float flipCorrection = GetFlipOffsetX() * _currentSprite.Scale.X;

        Vector2 drawPos = position + new Vector2(64 + flipCorrection, visualOffsetY);

        _currentSprite.Draw(spriteBatch, drawPos);
    }

    private float GetAnimationOffsetY()
    {
        return currentState switch
        {
            "walk" => 0f,
            "about_to_hit" => 0f,
            "hitting" => 0f,
            "about_to_kick" => -20f,
            "kicking" => -20f,
            "defeated" => 0f,
            "hit" => -5f,
            _ => 0f
        };
    }

    // Fixes horizontal shift when flipping the sprite
    private float GetFlipOffsetX() => facingLeft ? -15f : 15f;

    public void Attack()
    {
        _player.TakeDamage(10);
    }

    private float GetTigerCenterX() => position.X + 64f;
    private float GetPlayerCenterX() => _player.Position.X;

    public void TakeDamage(double damage)
    {
        hp -= damage;

        if (hp > 0 && currentState != "hit")
        {
            currentState = "hit";
            stateTimer = 0f;
            UpdateCurrentSprite();
        }
    }
}