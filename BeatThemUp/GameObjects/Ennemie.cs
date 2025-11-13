using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeatThemUp.GameObjects;

public class Ennemie
{
    private Texture2D textureWalk;
    private Texture2D textureHit;
    private Texture2D textureAboutToHit;
    private Texture2D textureHitting;
    private Texture2D textureAboutToKick;
    private Texture2D textureKicking;
    private Texture2D textureDefeated;
    private Texture2D currentTexture; // What animation currently being shown like if tiger is resting 
    private Vector2 position;
    private Vector2 velocity;
    private Joueur joueur;
    private float speed;
    private double hp;
    private string type;
    private string currentState; // Tracks the current animation state
    private float stateTimer; // Timer to switch states
    private const float StateDuration = 0.5f; // Duration 0.5secs

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

    public Ennemie(double hp, string type, Texture2D textureWalk, Texture2D textureHit,
                   Texture2D textureAboutToHit, Texture2D textureHitting,
                   Texture2D textureAboutToKick, Texture2D textureKicking,
                   Texture2D textureDefeated, Vector2 position, Joueur joueur)
    {
        this.hp = hp;
        this.type = type;
        this.textureWalk = textureWalk;
        this.textureHit = textureHit;
        this.textureAboutToHit = textureAboutToHit;
        this.textureHitting = textureHitting;
        this.textureAboutToKick = textureAboutToKick;
        this.textureKicking = textureKicking;
        this.textureDefeated = textureDefeated;
        this.position = position;
        this.joueur = joueur;
        this.speed = 2f;
        this.velocity = Vector2.Zero;
        this.currentState = "walk"; // Resting
        this.stateTimer = 0f;
        UpdateCurrentTexture();
    }

    private void UpdateCurrentTexture()
    {
        if (currentState == "walk")
        {
            currentTexture = textureWalk;
        }
        else if (currentState == "hit")
        {
            currentTexture = textureHit;
        }
        else if (currentState == "about_to_hit")
        {
            currentTexture = textureAboutToHit;
        }
        else if (currentState == "hitting")
        {
            currentTexture = textureHitting;
        }
        else if (currentState == "about_to_kick")
        {
            currentTexture = textureAboutToKick;
        }
        else if (currentState == "kicking")
        {
            currentTexture = textureKicking;
        }
        else if (currentState == "defeated")
        {
            currentTexture = textureDefeated;
        }
        else
        {
            currentTexture = textureWalk; // Default to walk if state is invalid
        }
    }

    public void Update(GameTime gameTime, Joueur player)
    {
        stateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (stateTimer >= StateDuration)
        {
            stateTimer = 0f; // resets timer
            if (IsDead)
            {
                currentState = "defeated";
            }
            else if (Vector2.Distance(position, player.Position) < 50 && currentState != "hitting" && currentState != "kicking")
            {
                currentState = (Random.Shared.Next(2) == 0) ? "about_to_hit" : "about_to_kick";
            }
            else if (currentState == "about_to_hit" || currentState == "about_to_kick")
            {
                currentState = currentState == "about_to_hit" ? "hitting" : "kicking"; // If adding more attacks fix this 
                if (currentState == "hitting" || currentState == "kicking")
                {
                    Attack(player);
                }
            }
            else if (currentState == "hitting" || currentState == "kicking")
            {
                currentState = "walk";
            }
            else if (!IsDead)
            {
                currentState = "walk";
            }
            UpdateCurrentTexture();
        }

        // Move toward the player
        Vector2 direction = player.Position - position;
        if (direction.LengthSquared() > 0 && currentState == "walk")
        {
            direction.Normalize();
            velocity = direction * speed;
        }
        else
        {
            velocity = Vector2.Zero; // Stop moving during attack or hit states
        }

        position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 60; // IDK what fps we're running

        // Transition from "about_to_hit" or "about_to_kick" to "hitting" or "kicking" after a short delay
        if ((currentState == "about_to_hit" || currentState == "about_to_kick") && stateTimer >= StateDuration / 2)
        {
            currentState = currentState == "about_to_hit" ? "hitting" : "kicking";
            UpdateCurrentTexture();
            if (currentState == "hitting" || currentState == "kicking")
            {
                Attack(player);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsDead || currentState == "defeated") // enemy doesn't actually disappear
        {
            spriteBatch.Draw(currentTexture, position, Color.White);
        }
    }

    public void Attack(Joueur player)
    {
        player.TakeDamage(10); // need to adjust damage separate for kicks and hits
    }

    public void TakeDamage(double damage)
    {
        hp -= damage;
        if (hp > 0 && currentState != "hit") // Only change to hit state if not already dead
        {
            currentState = "hit";
            stateTimer = 0f;
            UpdateCurrentTexture();
        }
    }
}