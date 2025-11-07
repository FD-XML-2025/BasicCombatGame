using System;
using BeatThemUp.GameObjects;
using BeatThemUp.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameLibrary;

namespace BeatThemUp;

public class Game1 : Core
{
    // The background theme song
    private Song _themeSong;
    
    private SpriteBatch spriteBatch;
    private Joueur player;
    private Ennemie tiger;
    

    public Game1() : base("Beat Them Up!", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();
        
        spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Start the game with the title scene.
        ChangeScene(new TitleScene());
        
        // Initialize the Gum UI service
        InitializeGum();

        // Load the background theme music
        _themeSong = Content.Load<Song>("audio/theme-03");
    
        // Start playing the background music
        Audio.PlaySong(_themeSong);
    }

    private void InitializeGum()
    {
        // Initialize the Gum service
        GumService.Default.Initialize(this);

        // Tell the Gum service which content manager to use.  We will tell it to
        // use the global content manager from our Core.
        GumService.Default.ContentLoader.XnaContentManager = Core.Content;

        // Register keyboard input for UI control.
        FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);

        // Register gamepad input for Ui control.
        FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);

        // Customize the tab reverse UI navigation to also trigger when the keyboard
        // Up arrow key is pushed.
        FrameworkElement.TabReverseKeyCombos.Add(
           new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Up });

        // Customize the tab UI navigation to also trigger when the keyboard
        // Down arrow key is pushed.
        FrameworkElement.TabKeyCombos.Add(
           new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Down });

        // The assets created for the UI were done so at 1/4th the size to keep the size of the
        // texture atlas small.  So we will set the default canvas size to be 1/4th the size of
        // the game's resolution then tell gum to zoom in by a factor of 4.
        GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
        GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
        GumService.Default.Renderer.Camera.Zoom = 4.0f;
    }

    protected override void LoadContent()
    {
        
        player = new Joueur(100, 10);
        player.LoadContent(Content);
    
        Texture2D tigerWalk = Content.Load<Texture2D>("images/Tiger_Enemy/RCG_Walk_09");
        Texture2D tigerHit = Content.Load<Texture2D>("images/Tiger_Enemy/RCG_Idle_10");
        Texture2D tigerAboutToHit = Content.Load<Texture2D>("images/Tiger_Enemy/RCG_Attack_Chop_03");
        Texture2D tigerHitting = Content.Load<Texture2D>("images/Tiger_Enemy/RCG_Attack_Chop_04");
        Texture2D tigerAboutToKick = Content.Load<Texture2D>("images/Tiger_Enemy/RCG_Attack_Boot_05");
        Texture2D tigerKicking = Content.Load<Texture2D>("images/Tiger_Enemy/RCG_Attack_Boot_06");
        Texture2D tigerDefeated = Content.Load<Texture2D>("images/Tiger_Enemy/RCG_Idle_10");
        
        if (tigerWalk == null) throw new Exception("tigerWalk texture failed to load");
        if (tigerHit == null) throw new Exception("tigerHit texture failed to load");
        if (tigerAboutToHit == null) throw new Exception("tigerAboutToHit texture failed to load");
        if (tigerHitting == null) throw new Exception("tigerHitting texture failed to load");
        if (tigerAboutToKick == null) throw new Exception("tigerAboutToKick texture failed to load");
        if (tigerKicking == null) throw new Exception("tigerKicking texture failed to load");
        if (tigerDefeated == null) throw new Exception("tigerDefeated texture failed to load");
        
        tiger = new Ennemie(
            100,
            "Tiger",
            tigerWalk, tigerHit, tigerAboutToHit, tigerHitting,
            tigerAboutToKick, tigerKicking, tigerDefeated,
            new Vector2(300, 300),
            player
        );

    }

    protected override void Update(GameTime gameTime)
    {
        player.Update();
        tiger.Update(gameTime, player);
        base.Update(gameTime);
        Console.WriteLine($"Tiger position: {tiger.Position}, Alive: {tiger.IsAlive}");

    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        player.Draw(spriteBatch);
        tiger.Draw(spriteBatch);

        spriteBatch.End();
    
    }
}
