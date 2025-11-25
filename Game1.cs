using Apo_CHAOS_lypse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Apo_CHAOS_lypse;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    Texture2D texturePlayer;
    Texture2D mancTexture;
    Sprite mancSprite;
    Vector2 mancPosition;
    Color mancColor;
    MovingSprite player;
    SpriteFont arialFont;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        texturePlayer = Content.Load<Texture2D>("doomguy_player");
        mancTexture = Content.Load<Texture2D>("pixel_mancubus");
        mancPosition = new Vector2(100, 100);
        mancColor = Color.Red;

        arialFont = Content.Load<SpriteFont>("Arial");

        mancSprite = new Sprite(mancTexture, mancPosition, mancColor);
        player = new MovingSprite(texturePlayer, Vector2.Zero, Color.White, 5f);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        player.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();

        _spriteBatch.Draw(player.texture, player.Rect, player.color);

        _spriteBatch.Draw(mancTexture, mancPosition, mancColor);
        
        _spriteBatch.DrawString(arialFont, "Teasteando fuente!", new Vector2(600, 100), Color.White);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
