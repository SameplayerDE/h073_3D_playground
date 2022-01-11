using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game_0;

public class World
{
    public readonly List<Sprite> Sprites;

    public World()
    {
        Sprites = new List<Sprite>();
    }

    public void Add(Sprite sprite)
    {
        sprite.World = this;
        Sprites.Add(sprite);
    }
    
    public void Remove(Sprite sprite)
    {
        sprite.World = null;
        Sprites.Remove(sprite);
    }
    
    public void Update(GameTime gameTime)
    {
        for (var i = Sprites.Count - 1; i >= 0; i--)
        {
            var sprite = Sprites[i];
            sprite.Update(gameTime);
        }
    }
    public void Draw(GraphicsDevice graphicsDevice, Effect effect, Matrix world, Matrix view, Matrix projection)
    {
        for (var i = Sprites.Count - 1; i >= 0; i--)
        {
            var sprite = Sprites[i];
            sprite.Draw(graphicsDevice, effect, world, view, projection);
        }
    }
    
}