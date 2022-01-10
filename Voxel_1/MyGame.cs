using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StartUp_DesktopGL;

namespace Voxel_1;

public class MyGame : StartUpDesktopGL
{
    private Voxel[] _voxels;
    private Effect _shader;
    private Texture2D _texture;
    private RenderTarget2D _target;

    private Matrix _view;
    private Matrix _projection;
    private Matrix _world;

    private Vector3 _cameraPosition;
    private Vector3 _cameraRotation;
    private float _fieldOfView;


    public MyGame()
    {
        Content.RootDirectory = "Content";

        IsMouseVisible = true;
        Window.AllowUserResizing = true;

        GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false; //Vsync
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 25);
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        _shader = Content.Load<Effect>("shader");
        _texture = Content.Load<Texture2D>("texture");

        _target = new RenderTarget2D(
            GraphicsDevice,
            GraphicsDeviceManager.PreferredBackBufferWidth,
            GraphicsDeviceManager.PreferredBackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24
        );

        _fieldOfView = 48f;
        _world = Matrix.Identity;

        _shader.Parameters["Texture"]?.SetValue(_texture);

        const int width = 10;
        const int depth = 10;

        _voxels = new Voxel[width * depth];

        for (var i = 0; i < depth; i++)
        {
            for (var j = 0; j < width; j++)
            {
                _voxels[j + width * i] = new Voxel(_shader);
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        var keyboardState = Keyboard.GetState();

        var frameRate = 1 / deltaTime;

        Window.Title = $"{frameRate}";

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

        for (var index = 0; index < _voxels.Length; index++)
        {
            var voxel = _voxels[index];
            _shader.Parameters["WorldViewProjection"]
                ?.SetValue(Matrix.CreateTranslation(new Vector3(index % 10, 0, index / 10)) * _view * _projection);
            voxel.Draw(GraphicsDevice);
        }
    }
}