using Arcanoid.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Arcanoid
{
    internal class Brick
    {
        public List<Vector3> gameVerts;
        public List<Vector2> gameGraphic = new List<Vector2>();
        public List<uint> gameInd;
        int type;

        VAO gameVAO;
        VBO gameVertexVBO;
        VBO gameGraphicVBO;
        IBO gameIBO;

        Texture texture;
        public Brick() 
        {
            gameVerts = new List<Vector3>()
            {
            new Vector3(-2, 2, -20),
            new Vector3(2, 2, -20),
            new Vector3(2, -2, -20),
            new Vector3(-2, -2, -20),

            new Vector3(2, 2, -20),
            new Vector3(2, 2, -20),
            new Vector3(2, -2, -20),
            new Vector3(2, -2, -20),

            new Vector3(2, 2, -20),
            new Vector3(-2, 2, -20),
            new Vector3(-2, -2, -20),
            new Vector3(2, -2, -20),

            new Vector3(-2, 2, -20),
            new Vector3(-2, 2, -20),
            new Vector3(-2, -2, -20),
            new Vector3(-2, -2, -20),

            new Vector3(-2, 2, -20),
            new Vector3(2, 2, -20),
            new Vector3(2, 2, -20),
            new Vector3(-2, 2, -20),

            new Vector3(-2, -2, -20),
            new Vector3(2, -2, -20),
            new Vector3(2, -2, -20),
            new Vector3(-2, -2, -20),
            };
            gameGraphic = new List<Vector2>()
            {
            new Vector2(1f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),

            //new Vector2(0f, 1f),
            //new Vector2(1f, 1f),
            //new Vector2(1f, 0f),
            //new Vector2(0f, 0f),

             new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
            };

            gameInd = new List<uint>{
            0, 1, 2,
            2, 3, 0,

            4, 5, 6,
            6, 7, 4,

            8, 9, 10,
            10, 11, 8,

            12, 13, 14,
            14, 15, 12,

            16, 17, 18,
            18, 19, 16,

            20, 21, 22,
            22, 23, 20
            };

            BuildObject("Brick.png");
        }
        public void BuildObject(String path)
        {
            gameVAO = new VAO();
            gameVAO.Bind();

            gameVertexVBO = new VBO(gameVerts);
            gameVertexVBO.Bind();
            gameVAO.LinkToVAO(0, 3, gameVertexVBO);

            gameGraphicVBO = new VBO(gameGraphic);
            gameGraphicVBO.Bind();
            gameVAO.LinkToVAO(1, 2, gameGraphicVBO);

            gameIBO = new IBO(gameInd);

            texture = new Texture(path);
        }

        public void Render(ShaderProgram program)
        {
            program.Bind();
            gameVAO.Bind();
            gameIBO.Bind();
            texture.Bind();
            GL.DrawElements(PrimitiveType.Triangles, gameInd.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void Delete()
        {
            gameVAO.Delete();
            gameVertexVBO.Delete();
            gameGraphicVBO.Delete();
            gameIBO.Delete();
            texture.Delete();
        }

    }
}
