using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StartUp_0;

namespace Texture_2
{

    public class MyGame : StartUp0
    {
        private VertexPositionTexture[] _vertices;
        private Effect _shader;

        public MyGame()
        {
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            _shader = Content.Load<Effect>("shader");


            _shader.Parameters["Texture"]?.SetValue(Content.Load<Texture2D>("texture"));
            _shader.Parameters["Effect"]?.SetValue(Content.Load<Texture2D>("flash"));

            GenerateVertices();
        }

        private void GenerateVertices()
        {
            _vertices = new VertexPositionTexture[6];

            _vertices[0] = new VertexPositionTexture(new Vector3(-.5f, +.5f, 0), new Vector2(0, 0));
            _vertices[1] = new VertexPositionTexture(new Vector3(+.5f, +.5f, 0), new Vector2(1, 0));
            _vertices[2] = new VertexPositionTexture(new Vector3(-.5f, -.5f, 0), new Vector2(0, 1));

            _vertices[3] = new VertexPositionTexture(new Vector3(-.5f, -.5f, 0), new Vector2(0, 1));
            _vertices[4] = new VertexPositionTexture(new Vector3(+.5f, +.5f, 0), new Vector2(1, 0));
            _vertices[5] = new VertexPositionTexture(new Vector3(+.5f, -.5f, 0), new Vector2(1, 1));
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            var (x, y) = GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2;

            const float value = 512f; //PixelsPerUnit

            var world = Matrix.Identity;
            var view = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
            var projection =
                Matrix.CreateOrthographicOffCenter(-x / value, x / value, -y / value, y / value, 0.1f, 100f);

            _shader.Parameters["WorldViewProjection"]?.SetValue(world * view * projection);

            _shader.Parameters["Delta"]?.SetValue(deltaTime);
            _shader.Parameters["Total"]?.SetValue(totalTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var pass in _shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 2);
            }
        }
    }
}