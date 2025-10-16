using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BasicCombatGame;

public class Joueur
{
    private Texture2D _playerTexture;   // ton image
    private Vector2 _playerPosition;
    private double hp { get; set; }
    private double dmg { get; set; }

    public Joueur(double hp, double dmg)
    {
        this.hp = hp;
        this.dmg = dmg;
        // Position initiale
        _playerPosition = new Vector2(100, 100);
    }

    public void LoadContent(ContentManager content)
    {
        // TODO: use this.Content to load your game content here

        // Charger ton sprite (nom sans extension)
        _playerTexture = content.Load<Texture2D>("RCG2_Selfie_01");
    }
    
    public void Update()
    {
        var keyboard = Keyboard.GetState();
        
        if (keyboard.IsKeyDown(Keys.Right))
            _playerPosition.X += 3;
        if (keyboard.IsKeyDown(Keys.Left))
            _playerPosition.X -= 3;
        if (keyboard.IsKeyDown(Keys.Up))
            _playerPosition.Y -= 3;
        if (keyboard.IsKeyDown(Keys.Down))
            _playerPosition.Y += 3;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        // TODO: Add your drawing code here
        spriteBatch.Draw(_playerTexture, _playerPosition, Color.White);
    }

    public void Attack()
    {
        
    }

    public void TakeDamage()
    {
        
    }
}