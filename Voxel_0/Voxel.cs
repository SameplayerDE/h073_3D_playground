using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Voxel_0;

public class Voxel
{
    private VertexPositionTexture[] _vertices;
    private Effect _shader;

    private int _index = 0;

    public Voxel(Effect shader)
    {
        _shader = shader;
        
        GenerateVertices();
    }

    private void GenerateVertices()
    {
        _vertices = new VertexPositionTexture[36];
        
        GenerateBackwards(ref _vertices);
        GenerateUp(ref _vertices);
        GenerateForwards(ref _vertices);
        GenerateRight(ref _vertices);
        GenerateDown(ref _vertices);
        GenerateLeft(ref _vertices);
    }

    private void GenerateBackwards(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(1, 1));
    }
    
    private void GenerateForwards(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(1, 1));
    }
    
    private void GenerateRight(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 1));
    }
    
    private void GenerateLeft(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(1, 1));
    }
    
    private void GenerateUp(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 1));
    }
    
    private void GenerateDown(ref VertexPositionTexture[] vertices)
    {
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));

        vertices[_index++] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(1, 0));
        vertices[_index++] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 1));
    }

    public void Draw(GraphicsDevice graphicsDevice)
    {
        foreach (var pass in _shader.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length / 3);
        }
    }

}