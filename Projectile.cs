using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apo_CHAOS_lypse;

public class Projectile : Sprite
{
    public Vector2 Velocity { get; set; }
    public float Lifetime { get; set; }
    public int Damage { get; set; }
    public bool FromPlayer { get; set; }

    public Projectile(Texture2D texture, Vector2 position, Color color, Vector2 size, Vector2 velocity, float lifetime, int damage, bool fromPlayer) : base(texture, position, color)
    {
        Size = size;
        Velocity = velocity;
        Lifetime = lifetime;
        Damage = damage;
        FromPlayer = fromPlayer;
    }

    public void Update(float dt)
    {
        Position += Velocity * dt;
        Lifetime -= dt;
    }
}