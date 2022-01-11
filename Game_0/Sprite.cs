using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game_0;

public class Sprite
{
    private static VertexPositionTexture[] _vertices;

    private static void GenerateVertices()
    {
        _vertices = new VertexPositionTexture[6];

        _vertices[0] = new VertexPositionTexture(new Vector3(-.5f, +.5f, 0), new Vector2(0, 0));
        _vertices[1] = new VertexPositionTexture(new Vector3(+.5f, +.5f, 0), new Vector2(1, 0));
        _vertices[2] = new VertexPositionTexture(new Vector3(-.5f, -.5f, 0), new Vector2(0, 1));

        _vertices[3] = new VertexPositionTexture(new Vector3(-.5f, -.5f, 0), new Vector2(0, 1));
        _vertices[4] = new VertexPositionTexture(new Vector3(+.5f, +.5f, 0), new Vector2(1, 0));
        _vertices[5] = new VertexPositionTexture(new Vector3(+.5f, -.5f, 0), new Vector2(1, 1));
    }

    public World World;
    public Vector3 Position;
    public Vector3 Velocity;
    public bool Dynamic = true;
    
    
    public string TextureName;

    static Sprite()
    {
        GenerateVertices();
    }

    public Sprite()
    {
        const float min = -2.0f;
        const float max = +2.0f;
        var x = min + MyGame.Random.NextSingle() * (max - min);
        var y = min + MyGame.Random.NextSingle() * (max - min);
        Velocity += new Vector3(x, y, 0);
    }

    public void Update(GameTime gameTime)
    {

        if (!Dynamic) return;

        var self = this;

        if (self.Position.X < -5)
        {
            Velocity.X *= -1;
            self.Position.X = -5;
        }

        if (self.Position.Y < -5)
        {
            Velocity.Y *= -1;
            self.Position.Y = -5;
        }

        if (self.Position.X > 5)
        {
            Velocity.X *= -1;
            self.Position.X = 5;
        }

        if (self.Position.Y > 5)
        {
            Velocity.Y *= -1;
            self.Position.Y = 5;
        }

        for (var i = World.Sprites.Count - 1; i >= 0; i--)
        {
            var other = World.Sprites[i];
            
            if (!other.Dynamic) continue;
            if (other == self) continue;

            var distance = Vector3.Distance(self.Position, other.Position);

            if (!(distance <= 1)) continue;

            var collisionPointX = (self.Position.X + other.Position.X) / 2;
            var collisionPointY = (self.Position.Y + other.Position.Y) / 2;

            var penetration = distance - 1;

            if (penetration <= 0)
            {
                self.Position += self.Velocity * penetration;
                other.Position += other.Velocity * penetration;
            }

            var selfVelX = (self.Velocity.X * 0 + 2 * 1 * other.Velocity.X) / 2;
            var selfVelY = (self.Velocity.Y * 0 + 2 * 1 * other.Velocity.Y) / 2;
            
            var otherVelX = (other.Velocity.X * 0 + 2 * 1 * self.Velocity.X) / 2;
            var otherVelY = (other.Velocity.Y * 0 + 2 * 1 * self.Velocity.Y) / 2;

            other.Velocity = new Vector3(otherVelX, otherVelY, 0);
            self.Velocity = new Vector3(selfVelX, selfVelY, 0);
            
        }

        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Draw(GraphicsDevice graphicsDevice, Vector3 position, Effect effect, Matrix world, Matrix view,
        Matrix projection)
    {
        effect.Parameters["Texture"]?.SetValue(MyGame.Textures[TextureName ?? "texture"]);
        effect.Parameters["WorldViewProjection"]
            ?.SetValue(world * Matrix.CreateTranslation(position) * view * projection);
        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 2);
        }
    }

    public void Draw(GraphicsDevice graphicsDevice, Effect effect, Matrix world, Matrix view, Matrix projection)
    {
        Draw(graphicsDevice, Position, effect, world, view, projection);
    }
}