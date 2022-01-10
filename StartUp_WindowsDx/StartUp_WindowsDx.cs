using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StartUp_Dx;

public abstract class StartUpDx : Game
{
    public GraphicsDeviceManager GraphicsDeviceManager;
    public SpriteBatch SpriteBatch;

    protected StartUpDx()
    {
        GraphicsDeviceManager = new GraphicsDeviceManager(this);
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }
}