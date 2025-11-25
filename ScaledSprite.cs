 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 
namespace Apo_CHAOS_lypse;

public class ScaledSprite : Sprite
 {
    public ScaledSprite()
    {
    }
 
    public ScaledSprite(Texture2D texture, Vector2 position, Color color, Vector2? sizeOverride = null) : base(texture, position, color)
    {
        if (sizeOverride.HasValue)
         {
            Size = sizeOverride.Value;
         }
    }
 

    public virtual void Update(GameTime gameTime)
    {
    }
}
