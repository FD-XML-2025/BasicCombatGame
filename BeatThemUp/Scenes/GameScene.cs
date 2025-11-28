using System;
using BeatThemUp.GameObjects;
using BeatThemUp.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;

namespace BeatThemUp.Scenes;

public class GameScene : Scene
{
    private enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    // Game timer
    private Timer _timer;

    // Reference to the player.
    private Player _player;
    
    private Ennemie tiger;

    // The sound effect to play when the player ...
    private SoundEffect _collectSoundEffect;

    // Tracks the players score.
    private int _score;

    private GameSceneUI _ui;

    private GameState _state;

    // The grayscale shader effect.
    private Effect _grayscaleEffect;

    // The amount of saturation to provide the grayscale shader effect
    private float _saturation = 1.0f;

    // The speed of the fade to grayscale effect.
    private const float FADE_SPEED = 0.02f;

    // The level duration in seconds
    private const float LEVEL_DURATION = 5 * 60f;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;

        // Subscribe to the player's BodyCollision event so that a game over
        // can be triggered when this event is raised.
        //_player.BodyCollision += OnSlimeBodyCollision;

        // Create any UI elements from the root element created in previous
        // scenes
        GumService.Default.Root.Children.Clear();

        // Initialize the user interface for the game scene.
        InitializeUI();

        // Initialize a new game to be played.
        InitializeNewGame();
    }

    private void InitializeUI()
    {
        // Clear out any previous UI element incase we came here
        // from a different scene.
        GumService.Default.Root.Children.Clear();

        // Create the game scene ui instance.
        _ui = new GameSceneUI();

        // Subscribe to the events from the game scene ui.
        _ui.ResumeButtonClick += OnResumeButtonClicked;
        _ui.RetryButtonClick += OnRetryButtonClicked;
        _ui.QuitButtonClick += OnQuitButtonClicked;
    }

    private void OnResumeButtonClicked(object sender, EventArgs args)
    {
        // Change the game state back to playing
        _state = GameState.Playing;
    }

    private void OnRetryButtonClicked(object sender, EventArgs args)
    {
        // Player has chosen to retry, so initialize a new game
        InitializeNewGame();
    }

    private void OnQuitButtonClicked(object sender, EventArgs args)
    {
        // Player has chosen to quit, so return back to the title scene
        Core.ChangeScene(new TitleScene());
    }

    private void InitializeNewGame()
    {
        // Calculate the position for the player
        Vector2 playerPos = new Vector2(0f, 400f);

        // Initialize the player
        _player.Initialize(playerPos);

        // Reset the score
        _score = 0;

        // Set the game state to playing
        _state = GameState.Playing;
        
        // Init and start timer
        _timer = new Timer(LEVEL_DURATION);
        _timer.Start();
        
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
        
        /*tiger = new Ennemie(
            100,
            "Tiger",
            tigerWalk, tigerHit, tigerAboutToHit, tigerHitting,
            tigerAboutToKick, tigerKicking, tigerDefeated,
            new Vector2(300, 300),
            _player
        );*/
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
        
        TextureAtlas charactersAtlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition-characters.xml");

        // Create the animated sprite for the player from the atlas.
        AnimatedSprite playerAnimation = charactersAtlas.CreateAnimatedSprite("yakuza-male-idle");
        AnimatedSprite playerAnimationWalk = charactersAtlas.CreateAnimatedSprite("yakuza-male-walk");

        // Create the player
        _player = new Player(playerAnimation, playerAnimationWalk);

        // Load the bounce sound effect for the bat
        SoundEffect bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the collect sound effect
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the grayscale effect
        _grayscaleEffect = Content.Load<Effect>("effects/grayscaleEffect");
    }

    public override void Update(GameTime gameTime)
    {
        // Ensure the UI is always updated
        _ui.Update(gameTime);

        if (_state != GameState.Playing)
        {
            // The game is in either a paused or game over state, so
            // gradually decrease the saturation to create the fading grayscale.
            _saturation = Math.Max(0.0f, _saturation - FADE_SPEED);

            // If its just a game over state, return back
            if (_state == GameState.GameOver)
            {
                return;
            }
        }

        // If the pause button is pressed, toggle the pause state
        if (GameController.Pause())
        {
            TogglePause();
        }

        // At this point, if the game is paused, just return back early
        if (_state == GameState.Paused)
        {
            return;
        }

        // Update the player;
        _player.Update(gameTime);
        
        //tiger.Update(gameTime);
        
        // Update the timer
        _timer.Update(gameTime);
        _ui.UpdateTimerText((int)_timer.GetRemainingTime());

        if (_timer.IsFinished())
        {
            GameOver();
        }

        // Perform collision checks
        CollisionChecks();
    }

    private void CollisionChecks()
    {
        // Capture the current bounds of the player
        Circle playerBounds = _player.GetBounds();

        // First perform a collision check to see if the player is colliding with something
        /*if (slimeBounds.Intersects(batBounds))
        {
            // Increment the score.
            _score += 100;

            // Update the score display on the UI.
            _ui.UpdateScoreText(_score);

            // Play the collect sound effect
            Core.Audio.PlaySoundEffect(_collectSoundEffect);
        }*/
    }

    private void TogglePause()
    {
        if (_state == GameState.Paused)
        {
            // We're now unpausing the game, so hide the pause panel
            _ui.HidePausePanel();

            // And set the state back to playing
            _state = GameState.Playing;
            
            // Unpause timer
            _timer.Start();
        }
        else
        {
            // Pause the timer
            _timer.Stop();
            
            // We're now pausing the game, so show the pause panel
            _ui.ShowPausePanel();

            // And set the state to paused
            _state = GameState.Paused;

            // Set the grayscale effect saturation to 1.0f;
            _saturation = 1.0f;
        }
    }

    private void GameOver()
    {
        // Show the game over panel
        _ui.ShowGameOverPanel();

        // Set the game state to game over
        _state = GameState.GameOver;

        // Set the grayscale effect saturation to 1.0f;
        _saturation = 1.0f;
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        if (_state != GameState.Playing)
        {
            // We are in a game over state, so apply the saturation parameter.
            _grayscaleEffect.Parameters["Saturation"].SetValue(_saturation);

            // And begin the sprite batch using the grayscale effect.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _grayscaleEffect);
        }
        else
        {
            // Otherwise, just begin the sprite batch as normal.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        }

        // Draw the player.
        _player.Draw();
        
        //tiger.Draw(spriteBatch);

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        // Draw the UI
        _ui.Draw();
    }
}
