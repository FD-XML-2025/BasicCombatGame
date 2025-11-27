using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Ennemie
{
    private float attackStartX;   //  tiger pos when attack started
    private float playerStartX;   // player pos when attack started
    private AnimatedSprite _walkSprite, _aboutToHitSprite, _hittingSprite;
    private AnimatedSprite _aboutToKickSprite, _kickingSprite, _defeatedSprite;
    private AnimatedSprite _currentSprite;
    private Vector2 position;
    private Vector2 velocity;
    private Character _player;
    private float speed;
    private double hp;
    private string type;
    private string currentState; // animation state
    private float stateTimer;
    private const float StateDuration = 0.5f;
    private readonly Vector2 origin = new Vector2(64, 128); // Bottom-center
    private bool facingLeft = true;
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
        this._player = player;
        this.speed = 100f;
        this.velocity = Vector2.Zero;
        this.currentState = "walk";
        this.stateTimer = 0f;
        UpdateCurrentSprite();
    }

    private void UpdateCurrentSprite()
    {
        _currentSprite = currentState switch
        {
            "walk"           => _walkSprite,
            "hit"            => _aboutToHitSprite,
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
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        stateTimer += delta;

        if (hp <= 0)
        {
            currentState = "defeated";
            UpdateCurrentSprite();
            _currentSprite.Update(gameTime);
            return;
        }

        float playerX = _player.Position.X;
        float tigerX  = position.X;

        if (currentState == "walk" || currentState == "hit")
            facingLeft = playerX < tigerX;

        // Start attack when close enough
        if (currentState == "walk")
        {
            bool inFront = facingLeft ? (playerX < tigerX) : (playerX > tigerX);
            if (inFront && Math.Abs(playerX - tigerX) < 220f)
            {
                if (stateTimer > 1.0f)
                {
                    stateTimer = 0f;
                    currentState = Random.Shared.Next(2) == 0 ? "about_to_hit" : "about_to_kick";
                }
            }
        }

        // Attack sequence
        if (currentState == "about_to_hit" && stateTimer > 0.3f) { currentState = "hitting"; Attack(); }
        if (currentState == "about_to_kick" && stateTimer > 0.3f) { currentState = "kicking"; Attack(); }
        if ((currentState == "hitting" || currentState == "kicking") && stateTimer > 0.7f)
            currentState = "walk";

        // stopping distance based on direction
        if (currentState == "walk")
        {
            float stopDistance = facingLeft ? 210f : 150f;   // ← Tiger on right = farther stop

            if (facingLeft)
            {
                if (position.X > _player.Position.X + stopDistance)
                    position.X -= speed * delta;
            }
            else
            {
                if (position.X < _player.Position.X - stopDistance)
                    position.X += speed * delta;
            }
        }

        if (currentState == "hit" && stateTimer > 0.4f) currentState = "walk";

        UpdateCurrentSprite();
        _currentSprite.Update(gameTime);
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsDead && currentState != "defeated") return;

        _currentSprite.Origin = new Vector2(128, 128);  // Bottom-RIGHT pivot

        _currentSprite.Effects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        // move to centre feet at position
        Vector2 drawPos = position + new Vector2(64, 0);

        _currentSprite.Draw(spriteBatch, drawPos);
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