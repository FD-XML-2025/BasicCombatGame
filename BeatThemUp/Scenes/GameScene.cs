using System;
using BeatThemUp.GameObjects;
using BeatThemUp.UI;
using BeatThemUp.Utils;
using GameDataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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
        GameOver,
        Win,
    }

    private int _kills;

    private float _damageDealt;
    
    private float _damageTaken;

    // Game timer
    private Timer _timer;

    // Reference to the player.
    private Player _player;

    private EnemyManager _enemyManager;
    
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
    
    private Texture2D _backgroundTexture;
    private Rectangle _backgroundRect;
    
    private const string THEME_SONG = "audio/theme-01";

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
        
        // Play theme song
        Song themeSong = Content.Load<Song>(THEME_SONG);
        Core.Audio.PlaySong(themeSong);
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
        
        _ui.WinRetryButtonClick += (s,e)=> InitializeNewGame();
        _ui.WinQuitButtonClick  += (s,e)=> Core.ChangeScene(new TitleScene());
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
        _ui.HideWinPanel();
        _ui.HideGameOverPanel();

        // Reset the game stats
        _kills = 0;
        _damageTaken = 0;
        _damageDealt = 0;
        
        // Default player position
        Vector2 playerPos = new Vector2(0, 340f);
        
        // Initialize the player
        _player.Initialize(playerPos);
        _player.Heal(_player.MaxHealth);
        
        // Update game stats when player take damage
        _player.OnTakeDamageEvent += (damage) => _damageTaken += damage;
        
        // Call game over when player die
        _player.OnDeathEvent += () => GameEnd(true);

        // Update player healthbar when health change
        _player.OnHealthChangeEvent += UpdatePlayerHealth;

        _enemyManager = new EnemyManager(_player);
        _player.SetEnemyManager(_enemyManager);

        // Reset the score
        _score = 0;

        // Set the game state to playing
        _state = GameState.Playing;
        
        // Init and start timer
        _timer = new Timer(LEVEL_DURATION);
        _timer.Start();
        
        // Tiger atlas (NEW)
        TextureAtlas tigerAtlas = TextureAtlas.FromFile(Core.Content, "images/atlas-tiger.xml");
        TextureAtlas tigerIdleAtlas = TextureAtlas.FromFile(Core.Content, "images/atlas-tiger-idle.xml");
        
        
        // Tiger animations
        // Tiger animations
        AnimatedSprite tigerWalk = tigerAtlas.CreateAnimatedSprite("tiger_walk");
        AnimatedSprite tigerPunch = tigerAtlas.CreateAnimatedSprite("tiger_punch");       // punch
        AnimatedSprite tigerKick = tigerAtlas.CreateAnimatedSprite("tiger_kick");        // kick
        AnimatedSprite tigerHit = tigerAtlas.CreateAnimatedSprite("tiger_hit");         // << getting hit animation
        AnimatedSprite tigerDefeated = tigerAtlas.CreateAnimatedSprite("tiger_defeated");    // dead
        AnimatedSprite tigerIdle = tigerIdleAtlas.CreateAnimatedSprite("tiger_idle");    // idle
        AnimatedSprite tigerWalkBack = tigerAtlas.CreateAnimatedSprite("tiger_walk_back");   // retreat

        
        var tiger = new Enemy(
            tigerWalk,
            tigerPunch,      // aboutToPunch
            tigerPunch,      // punching
            tigerKick,       // aboutToKick
            tigerKick,       // kicking
            tigerHit,        // getting hit animation
            tigerDefeated,
            tigerWalkBack,
            tigerIdle,
            new Vector2(1100f, 550f),
            _player
        );
        _enemyManager.AddEnemy(tiger);
    }

    private void UpdatePlayerHealth()
    {
        _ui.SetHealthBar(_player.Health / _player.MaxHealth);
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
        
        _backgroundTexture = Content.Load<Texture2D>("images/BackgroundImage");
        _backgroundRect = Core.GraphicsDevice.PresentationParameters.Bounds;

        TextureAtlas charactersAtlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition-characters.xml");
        TextureAtlas charactersAtlasAttack = TextureAtlas.FromFile(Core.Content, "images/atlas-definition-characters-attack.xml");
        
        // Create the animated sprite for the player from the atlas.
        AnimatedSprite playerIdleAnimation = charactersAtlas.CreateAnimatedSprite("yakuza-male-idle");
        AnimatedSprite playerWalkAnimation = charactersAtlas.CreateAnimatedSprite("yakuza-male-walk");
        
        AnimatedSprite playerAttack1    = charactersAtlasAttack.CreateAnimatedSprite("yakuza-male-attack1");
        AnimatedSprite playerAttack2    = charactersAtlasAttack.CreateAnimatedSprite("yakuza-male-attack2");
        AnimatedSprite playerHit1       = charactersAtlasAttack.CreateAnimatedSprite("yakuza-male-hit1");
        AnimatedSprite playerHit2       = charactersAtlasAttack.CreateAnimatedSprite("yakuza-male-hit2");
        AnimatedSprite playerKnockdown  = charactersAtlasAttack.CreateAnimatedSprite("yakuza-male-knockdown");
        
        // Create the player
        _player = new Player(
            playerIdleAnimation,     // idle
            playerWalkAnimation,     // walk
            playerAttack1,           // attack 1
            playerAttack2,           // attack 2
            playerHit1,              // hit reaction 1
            playerHit2,              // hit reaction 2
            playerKnockdown          // death/knockdown
        );
        _player.SetEnemyManager(_enemyManager);

        // Load the bounce sound effect for the bat
        SoundEffect bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the collect sound effect
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the grayscale effect
        _grayscaleEffect = Content.Load<Effect>("effects/grayscaleEffect");
        
        TextureManager.Init(Core.GraphicsDevice);
    }

    public override void Update(GameTime gameTime)
    {
        // UI is always updated
        _ui.Update(gameTime);
        
        var enemy = _enemyManager.FirstEnemy;
        if (enemy == null && _state == GameState.Playing)
        {
            GameEnd(false);
        }


        if (_state != GameState.Playing)
        {
            _saturation = Math.Max(0.0f, _saturation - FADE_SPEED);
            return; // only block gameplay when game over
        }
        else if (_state == GameState.Win)
        {
            _saturation = Math.Max(0.0f, _saturation - FADE_SPEED);
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

        // Update the timer
        _enemyManager.Update(gameTime);
        
        // Wall so player cannot pass the enemies
        var tiger = _enemyManager.FirstEnemy;

        if (tiger != null)
        {
            float tigerFront = tiger.Position.X - 230f; // spacing for wall

            if (_player.Position.X > tigerFront)
            {
                _player.Position = new Vector2(tigerFront, _player.Position.Y);
            }
        }

        // Update timer and its HUD when running
        if (_timer.IsRunning())
        {
            _timer.Update(gameTime);
            _ui.UpdateTimerText((int)_timer.GetRemainingTime());   
        }

        // Game over if timer is finished
        if (_timer.IsFinished())
        {
            GameEnd(true);
        }

        // Perform collision checks
        CollisionChecks();
    }

    private void CollisionChecks()
    {
        var enemy = _enemyManager.FirstEnemy;
        if (enemy == null)
            return;

        // 1D distance on X axis
        float distance = Math.Abs(enemy.Position.X - _player.Position.X);
        const float hitRange = 300f;
        
        // Capture the current bounds of the player
        //Circle playerBounds = _player.GetBounds();

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

    private void GameEnd(bool isGameOver)
    {
        if (isGameOver)
            GameOver();
        else
            GameWon();

        SaveGame(isGameOver);
    }

    private void SaveGame(bool isGameOver)
    {
        XMLManager<SaveData> xmlManager = new XMLManager<SaveData>();
        SaveData save = xmlManager.Load("Content/xml/save.xml");
        var newSave = new SaveData()
        {
            Kills = save.Kills + _kills,
            Wins = save.Wins + (!isGameOver ? 1 : 0),
            Loses = save.Loses + (isGameOver ? 1 : 0),
            DamageDealt = save.DamageDealt + (int)_damageDealt,
            DamageTaken = save.DamageTaken + (int)_damageTaken
        };
        xmlManager.Save("Content/xml/save.xml", newSave);
    }

    private void GameWon()
    {
        _state = GameState.Win;
        _ui.ShowWinPanel();
        _saturation = 1.0f;
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
        Core.SpriteBatch.Draw(_backgroundTexture, _backgroundRect, Color.White);
        // Draw the player.
        _player.Draw();
        
        _enemyManager.Draw(Core.SpriteBatch);

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        // Draw the UI
        _ui.Draw();
    }
}
