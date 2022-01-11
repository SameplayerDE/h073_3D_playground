using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StartUp_DesktopGL;

namespace Game_0
{
    public class MyGame : StartUpDesktopGL
    {
        private Effect _shader;
        public static Dictionary<string, Texture2D> Textures;

        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;
        
        private World _gameWorld;

        public static Random Random = new Random();
        

        public MyGame()
        {
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            Window.AllowUserResizing = true;
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

            var value = 42f * GraphicsDevice.Viewport.AspectRatio; //PixelsPerUnit

            _world = Matrix.Identity;
            _view = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
            _projection =
                Matrix.CreateOrthographicOffCenter(-x / value, x / value, -y / value, y / value, 0.1f, 100f);

            _shader.Parameters["WorldViewProjection"]?.SetValue(_world * _view * _projection);

            _shader.Parameters["Delta"]?.SetValue(deltaTime);
            _shader.Parameters["Total"]?.SetValue(totalTime);
            
            _gameWorld.Update(gameTime);
            
        }

        protected override void LoadContent()
        {

            Textures = new Dictionary<string, Texture2D>();
            
            _shader = Content.Load<Effect>("shader");

            Textures.Add("texture", Content.Load<Texture2D>("texture"));
            Textures.Add("test", Content.Load<Texture2D>("test"));

            _gameWorld = new World();


            
            for (var i = 0; i < 5; i++)
            {
                _gameWorld.Add(
                    new Sprite()
                    {
                        Position = new Vector3(i, 0, 0),
                        TextureName = "test"
                    }
                );
            }
            
            for (var i = -5; i <= 5; i++)
            {
                for (var j = -5; j <= 5; j++)
                {
                    _gameWorld.Add(
                        new Sprite()
                        {
                            Position = new Vector3(i, j, 0),
                            Dynamic = false
                        }
                    );
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _gameWorld.Draw(GraphicsDevice, _shader, _world, _view, _projection);
        }
    }
}