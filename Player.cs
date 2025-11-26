using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Apo_CHAOS_lypse;

public class Player : ScaledSprite
{
    private const float Gravity = 1200f;
    private const float MoveSpeed = 250f;
    private const float JumpStrength = 460f;
    private const float DashSpeed = 650f;
    private const float DashDuration = 0.15f;
    private const float DashCooldown = 0.65f;

    private float _dashTimer;
    private float _dashCooldownTimer;
    private int _jumpsRemaining;
    private bool _jumpHeldLastFrame;

    public Vector2 Velocity { get; private set; }
    public bool OnGround { get; private set; }
    public bool FacingLeft { get; private set; }
    public bool Strafing { get; private set; }
    public Weapon CurrentWeapon { get; set; } = null!;

    public Player(Texture2D texture, Vector2 position, Color color, Vector2 size) : base(texture, position, color, size)
    {
        _jumpsRemaining = 2;
    }

    public override void Update(GameTime gameTime)
    {
        // Managed via Update(GameTime, ...)
    }

    public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState,
                    IReadOnlyList<Platform> platforms, List<Projectile> projectiles,
                    IReadOnlyList<Enemy> enemies)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var moveInput = 0f;

        if (keyboardState.IsKeyDown(Keys.A))
            moveInput -= 1f;
        if (keyboardState.IsKeyDown(Keys.D))
            moveInput += 1f;

        Strafing = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
        if (!Strafing && Math.Abs(moveInput) > 0.01f)
        {
            FacingLeft = moveInput < 0;
        }

        // === USAR VARIABLE LOCAL ===
        var velocity = Velocity;

        velocity.X = moveInput * MoveSpeed;

        HandleDashing(dt, keyboardState);
        HandleJumping(keyboardState, ref velocity);

        if (_dashTimer > 0)
        {
            velocity = new Vector2((FacingLeft ? -1 : 1) * DashSpeed, 0);
        }
        else
        {
            velocity.Y += Gravity * dt;
        }

        Velocity = velocity; // reasignás la propiedad completa

        ApplyMovement(dt, platforms);
        if (CurrentWeapon != null && CurrentWeapon.TryFire(gameTime, this, mouseState, projectiles, enemies, out var shake))
        {
            PendingShake = Math.Max(PendingShake, shake);
        }    
    }


    private void HandleJumping(KeyboardState keyboardState, ref Vector2 velocity)
    {
        var jumpPressed = keyboardState.IsKeyDown(Keys.Space);
        if (jumpPressed && !_jumpHeldLastFrame && _jumpsRemaining > 0)
        {
            velocity.Y = -JumpStrength;
            _jumpsRemaining--;
            OnGround = false;
        }

        _jumpHeldLastFrame = jumpPressed;
    }

    private void HandleDashing(float dt, KeyboardState keyboardState)
    {
        if (_dashCooldownTimer > 0)
            _dashCooldownTimer -= dt;

        if (_dashTimer > 0)
        {
            _dashTimer -= dt;
        }

        if (_dashCooldownTimer <= 0 && keyboardState.IsKeyDown(Keys.LeftControl))
        {
            _dashTimer = DashDuration;
            _dashCooldownTimer = DashCooldown;
        }
    }

    private void ApplyMovement(float dt, IReadOnlyList<Platform> platforms)
    {
        OnGround = false;
        var newPosition = Position;

        // Horizontal
        newPosition.X += Velocity.X * dt;
        var horizontalBox = new Rectangle((int)newPosition.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        foreach (var platform in platforms)
        {
            if (!platform.Bounds.Intersects(horizontalBox))
                continue;

            if (platform.PassThrough)
                continue;

            if (Velocity.X > 0)
                newPosition.X = platform.Bounds.Left - Size.X;
            else if (Velocity.X < 0)
                newPosition.X = platform.Bounds.Right;

            Velocity = new Vector2(0, Velocity.Y);
        }

        // Vertical
        newPosition.Y += Velocity.Y * dt;
        var verticalBox = new Rectangle((int)newPosition.X, (int)newPosition.Y, (int)Size.X, (int)Size.Y);
        foreach (var platform in platforms)
        {
            if (!platform.Bounds.Intersects(verticalBox))
                continue;

            if (platform.PassThrough)
            {
                var movingDownward = Velocity.Y >= 0;
                var wasAbove = Position.Y + Size.Y <= platform.Bounds.Top;
                if (!(movingDownward && wasAbove))
                    continue;
            }

            if (Velocity.Y > 0)
            {
                newPosition.Y = platform.Bounds.Top - Size.Y;
                OnGround = true;
                _jumpsRemaining = 2;
            }
            else if (Velocity.Y < 0)
            {
                newPosition.Y = platform.Bounds.Bottom;
            }

            Velocity = new Vector2(Velocity.X, 0);
        }

        Position = newPosition;
    }

    public float ConsumePendingShake()
    {
        var shake = PendingShake;
        PendingShake = 0f;
        return shake;
    }

    public float PendingShake { get; private set; }
}