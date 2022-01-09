using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terrain_0
{
    public class Game0 : Game
    {
        public GraphicsDeviceManager GraphicsDeviceManager;
        public SpriteBatch SpriteBatch;

        public Game0()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }
    }
}