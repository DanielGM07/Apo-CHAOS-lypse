using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Apo_CHAOS_lypse;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    private Texture2D _pixel = null!;
    private Texture2D _playerTexture = null!;
    private Texture2D _enemyTexture = null!;
    private SpriteFont _arialFont = null!;

    private Player _player = null!;
    private readonly List<Platform> _platforms = new();
    private readonly List<Projectile> _projectiles = new();
    private readonly List<Enemy> _enemies = new();
    private readonly List<Weapon> _arsenal = new();
    private int _weaponIndex;
    private readonly Random _random = new();

    private float _cameraShakeTimer;
    private float _cameraShakeDuration;
    private float _cameraShakeMagnitude;
    private Vector2 _cameraOffset;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _playerTexture = Content.Load<Texture2D>("doomguy_player");
        _enemyTexture = Content.Load<Texture2D>("pixel_mancubus");
        _arialFont = Content.Load<SpriteFont>("Arial");
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        BuildLevel();
        BuildWeapons();
        InitializeCamera();
    }

    private void BuildWeapons()
    {
        _arsenal.Add(new GunWeapon("AK-47", 0.08f, Color.Goldenrod, _pixel, 900f, 8, new Vector2(10, 4), 4f));
        _arsenal.Add(new RocketLauncher(_pixel));
        _arsenal.Add(new Katana(_pixel));

        _weaponIndex = 0;
        _player.CurrentWeapon = _arsenal[_weaponIndex];
    }

    private void BuildLevel()
    {
        var playerStart = new Vector2(120, 200);
        _player = new Player(_playerTexture, playerStart, Color.White, new Vector2(38, 48));

        _platforms.Add(new Platform(new Rectangle(0, 460, 900, 40), false, Color.DarkSlateGray));
        _platforms.Add(new Platform(new Rectangle(120, 360, 200, 20), false, Color.SlateGray));
        _platforms.Add(new Platform(new Rectangle(360, 320, 160, 16), true, Color.SteelBlue * 0.8f));
        _platforms.Add(new Platform(new Rectangle(580, 260, 200, 16), false, Color.SlateGray));
        _platforms.Add(new Platform(new Rectangle(740, 420, 180, 16), true, Color.SteelBlue * 0.8f));

        _enemies.Add(new Enemy(_enemyTexture, new Vector2(600, 100), Color.White, new Vector2(48, 48), 100));
        _enemies.Add(new Enemy(_enemyTexture, new Vector2(250, 60), Color.White, new Vector2(48, 48), 60));
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        HandleWeaponSelection(keyboardState);

        _player.Update(gameTime, keyboardState, mouseState, _platforms, _projectiles, _enemies);
        UpdateCamera(gameTime, keyboardState);

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        foreach (var enemy in _enemies)
        {
            enemy.Update(dt, _player.Position, _platforms);
        }

        UpdateProjectiles(dt);
        Cleanup();

        base.Update(gameTime);
    }

    private void Cleanup()
    {
        _projectiles.RemoveAll(p => p.Lifetime <= 0);
        _enemies.RemoveAll(e => e.Health <= 0);
    }

    private void HandleWeaponSelection(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.D1)) _weaponIndex = 0;
        if (keyboardState.IsKeyDown(Keys.D2)) _weaponIndex = 1;
        if (keyboardState.IsKeyDown(Keys.D3)) _weaponIndex = 2;

        _weaponIndex = MathHelper.Clamp(_weaponIndex, 0, _arsenal.Count - 1);
        _player.CurrentWeapon = _arsenal[_weaponIndex];
    }

    private void UpdateProjectiles(float dt)
    {
        foreach (var projectile in _projectiles)
        {
            projectile.Update(dt);

            foreach (var enemy in _enemies.Where(e => e.Health > 0))
            {
                if (!projectile.FromPlayer || projectile.Lifetime <= 0)
                    continue;

                if (projectile.BoundingBox.Intersects(enemy.BoundingBox))
                {
                    enemy.ApplyDamage(projectile.Damage);
                    projectile.Lifetime = 0;
                }
            }

            if (!projectile.FromPlayer && projectile.BoundingBox.Intersects(_player.BoundingBox))
            {
                projectile.Lifetime = 0;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(18, 20, 29));

        _spriteBatch.Begin(transformMatrix: Camera.Instance.Matrix);

        foreach (var platform in _platforms)
        {
            platform.Draw(_spriteBatch, _pixel);
        }

        foreach (var projectile in _projectiles)
        {
            projectile.Draw(_spriteBatch);
        }

        foreach (var enemy in _enemies)
        {
            if (enemy.Health > 0)
                enemy.Draw(_spriteBatch);
        }

        _player.Draw(_spriteBatch);

        _spriteBatch.End();

        _spriteBatch.Begin();

        DrawHud();

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawHud()
    {
        var weapon = _arsenal[_weaponIndex];
        var lines = new List<string>
        {
            $"Weapon: {weapon.Name}",
            "Controls:",
            "A/D to move, Space to double jump, LCtrl to dash",
            "Hold Shift to strafe, 1-3 to swap weapons",
            "Left click to attack",
            "Q to zoom in, E to zoom out",
            "Blue platforms are pass-through from below"
        };

        var position = new Vector2(16, 16);
        foreach (var line in lines)
        {
            _spriteBatch.DrawString(_arialFont, line, position, Color.White);
            position.Y += 20;
        }
    }

    private void InitializeCamera()
    {
        Camera.Initialize(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        Camera.Instance.CenterOrigin();
        var playerCenter = _player.Position + _player.Size / 2f;
        Camera.Instance.Position = playerCenter;
    }

    private void UpdateCamera(GameTime gameTime, KeyboardState keyboardState)
    {
        var camera = Camera.Instance;
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        camera.Position -= _cameraOffset;

        HandleZoomInput(camera, keyboardState, dt);

        camera.FollowPlayer(gameTime, _player);

        var pendingShake = _player.ConsumePendingShake();
        if (pendingShake > 0)
        {
            StartCameraShake(pendingShake, 0.25f);
        }

        if (_cameraShakeTimer > 0)
        {
            _cameraShakeTimer -= dt;
            var normalized = _cameraShakeDuration > 0 ? MathF.Max(_cameraShakeTimer / _cameraShakeDuration, 0f) : 0f;
            var intensity = _cameraShakeMagnitude * normalized;
            _cameraOffset = intensity <= 0 ? Vector2.Zero : new Vector2(
                (float)(_random.NextDouble() * 2 - 1) * intensity,
                (float)(_random.NextDouble() * 2 - 1) * intensity);
        }
        else
        {
            _cameraOffset = Vector2.Zero;
        }

        camera.Position += _cameraOffset;
    }

    private void HandleZoomInput(Camera camera, KeyboardState keyboardState, float dt)
    {
        var zoom = camera.Zoom;
        if (keyboardState.IsKeyDown(Keys.Q))
            zoom += 0.8f * dt;
        if (keyboardState.IsKeyDown(Keys.E))
            zoom -= 0.8f * dt;

        zoom = MathHelper.Clamp(zoom, 0.6f, 1.8f);
        camera.Zoom = zoom;
        camera.CenterOrigin();
    }

    private void StartCameraShake(float magnitude, float duration)
    {
        _cameraShakeMagnitude = magnitude;
        _cameraShakeDuration = duration;
        _cameraShakeTimer = duration;
    }
}