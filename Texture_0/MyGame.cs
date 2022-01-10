using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StartUp_DesktopGL;

namespace Texture_0;

public class MyGame : StartUpDesktopGL
{
    private VertexPositionTexture[] _vertices;
    private BasicEffect _shader;
    
    public MyGame()
    {
        Content.RootDirectory = "Content";
        
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void LoadContent()
    {
        _shader = new BasicEffect(GraphicsDevice);

        _shader.World = Matrix.Identity;
        _shader.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
        _shader.Projection = Matrix.CreateOrthographicOffCenter(-1, 1, -1, 1, 0.1f, 1f);
        _shader.TextureEnabled = true;
        _shader.Texture = Content.Load<Texture2D>("texture");

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
        var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
        var totalTime = (float) gameTime.TotalGameTime.TotalSeconds;

        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }
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