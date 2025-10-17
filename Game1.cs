using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;

namespace BasicCombatGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Character _testcharacter;
    private Vector2 _playerPosition;

    public Game1()
    {
        // permet de lier le jeu (classe Game et dérivées) au
        // gestionnaire de périphériques graphiques positions, calculs, mouvements
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _testcharacter = new Character("SpriteSheet/female-yakuza", "yakuza-female");

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // TODO: use this.Content to load your game content here
        _testcharacter.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    // une callback qui permet d’actualiser les évènements
    // d’entrée (clavier, souris).
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Right))
            _playerPosition.X += 3;
        if (keyboard.IsKeyDown(Keys.Left))
            _playerPosition.X -= 3;
        if (keyboard.IsKeyDown(Keys.Up))
            _playerPosition.Y -= 3;
        if (keyboard.IsKeyDown(Keys.Down))
            _playerPosition.Y += 3;
        
        _testcharacter.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        _testcharacter.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}