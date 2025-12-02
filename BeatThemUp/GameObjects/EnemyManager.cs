namespace BeatThemUp.GameObjects;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class EnemyManager
{
    private readonly List<Ennemie> _enemies = new List<Ennemie>();
    private readonly Character _player;
    public Ennemie FirstEnemy => _enemies.Count > 0 ? _enemies[0] : null;

    
    public EnemyManager(Character player)
    {
        _player = player;
    }

    public void AddEnemy(Ennemie enemy)
    {
        _enemies.Add(enemy);
    }

    public void Update(GameTime gameTime)
    {
        // Update all enemies
        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime);   // uses logic from Ennemie.cs :contentReference[oaicite:0]{index=0}
        }

        // Remove defeated enemies
        _enemies.RemoveAll(e => e.IsDead);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }

    public IReadOnlyList<Ennemie> Enemies => _enemies;
}