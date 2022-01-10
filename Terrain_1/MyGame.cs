using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StartUp_0;

namespace Terrain_1;

public class MyGame : StartUp0
{
    private RenderTarget2D _target;

    private Matrix _view;
    private Matrix _projection;
    private Matrix _world;

    private Vector3 _cameraPosition;
    
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private float[] _heightMap;

    private const int Width = 100;
    private const float Smoothness = 10f;
    private const float MinHeight = 1.1f;
    private const float MaxHeight = 1.5f;

    private Effect _shader;
    private SpriteFont _font;
    private Texture2D _pixel;

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

        _target = new RenderTarget2D(
            GraphicsDevice,
            GraphicsDeviceManager.PreferredBackBufferWidth / 10,
            GraphicsDeviceManager.PreferredBackBufferHeight / 10,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24
        );

        _world = Matrix.Identity;

        _cameraPosition = new Vector3(0, 0, 10f);

        _shader = Content.Load<Effect>("shader");
        _font = Content.Load<SpriteFont>("font");
        _pixel = Content.Load<Texture2D>("p_w");

        _shader.Parameters["Texture"]?.SetValue(Content.Load<Texture>("texture"));

        _heightMap = new float[Width + 1];

        var vertices = new VertexPositionTexture[Width * 2 + 2];

        var indices = new short[Width * 6];
        var counter = 0;

        //Fill HeightMap
        for (var i = 0; i < Width + 1; i++)
        {
            _heightMap[i] = Random.NextSingle() * (MaxHeight - MinHeight) + MinHeight;
        }

        //Generate Vertices
        for (var i = 0; i < Width * 2 + 2; i += 2)
        {
            var index = i / 2;

            vertices[i].Position = new Vector3(index, _heightMap[index], 0);
            vertices[i].TextureCoordinate.X = index;
            vertices[i].TextureCoordinate.Y = _heightMap[index];

            vertices[i + 1].Position = new Vector3(index, 0, 0);
            vertices[i + 1].TextureCoordinate.X = index;
            vertices[i + 1].TextureCoordinate.Y = 0;
        }

        //Generate Indices
        for (var i = 0; i < Width * 2; i += 2)
        {
            int lowerLeft = i + 1;
            int lowerRight = i + 3;
            int topLeft = i;
            int topRight = i + 2;

            indices[counter++] = (short)topLeft;
            indices[counter++] = (short)lowerRight;
            indices[counter++] = (short)lowerLeft;

            indices[counter++] = (short)topLeft;
            indices[counter++] = (short)topRight;
            indices[counter++] = (short)lowerRight;
        }

        //Create And Fill VertexBuffer
        _vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Length,
            BufferUsage.WriteOnly);
        _vertexBuffer.SetData(vertices);

        //Create And Fill IndexBuffer
        _indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indices.Length,
            BufferUsage.WriteOnly);
        _indexBuffer.SetData(indices);

        //Set Buffers
        GraphicsDevice.SetVertexBuffer(_vertexBuffer);
        GraphicsDevice.Indices = _indexBuffer;
    }

    protected override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        var keyboardState = Keyboard.GetState();

        const float movementSpeed = 2f;

        var velocity = Vector3.Zero;

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        //Right
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            velocity.X += 1f;
        }

        //Left
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            velocity.X -= 1f;
        }

        if (velocity.Length() != 0)
        {
            velocity.Normalize();
            velocity *= movementSpeed;
            velocity *= deltaTime;

            _cameraPosition += velocity;
        }

        var x = _cameraPosition.X;
        var clampedX = (int)x;

        if (x is >= 0 and < Width)
        {
            var a = _heightMap[clampedX];
            var b = _heightMap[clampedX + 1];

            var m = b - a;
            var y = (m * (x - clampedX)) + a;

            _cameraPosition.Y = y;
        }
        else
        {
            _cameraPosition.X = Math.Clamp(_cameraPosition.X, 0, Width);
        }


        _view = Matrix.CreateLookAt(
            _cameraPosition,
            _cameraPosition + Vector3.Forward,
            Vector3.Up
        );

        _projection = Matrix.CreateOrthographic(
            // ReSharper disable once PossibleLossOfFraction
            GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Width,
            // ReSharper disable once PossibleLossOfFraction
            GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
            0.1f,
            100f
        );

        _shader.Parameters["WorldViewProjection"]?.SetValue(_world * _view * _projection);
        _shader.Parameters["Delta"]?.SetValue(deltaTime);
        _shader.Parameters["Total"]?.SetValue(totalTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(_target);
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.Clear(Color.CornflowerBlue);

        foreach (var pass in _shader.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indexBuffer.IndexCount / 3);
        }

        GraphicsDevice.SetRenderTarget(null);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        SpriteBatch.Draw(_target, GraphicsDevice.Viewport.Bounds, Color.White);
        SpriteBatch.End();

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp,
            transformMatrix: Matrix.CreateTranslation(new Vector3(GraphicsDevice.Viewport.Bounds.Center.ToVector2(),
                0)));
        SpriteBatch.Draw(_pixel, new Rectangle(-1, -1, 2, 2), Color.Red);
        SpriteBatch.End();
    }
}