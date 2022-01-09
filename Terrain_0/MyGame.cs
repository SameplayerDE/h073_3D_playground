using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Terrain_0
{
    public class MyGame : Game0
    {
        
        private Matrix _view;
        private Matrix _projection;
        private Matrix _world;

        private Vector3 _cameraPosition;
        private Vector3 _cameraRotation;
        private float _fieldOfView;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private Effect _shader;
        private SpriteFont _font;

        private static readonly Random Random = new Random();

        public MyGame()
        {
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            Window.AllowUserResizing = true;

            if (GraphicsDevice == null)
            {
                GraphicsDeviceManager.ApplyChanges();
            }

            if (GraphicsDevice != null)
            {
                GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            }

            GraphicsDeviceManager.HardwareModeSwitch = false;
            GraphicsDeviceManager.PreferMultiSampling = false;
            GraphicsDeviceManager.IsFullScreen = true;
            GraphicsDeviceManager.ApplyChanges();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _world = Matrix.Identity;

            _cameraRotation = Vector3.Zero;
            _cameraPosition = Vector3.Zero;
            _fieldOfView = 45f;

            _shader = Content.Load<Effect>("shader");
            _font = Content.Load<SpriteFont>("font");
            
            _shader.Parameters["Texture"]?.SetValue(Content.Load<Texture>("texture"));

            var x = 0f;
            var y = 0f;
            var z = 0f;

            const int width = 50;
            const int height = 50;

            var vertices = new VertexPositionTexture[width * height];
            var indices = new short[(width - 1) * (height) * 6];

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var index = j + width * i;
                    x = j;
                    z = -i;

                    y = Random.NextSingle() / 2f;

                    //var vertex0 = new VertexPositionColor(new Vector3(x + 0, y + 0, z + 0), color);
                    vertices[index].Position = new Vector3(x + 0, y + 0, z + 0);
                    vertices[index].TextureCoordinate.X = x;
                    vertices[index].TextureCoordinate.Y = -z;
                }
            }


            int counter = 0;
            for (int i = 0; i < height - 1; i++)
            {
                for (int j = 0; j < width - 1; j++)
                {
                    int lowerLeft = j + i * width;
                    int lowerRight = (j + 1) + i * width;
                    int topLeft = j + (i + 1) * width;
                    int topRight = (j + 1) + (i + 1) * width;

                    indices[counter++] = (short) topLeft;
                    indices[counter++] = (short) lowerRight;
                    indices[counter++] = (short) lowerLeft;

                    indices[counter++] = (short) topLeft;
                    indices[counter++] = (short) topRight;
                    indices[counter++] = (short) lowerRight;
                }
            }

            _vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Length,
                BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);

            _indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indices.Length,
                BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);

            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            GraphicsDevice.Indices = _indexBuffer;
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var totalTime = (float) gameTime.TotalGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            const float movementSpeed = 2f;
            const float rotationSpeed = 2f;

            var velocity = Vector3.Zero;
            var rotation = Vector3.Zero;

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            
            if (keyboardState.IsKeyUp(Keys.LeftShift))
            {
                //Forward
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    velocity.Z -= 1f;
                }

                //Right
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    velocity.X += 1f;
                }

                //Backwards
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    velocity.Z += 1f;
                }

                //Left
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    velocity.X -= 1f;
                }

                //Up
                if (keyboardState.IsKeyDown(Keys.PageUp))
                {
                    velocity.Y += 1f;
                }

                //Down
                if (keyboardState.IsKeyDown(Keys.PageDown))
                {
                    velocity.Y -= 1f;
                }
            }
            else
            {
                //Up
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    rotation.X += 1f;
                }

                //Down
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    rotation.X -= 1f;
                }

                //Rotate Right
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    rotation.Y -= 1f;
                }

                //Rotate Left
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    rotation.Y += 1f;
                }
            }

            if (rotation.Length() != 0)
            {
                rotation.Normalize();
                rotation *= rotationSpeed;
                rotation *= deltaTime;

                _cameraRotation += rotation;
            }


            if (velocity.Length() != 0)
            {
                velocity.Normalize();
                velocity *= movementSpeed;
                velocity *= deltaTime;

                var rotationMatrix = Matrix.CreateRotationY(_cameraRotation.Y);

                _cameraPosition += Vector3.Transform(velocity, rotationMatrix);
            }

            var cameraRotationMatrix = Matrix.CreateRotationX(_cameraRotation.X) *
                                       Matrix.CreateRotationY(_cameraRotation.Y) *
                                       Matrix.CreateRotationZ(_cameraRotation.Z);
            var cameraDirection = Vector3.Transform(Vector3.Forward, cameraRotationMatrix);

            _view = Matrix.CreateLookAt(
                _cameraPosition,
                _cameraPosition + cameraDirection,
                Vector3.Up
            );

            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(_fieldOfView),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f
            );

            _shader.Parameters["WorldViewProjection"]?.SetValue(_world * _view * _projection);
            _shader.Parameters["Delta"]?.SetValue(deltaTime);
            _shader.Parameters["Total"]?.SetValue(totalTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var pass in _shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indexBuffer.IndexCount / 3);
            }
        }
    }
}