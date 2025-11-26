using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Apo_CHAOS_lypse;

public abstract class Weapon
{
    protected float Cooldown;
    private float _cooldownTimer;

    public string Name { get; }
    public Color ProjectileColor { get; }
    public float ShotShake { get; }

    protected Weapon(string name, float cooldown, Color projectileColor, float shotShake)
    {
        Name = name;
        Cooldown = cooldown;
        ProjectileColor = projectileColor;
        ShotShake = shotShake;
    }

    public bool TryFire(GameTime gameTime, Player player, MouseState mouseState, List<Projectile> projectiles, IReadOnlyList<Enemy> enemies, out float shake)
    {
        shake = 0f;
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_cooldownTimer > 0)
            _cooldownTimer -= dt;

        if (!mouseState.LeftButton.Equals(ButtonState.Pressed) || _cooldownTimer > 0)
            return false;

        Fire(player, projectiles, enemies);
        _cooldownTimer = Cooldown;
        shake = ShotShake;
        return true;
    }

    protected abstract void Fire(Player player, List<Projectile> projectiles, IReadOnlyList<Enemy> enemies);
}

public class GunWeapon : Weapon
{
    private readonly Texture2D _pixel;
    private readonly float _speed;
    private readonly int _damage;
    private readonly Vector2 _size;

    public GunWeapon(string name, float cooldown, Color projectileColor, Texture2D pixel, float speed, int damage, Vector2 size, float shake)
        : base(name, cooldown, projectileColor, shake)
    {
        _pixel = pixel;
        _speed = speed;
        _damage = damage;
        _size = size;
    }

    protected override void Fire(Player player, List<Projectile> projectiles, IReadOnlyList<Enemy> enemies)
    {
        var direction = player.FacingLeft ? -1 : 1;
        var velocity = new Vector2(_speed * direction, 0);
        var spawn = player.Position + new Vector2(player.Size.X / 2, player.Size.Y / 2);
        projectiles.Add(new Projectile(_pixel, spawn, ProjectileColor, _size, velocity, 2f, _damage, true));
    }
}

public class RocketLauncher : Weapon
{
    private readonly Texture2D _pixel;
    private readonly float _speed;
    private readonly int _damage;

    // Shot Shake originally in 12f, but I decided to let it in 50f
    public RocketLauncher(Texture2D pixel) : base("Rocket Launcher", 0.8f, Color.OrangeRed, 50f)
    {
        _pixel = pixel;
        _speed = 300f;
        _damage = 40;
    }

    protected override void Fire(Player player, List<Projectile> projectiles, IReadOnlyList<Enemy> enemies)
    {
        var direction = player.FacingLeft ? -1 : 1;
        var velocity = new Vector2(_speed * direction, -40f);
        var spawn = player.Position + new Vector2(player.Size.X / 2, player.Size.Y / 2);
        projectiles.Add(new Projectile(_pixel, spawn, ProjectileColor, new Vector2(22, 12), velocity, 2.8f, _damage, true));
    }
}

public class Katana : Weapon
{
    private readonly Texture2D _pixel;

    public Katana(Texture2D pixel) : base("Katana", 0.35f, Color.LightGray, 2f)
    {
        _pixel = pixel;
    }

    protected override void Fire(Player player, List<Projectile> projectiles, IReadOnlyList<Enemy> enemies)
    {
        var direction = player.FacingLeft ? -1 : 1;
        var swingSize = new Vector2(60, player.Size.Y);
        var spawn = player.Position + new Vector2(direction < 0 ? -swingSize.X : player.Size.X, 0);
        projectiles.Add(new Projectile(_pixel, spawn, ProjectileColor * 0.8f, swingSize, Vector2.Zero, 0.1f, 30, true));
    }
}