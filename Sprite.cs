
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 
namespace Apo_CHAOS_lypse;

public class Sprite
 {
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public Color Color { get; set; }
    public Vector2 Size { get; set; }

    public Rectangle BoundingBox => new((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    public Sprite()
     {
     }
 
    public Sprite(Texture2D texture, Vector2 position, Color color)
    {
        Texture = texture;
        Position = position;
        Color = color;
        Size = new Vector2(texture.Width, texture.Height);
    }
 
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, BoundingBox, Color);
     }

}
