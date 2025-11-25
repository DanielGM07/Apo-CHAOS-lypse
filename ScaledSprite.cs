using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apo_CHAOS_lypse
{
    public class ScaledSprite : Sprite
    {
        public Rectangle Rect
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, 150, 150);
            }
        }
        
        public ScaledSprite()
        {
        }

        public ScaledSprite(Texture2D texture, Vector2 position, Color color) : base(texture, position, color)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }
    }
}