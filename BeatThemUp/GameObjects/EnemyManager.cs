namespace BeatThemUp.GameObjects;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class EnemyManager
{
    private readonly List<Ennemie> _enemies = new List<Ennemie>();
    private readonly Character _player;
    public Ennemie? FirstEnemy
    {
        get
        {
            if (_enemies.Count > 0)
                return _enemies[0];
            else
                return null;
        }
    }
    
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
        // update all enemies
        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].Update(gameTime);
        }
        
        // remove defeated enemies
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (_enemies[i].IsDead)
            {
                _enemies.RemoveAt(i);
            }
        }

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }

    public IReadOnlyList<Ennemie> Enemies
    {
        get
        {
            return _enemies;
        }
    }
}