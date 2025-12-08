using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Player : Character
{
    private enum PlayerState
    {
        Idle,
        Walking,
        Attacking,
        Hit,
        KnockedDown
    }
    private PlayerState _state = PlayerState.Idle;
    private float _stateTimer;

    // Speed pixels/s
    private float _moveSpeed = 250f;
    private bool _hasDealtDamageThisAttack;
    private float _attackCooldown;

    private AnimatedSprite _idleSprite;
    
    private AnimatedSprite _walkSprite;
    
    private AnimatedSprite _attack1Sprite;
    private AnimatedSprite _attack2Sprite;
    private AnimatedSprite _hit1Sprite;
    private AnimatedSprite _hit2Sprite;
    private AnimatedSprite _knockdownSprite;
    
    private EnemyManager _enemyManager;

    public float Damage { get; private set; } = 10f;


    public Player(AnimatedSprite sprite, AnimatedSprite walkSprite, AnimatedSprite attack1Sprite,
        AnimatedSprite attack2Sprite, AnimatedSprite hit1Sprite,
        AnimatedSprite hit2Sprite, AnimatedSprite knockdownSprite) : base(sprite)
    {
        // Store the used sprites
        _idleSprite = sprite;
        _walkSprite = walkSprite;
        
        _attack1Sprite = attack1Sprite;
        _attack2Sprite = attack2Sprite;
        _hit1Sprite = hit1Sprite;
        _hit2Sprite = hit2Sprite;
        _knockdownSprite = knockdownSprite;
        
        Sprite = _idleSprite;
    }

    public void Initialize(Vector2 startingPosition)
    {
        // Setup the player position
        Position = startingPosition - new Vector2(_idleSprite.Width / 2, _idleSprite.Height / 2);
    }
    
    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _stateTimer += dt;

        if (_attackCooldown > 0f)
            _attackCooldown -= dt;

        // Handle movement ONLY if not in combat animations
        if (_state != PlayerState.Attacking && _state != PlayerState.Hit && _state != PlayerState.KnockedDown)
            HandleInput();

        UpdateState(gameTime);
        Sprite.Update(gameTime);

        base.Update(gameTime);
    }

    // Override OnStartMove event to set the walk sprite
    public override void OnStartMove()
    {
        if (_state == PlayerState.Attacking || _state == PlayerState.Hit || _state == PlayerState.KnockedDown)
            return;  
        base.OnStartMove();

        Sprite = _walkSprite;
    }

    // Override OnStopMove event to set the idle sprite
    public override void OnStopMove()
    {
        if (_state == PlayerState.Attacking || _state == PlayerState.Hit || _state == PlayerState.KnockedDown)
            return;  
        base.OnStopMove();
        
        Sprite = _idleSprite;
    }

    public override void Draw()
    {
        base.Draw();
    }

    private void HandleInput()
    {
        // Movement/attack disabled while animation playing to stop spamming and unfinished animations
        if (_state == PlayerState.Attacking ||
            _state == PlayerState.Hit ||
            _state == PlayerState.KnockedDown)
            return;

        Velocity = Vector2.Zero;

        if (GameController.MoveBackward())
            Velocity.X = -_moveSpeed;

        if (GameController.MoveForward())
            Velocity.X = _moveSpeed;

        // Attack input
        if (GameController.Attack() && _attackCooldown <= 0f)
        {
            Attack();
            return;
        }


        // Only updating movement if not attacking
        _state = Velocity.X != 0 ? PlayerState.Walking : PlayerState.Idle;
        Sprite = _state == PlayerState.Walking ? _walkSprite : _idleSprite;
    }

    private void Attack()
    {
        if (_state == PlayerState.Attacking || _state == PlayerState.KnockedDown)
            return;

        _state = PlayerState.Attacking;
        _stateTimer = 0f;

        int atk = Random.Shared.Next(3);
        Sprite = atk switch
        {
            0 => _attack1Sprite,
            _ => _attack2Sprite
        };

        _hasDealtDamageThisAttack = false;
    }
    
    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);

        Console.WriteLine($"[Player] Took {dmg}"); // Just debugging

        if (Health <= 0)
        {
            Knockdown();
            return;
        }

        if (_state == PlayerState.Hit || _state == PlayerState.Attacking)
            return;

        _state = PlayerState.Hit;
        _stateTimer = 0f;

        Sprite = Random.Shared.Next(2) == 0 ? _hit1Sprite : _hit2Sprite;
    }


    private void Knockdown()
    {
        _state = PlayerState.KnockedDown;
        _stateTimer = 0f;

        Sprite = _knockdownSprite;
    }

    // Enemy-style timed state behaviour
    private void UpdateState(GameTime gameTime)
    {
        switch (_state)
        {
            case PlayerState.Attacking:
                Velocity = Vector2.Zero;
                TryDealDamageToEnemy();

                if (_stateTimer > 0.45f)
                {
                    _state = PlayerState.Idle;
                    Sprite = _idleSprite;
                    _attackCooldown = 0.1f;
                }
                break;

            case PlayerState.Hit:
                if (_stateTimer > 0.35f)
                {
                    _state = PlayerState.Idle;
                    Sprite = _idleSprite;
                }
                break;

            case PlayerState.KnockedDown:
                return;
        }
    }
    private void TryDealDamageToEnemy()
    {
        if (_hasDealtDamageThisAttack || _enemyManager == null) return;

        var enemy = _enemyManager.FirstEnemy;
        if (enemy == null) return;

        float dist = Math.Abs(enemy.Position.X - Position.X);
        if (dist <= 250f)
        {
            Console.WriteLine("[ATTACK HIT] -10 HP");
            enemy.TakeDamage(Damage);
            _hasDealtDamageThisAttack = true;
        }
    }

    public void SetEnemyManager(EnemyManager manager)
    {
        _enemyManager = manager;
    }
    public bool IsAttacking()
    {
        return _state == PlayerState.Attacking;
    }

    public bool IsAlive() => _state != PlayerState.KnockedDown;

}