using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apo_CHAOS_lypse;

public class Enemy : ScaledSprite
{
    private const float Gravity = 900f;
    private const float MoveSpeed = 120f;

    public int Health { get; private set; }
    public Vector2 Velocity { get; private set; }

    public Enemy(Texture2D texture, Vector2 position, Color color, Vector2 size, int health) : base(texture, position, color, size)
    {
        Health = health;
    }

    public void ApplyDamage(int value)
    {
        Health = Math.Max(0, Health - value);
    }

    public void Update(float dt, Vector2 playerPosition, IReadOnlyList<Platform> platforms)
    {
        if (Health <= 0)
            return;

        var direction = Math.Sign(playerPosition.X - Position.X);
        Velocity = new Vector2(direction * MoveSpeed, Velocity.Y + Gravity * dt);

        var newPosition = Position;
        newPosition.X += Velocity.X * dt;
        var horizontal = new Rectangle((int)newPosition.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        foreach (var platform in platforms)
        {
            if (!platform.Bounds.Intersects(horizontal) || platform.PassThrough)
                continue;

            if (Velocity.X > 0)
                newPosition.X = platform.Bounds.Left - Size.X;
            else if (Velocity.X < 0)
                newPosition.X = platform.Bounds.Right;
            Velocity = new Vector2(0, Velocity.Y);
        }

        newPosition.Y += Velocity.Y * dt;
        var vertical = new Rectangle((int)newPosition.X, (int)newPosition.Y, (int)Size.X, (int)Size.Y);
        foreach (var platform in platforms)
        {
            if (!platform.Bounds.Intersects(vertical))
                continue;

            var canCollide = !platform.PassThrough || (Velocity.Y >= 0 && Position.Y + Size.Y <= platform.Bounds.Top);
            if (!canCollide)
                continue;

            if (Velocity.Y > 0)
                newPosition.Y = platform.Bounds.Top - Size.Y;
            else if (Velocity.Y < 0)
                newPosition.Y = platform.Bounds.Bottom;
            Velocity = new Vector2(Velocity.X, 0);
        }

        Position = newPosition;
    }
}