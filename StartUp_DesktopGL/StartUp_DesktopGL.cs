using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StartUp_DesktopGL;

public abstract class StartUpDesktopGL : Game
{
    public GraphicsDeviceManager GraphicsDeviceManager;
    public SpriteBatch SpriteBatch;

    protected StartUpDesktopGL()
    {
        GraphicsDeviceManager = new GraphicsDeviceManager(this);
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }
}