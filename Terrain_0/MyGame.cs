using Microsoft.Xna.Framework;

namespace Terrain_0
{
    public class MyGame : Game0
    {
        public MyGame()
        {
            IsMouseVisible = true;
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }
    }
}