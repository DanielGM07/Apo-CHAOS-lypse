using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Apo_CHAOS_lypse
{
    public class MovingSprite : ScaledSprite
    {
        float speed;

        public MovingSprite()
        {
        }

        public MovingSprite(Texture2D texture, Vector2 position, Color color, float speed) : base(texture, position, color)
        {
            this.speed = speed;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                position.Y -= 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                position.Y += 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.X -= 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.X += 5;
            }
        }
    }
}