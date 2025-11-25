using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apo_CHAOS_lypse
{
    public class Sprite
    {
        public Texture2D texture;
        public Vector2 position;
        public Color color;

        public Sprite()
        {
        }

        public Sprite(Texture2D texture, Vector2 position, Color color)
        {
            this.texture = texture;
            this.position = position;
            this.color = color;
        }
    }
}