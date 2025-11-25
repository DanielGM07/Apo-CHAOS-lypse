using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apo_CHAOS_lypse;

public class Platform
{
    public Rectangle Bounds { get; }
    public bool PassThrough { get; }
    public Color Color { get; }

    public Platform(Rectangle bounds, bool passThrough, Color color)
    {
        Bounds = bounds;
        PassThrough = passThrough;
        Color = color;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        spriteBatch.Draw(pixel, Bounds, Color);
    }
}