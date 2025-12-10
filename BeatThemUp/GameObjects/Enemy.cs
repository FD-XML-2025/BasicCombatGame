using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Enemy
{
    // enemy states
    private enum EnemyState
    {
        Walk,
        AboutToPunch,
        Punching,
        AboutToKick,
        Kicking,
        Idle,
        GettingHit,
        Defeated,
        Retreat
    }

    // animations
    // animations
    private AnimatedSprite walk, aboutToPunch, punching, aboutToKick, kicking, defeated, walkBack, idle, gettingHit;
    private AnimatedSprite current;

    // movement and player reference
    private Vector2 position;
    private Character player;
    private float speed = 100f;

    // health
    private double hp;

    // states
    private EnemyState state = EnemyState.Walk;
    private float stateTimer;

    // attack
    private const float AttackRange = 300f;
    private const float StopDistance = 300f;

    // retreat
    private bool retreating;
    private float retreatDuration;
    private float retreatSpeed = 150f; // faster backwards movement
    // is dead
    public bool IsDead => hp <= 0;

    // getter for hp
    public double GetHp() => hp;
    // x and y movement for enemy
    public Vector2 Position
    {
        get => position;
        set => position = value - new Vector2(current.Width / 2, current.Height / 2); // for centre of sprite
    }

    // constructor
    public Enemy(
        float hp,
        AnimatedSprite walk,
        AnimatedSprite aboutToPunch, AnimatedSprite punching,
        AnimatedSprite aboutToKick, AnimatedSprite kicking,
        AnimatedSprite gettingHit,
        AnimatedSprite defeated, AnimatedSprite walkBack, AnimatedSprite idle,
        Vector2 position, Character player)
    {
        this.hp = hp;
        this.walk = walk;

        this.aboutToPunch = aboutToPunch;
        this.punching = punching;

        this.aboutToKick = aboutToKick;
        this.kicking = kicking;

        this.gettingHit = gettingHit;   // Important

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
            EnemyState.AboutToPunch=> aboutToPunch,
            EnemyState.Punching    => punching,
            EnemyState.AboutToKick => aboutToKick,
            EnemyState.Kicking     => kicking,
            EnemyState.Idle        => idle,
            EnemyState.GettingHit  => gettingHit,
            EnemyState.Defeated    => defeated,
            EnemyState.Retreat     => walkBack,
            _ => walk
        };
    }

    // distance of player and enemy
    private float PlayerDist()
    {
        float enemyCenterX = position.X + 64f;
        float playerX = player.Position.X;

        return enemyCenterX - playerX;
    }
    // checks player is left
    private bool PlayerIsLeft()
    {
        float distance = PlayerDist();
        return distance > 0f;
    }
    // calls functions from enum states
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

            case EnemyState.AboutToPunch:
                UpdateWindUp(EnemyState.Punching);
                break;

            case EnemyState.AboutToKick:
                UpdateWindUp(EnemyState.Kicking);
                break;

            case EnemyState.Punching:
            case EnemyState.Kicking:
                UpdateAttackEnd();
                break;

            case EnemyState.Idle:
                UpdateIdle();
                break;

            case EnemyState.GettingHit:
                UpdateHitReaction();
                break;
            case EnemyState.Retreat:
                UpdateRetreat(dt);
                break;
        }

        UpdateCurrentSprite();
        current.Update(gameTime);
    }

    // death behavior *****have to test when player attacks is implemented*****
    private bool HandleDeath(GameTime gameTime)
    {
        if (hp > 0)
            return false;

        hp = 0;

        if (state != EnemyState.Defeated) // so doesn't reset
        {
            state = EnemyState.Defeated;
            stateTimer = 0f;
        }

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
                ? EnemyState.AboutToPunch
                : EnemyState.AboutToKick;
        }
    }

    private bool InAttackRange(float dist)
    {
        return PlayerIsLeft() && dist <= AttackRange && dist > 0f;
    }
    
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

    // after attack idle
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
            }
            else
            {
                state = EnemyState.Walk;
                stateTimer = 0f;
            }
        }
    }

    private void StartRetreat()
    {
        state = EnemyState.Retreat;
        stateTimer = 0f;
        retreatDuration = Random.Shared.NextSingle() + 0.5f;
    }

    private void UpdateRetreat(float dt)
    {
        // Move backwards to the right
        position.X += retreatSpeed * dt;

        if (stateTimer >= retreatDuration)
        {
            // After retreat idle briefly
            state = EnemyState.Idle;
            stateTimer = 0f;
        }
    }
    
    // when enemy takes a hit
    private void UpdateHitReaction()
    {
        if (stateTimer > 0.4f)       // Always return to idle, not only when player walks
        {
            state = EnemyState.Idle;
            stateTimer = 0f;
        }
    }
    
    public void Attack()
    {
        Console.WriteLine("[Enemy] ATTACK fired");
        player.TakeDamage(50);
    }

    public void TakeDamage(double damage)
    {
        hp -= damage;
        Console.WriteLine($"[Enemy] Took {damage}. HP now {hp}");

        if (hp <= 0)
        {
            state = EnemyState.Defeated;
            stateTimer = 0f;
            return;
        }

        state = EnemyState.GettingHit;
        stateTimer = 0f;
        current = gettingHit; // << play the animation here
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsDead && state != EnemyState.Defeated)
            return;

        current.Effects = SpriteEffects.FlipHorizontally; // to face left

        current.Origin = new Vector2(current.Width / 2f, 62f); // middle, feet pos
        current.Scale = new Vector2(2f, 2f);

        float offsetY = GetAnimationOffsetY() * current.Scale.Y;

        Vector2 drawPos = position + new Vector2(64, offsetY); // centre of 128px

        current.Draw(spriteBatch, drawPos);
    }

    
    // images aren't even so fixes that
    private float GetAnimationOffsetY()
    {
        return state switch
        {
            EnemyState.AboutToKick => -20f,
            EnemyState.Kicking => -20f,
            EnemyState.GettingHit => -5f,
            _                      => 0f
        };
    }
    public bool IsAttacking() => state == EnemyState.Punching || state == EnemyState.Kicking;
}
