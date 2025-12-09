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
using BeatThemUp.Utils;

namespace BeatThemUp;

public class Game1 : Core
{
    private SpriteBatch _spriteBatch;
    
    public static WindowMode WindowMode;

    public Game1() : base("Yakuza's Revenge", 1280, 720, false)
    {
        
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Start the game with the title scene.
        ChangeScene(new TitleScene());
        
        // Initialize the Gum UI service
        InitializeGum();

        // Apply saved settings
        ApplySavedSettings();
    }

    private void ApplySavedSettings()
    {
        // Load settings from XML
        XMLManager<SettingsData> xmlManager = new XMLManager<SettingsData>();
        SettingsData settings = xmlManager.Load("Content/xml/settings.xml");

        // Apply the loaded settings
        Core.Audio.SoundEffectVolume = settings.Volume.General;
        Core.Audio.SongVolume = settings.Volume.Music;
        
        SetWindowMode(settings.WindowMode);
    }
    
    /// <summary>
    /// This changes the window mode to either fullscreen or windowed.
    /// </summary>
    /// <param name="mode"></param>
    public static void SetWindowMode(WindowMode mode)
    {
        switch (mode)
        {
            case WindowMode.Windowed :
                Graphics.IsFullScreen = false;
                break;
            case WindowMode.Fullscreen :
                Graphics.IsFullScreen = true;
                break;
        }
        Graphics.ApplyChanges();
        WindowMode = mode;
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
    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        GumService.Default.Draw();
    }
}
