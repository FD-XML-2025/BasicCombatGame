using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Ennemie
{
    // enemy states
    private enum EnemyState
    {
        Walk,
        AboutToHit,
        Hitting,
        AboutToKick,
        Kicking,
        Idle,
        Hit,
        Defeated,
        Retreat
    }

    // animations
    private AnimatedSprite walk, aboutHit, hit, aboutKick, kick, defeated, walkBack, idle;
    private AnimatedSprite current;

    // movement and player reference
    private Vector2 position;
    private Character player;
    private float speed = 100f;

    // health
    private double hp;
    public double GetHp() => hp;

    // stateS
    private EnemyState state = EnemyState.Walk;
    private float stateTimer;

    // attack
    private const float AttackRange = 300f;
    private const float StopDistance = 300f;
    
    // retreat
    private bool retreating;
    private float retreatDuration;
    private float retreatSpeed = 150f; // faster backwards movement

    public bool IsDead => hp <= 0;

    public Vector2 Position
    {
        get => position;
        set => position = value - new Vector2(current.Width / 2, current.Height / 2);
    }

    public Ennemie(
        float hp,
        string type,
        AnimatedSprite walk, AnimatedSprite aboutHit, AnimatedSprite hit,
        AnimatedSprite aboutKick, AnimatedSprite kick, AnimatedSprite defeated, AnimatedSprite walkBack, AnimatedSprite idle,
        Vector2 position, Character player)
    {
        this.hp = hp;
        this.walk = walk;
        this.aboutHit = aboutHit;
        this.hit = hit;
        this.aboutKick = aboutKick;
        this.kick = kick;
        this.defeated = defeated;
        this.walkBack = walkBack;
        this.idle = idle;
        this.position = position;
        this.player = player;

        UpdateCurrentSprite();
    }

    // choose sprite based on current state
    private void UpdateCurrentSprite()
    {
        current = state switch
        {
            EnemyState.Walk        => walk,
            EnemyState.AboutToHit  => aboutHit,
            EnemyState.Hitting     => hit,
            EnemyState.AboutToKick => aboutKick,
            EnemyState.Kicking     => kick,
            EnemyState.Idle        => idle,
            EnemyState.Hit         => hit,
            EnemyState.Defeated    => defeated,
            EnemyState.Retreat     => walkBack,
            _                      => walk
        };
    }

    // helpers
    private float PlayerDist() => (position.X + 64f) - player.Position.X;
    private bool PlayerIsLeft() => PlayerDist() > 0f;

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        stateTimer += dt;

        // if dead handle death animation
        if (HandleDeath(gameTime))
            return;

        float dist = PlayerDist();

        // main state machine
        switch (state)
        {
            case EnemyState.Walk:
                UpdateWalk(dt, dist);
                break;

            case EnemyState.AboutToHit:
                UpdateWindUp(EnemyState.Hitting);
                break;

            case EnemyState.AboutToKick:
                UpdateWindUp(EnemyState.Kicking);
                break;

            case EnemyState.Hitting:
            case EnemyState.Kicking:
                UpdateAttackEnd();
                break;

            case EnemyState.Idle:
                UpdateIdle();
                break;

            case EnemyState.Hit:
                UpdateHitReaction();
                break;
            case EnemyState.Retreat:
                UpdateRetreat(dt);
                break;
        }

        UpdateCurrentSprite();
        current.Update(gameTime);
    }

    // death behavior
    private bool HandleDeath(GameTime gameTime)
    {
        if (hp > 0)
            return false;

        hp = 0;

        if (state != EnemyState.Defeated)
        {
            state = EnemyState.Defeated;
            stateTimer = 0f;
        }

        if (stateTimer > 1f)
            hp = -999; // remove enemy

        UpdateCurrentSprite();
        current.Update(gameTime);
        return true;
    }

    // walking and approaching player
    private void UpdateWalk(float dt, float dist)
    {
        if (PlayerIsLeft() && dist > StopDistance)
            position.X -= speed * dt;

        if (stateTimer > 1f && InAttackRange(dist))
        {
            stateTimer = 0f;

            state = Random.Shared.Next(2) == 0
                ? EnemyState.AboutToHit
                : EnemyState.AboutToKick;
        }
    }

    private bool InAttackRange(float dist) =>
        PlayerIsLeft() && dist <= AttackRange && dist > 0f;

    // wind up before attacking
    private void UpdateWindUp(EnemyState nextState)
    {
        if (stateTimer > 0.3f)
        {
            state = nextState;
            stateTimer = 0f;
            Attack();
        }
    }

    // after attack play idle
    private void UpdateAttackEnd()
    {
        if (stateTimer > 0.7f)
        {
            state = EnemyState.Idle;
            stateTimer = 0f;
        }
    }

    // idle pause
    private void UpdateIdle()
    {
        if (stateTimer > 1.5f)
        {
            if (Random.Shared.Next(4) == 0)
            {
                StartRetreat();
                return;
            }

            state = EnemyState.Walk;
            stateTimer = 0f;
        }
    }
    private void StartRetreat()
    {
        state = EnemyState.Retreat;
        stateTimer = 0f;

        // Random retreat duration between 0.5 and 1.2 seconds
        retreatDuration = Random.Shared.NextSingle() * 0.7f + 0.5f;
    }

    // when enemy takes a hit
    private void UpdateHitReaction()
    {
        if (player.IsWalking() && stateTimer > 0.4f)
        {
            state = EnemyState.Walk;
            stateTimer = 0f;
        }
    }
    
    private void UpdateRetreat(float dt)
    {
        // Move backwards to the right
        position.X += retreatSpeed * dt;

        if (stateTimer >= retreatDuration)
        {
            // After retreat, idle briefly
            state = EnemyState.Idle;
            stateTimer = 0f;
        }
    }


    public void Attack()
    {
        player.TakeDamage(10);
    }

    public void TakeDamage(double damage)
    {
        hp -= damage;

        if (hp > 0 && state != EnemyState.Hit)
        {
            state = EnemyState.Hit;
            stateTimer = 0f;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsDead && state != EnemyState.Defeated)
            return;

        current.Effects = SpriteEffects.FlipHorizontally;

        current.Origin = new Vector2(current.Width / 2f, 62f);
        current.Scale = new Vector2(2f, 2f);

        float offsetY = GetAnimationOffsetY() * current.Scale.Y;
        
        Vector2 drawPos = position + new Vector2(64, offsetY);

        current.Draw(spriteBatch, drawPos);
    }

    private float GetAnimationOffsetY() =>
        state switch
        {
            EnemyState.AboutToKick => -20f,
            EnemyState.Kicking => -20f,
            EnemyState.Hit => -5f,
            _ => 0f
        };

    public Circle GetBounds() =>
        new Circle(
            (int)(position.X + 64f),
            (int)(position.Y + 62f), // Same values as Vector2 drawPos = position + new Vector2(64, offsetY);
            50
        );
}
