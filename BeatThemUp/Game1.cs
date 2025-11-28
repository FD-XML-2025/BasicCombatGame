using BeatThemUp.GameObjects;
using BeatThemUp.Scenes;
using GameDataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameLibrary;
using System;
using System.IO;
using System.Xml.Serialization;

namespace BeatThemUp;

public class Game1 : Core
{
    // The background theme song
    private Song _themeSong;
    
    private SpriteBatch spriteBatch;

    public Game1() : base("Yakuza's Revenge", 1280, 720, false)
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

        // Apply saved settings
        ApplySavedSettings();

        // Load the background theme music
        _themeSong = Content.Load<Song>("audio/theme-03");
    
        // Start playing the background music
        Audio.PlaySong(_themeSong);
    }

    private void ApplySavedSettings()
    {
        SettingsData settings;

        // Init settings file if it doesn't exist
        if (!File.Exists("settings.xml"))
        {
            settings = new SettingsData()
            {
                MasterVolume = 1f,
                MusicVolume = 1f
            };
            using (TextWriter writer = new StreamWriter("settings.xml"))
            {
                var xml = new XmlSerializer(typeof(SettingsData));
                xml.Serialize(writer, settings);
            }
        }

        // Load settings from XML
        using (TextReader reader = new StreamReader("settings.xml"))
        {
            var xml = new XmlSerializer(typeof(SettingsData));
            settings = (SettingsData)xml.Deserialize(reader);
        }

        // Apply the loaded settings
        Core.Audio.SoundEffectVolume = settings.MasterVolume;
        Core.Audio.SongVolume = settings.MusicVolume;
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
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        spriteBatch.End();
    
    }
}
