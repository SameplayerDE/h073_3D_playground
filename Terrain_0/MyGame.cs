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
        
        private VertexBuffer _vertexBuffer;
        
        private Effect _shader;
        private SpriteFont _font;
        
        public MyGame()
        {
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _world = Matrix.Identity;
            
            _shader = Content.Load<Effect>("shader");
            _font = Content.Load<SpriteFont>("font");

            var vertices = new VertexPositionColor[3];
            
            const float x = 0f;
            const float y = 0f;
            const float z = 0f;
            
            var vertex0 = new VertexPositionColor(new Vector3(x + 0, y + 0, z + 0), Color.White);
            var vertex1 = new VertexPositionColor(new Vector3(x + 0, y + 1, z + 0), Color.White);
            var vertex2 = new VertexPositionColor(new Vector3(x + 1, y + 0, z + 0), Color.White);

            vertices[0] = vertex0;
            vertices[1] = vertex1;
            vertices[2] = vertex2;
            
            _vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, 3, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
            
            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();


            const float speed = 10f;
            var velocity = Vector3.Zero;

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
            }
            else
            {
                //Up
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    velocity.Y += 1f;
                }
                //Down
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    velocity.Y -= 1f;
                }
                //Rotate Right
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    //velocity.X += 1f;
                }
                //Rotate Left
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    //velocity.X -= 1f;
                }
            }

            if (velocity.Length() != 0)
            {
                velocity.Normalize();
                _cameraPosition += velocity * speed * deltaTime;
            }
            
            _view = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + Vector3.Forward, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70f), GraphicsDevice.Viewport.AspectRatio, 1f, 100f);
            
            _shader.Parameters["WorldViewProjection"]?.SetValue(_world * _view * _projection);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var pass in _shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }
            
        }
    }
}