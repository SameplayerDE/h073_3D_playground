using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StartUp_0;

public abstract class StartUp0 : Game
{
    public GraphicsDeviceManager GraphicsDeviceManager;
    public SpriteBatch SpriteBatch;

    protected StartUp0()
    {
        GraphicsDeviceManager = new GraphicsDeviceManager(this);
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }
}