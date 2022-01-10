using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StartUp_DesktopGL;

namespace Triangle_0;

public class MyGame : StartUpDesktopGL
{
    private VertexPositionColor[] _vertices;
    private BasicEffect _shader;
    
    public MyGame()
    {
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void LoadContent()
    {
        _shader = new BasicEffect(GraphicsDevice);

        _shader.World = Matrix.Identity;
        _shader.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
        _shader.Projection = Matrix.CreateOrthographicOffCenter(-1, 1, 0, 1, 0.1f, 1f);
        _shader.VertexColorEnabled = true;

        GenerateVertices();
    }

    private void GenerateVertices()
    {
        _vertices = new VertexPositionColor[3];

        _vertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
        _vertices[1] = new VertexPositionColor(new Vector3(1, 0, 0), Color.Green);
        _vertices[2] = new VertexPositionColor(new Vector3(-1, 0, 0), Color.Blue);
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
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 1);
        }
    }
}