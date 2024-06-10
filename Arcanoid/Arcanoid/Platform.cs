using Arcanoid.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;



namespace Arcanoid
{
    class Platform
    {
        public List<Vector3> verts;
        public List<Vector2> graphic = new List<Vector2>();
        public List<uint> indicies;

        VAO vao;
        VBO vbo;
        VBO uvVbo;
        IBO ibo;
        Texture texture;


        public Platform()
        {
            verts = new List<Vector3>()
            {
            new Vector3(-2.5f, 1.75f, 0.1f),
            new Vector3(2.5f, 1.75f, 0.1f),
            new Vector3(2.5f, -1.75f, 0.1f),
            new Vector3(-2.5f, -1.75f, 0.1f),

            new Vector3(2.5f, 1.75f, 0.1f),
            new Vector3(2.5f, 1.75f, -0.1f),
            new Vector3(2.5f, -1.75f, -0.1f),
            new Vector3(2.5f, -1.75f, 0.1f),

            new Vector3(2.5f, 1.75f, -0.1f),
            new Vector3(-2.5f, 1.75f, -0.1f),
            new Vector3(-2.5f, -1.75f, -0.1f),
            new Vector3(2.5f, -1.75f, -0.1f),

            new Vector3(-2.5f, 1.75f, -0.1f),
            new Vector3(-2.5f, 1.75f, 0.1f),
            new Vector3(-2.5f, -1.75f, 0.1f),
            new Vector3(-2.5f, -1.75f, -0.1f),

            new Vector3(-2.5f, 1.75f, -0.1f),
            new Vector3(2.5f, 1.75f, -0.1f),
            new Vector3(2.5f, 1.75f, 0.1f),
            new Vector3(-2.5f, 1.75f, 0.1f),

            new Vector3(-2.5f, -1.75f, 0.1f),
            new Vector3(2.5f, -1.75f, 0.1f),
            new Vector3(2.5f, -1.75f, -0.1f),
            new Vector3(-2.5f, -1.75f, -0.1f),
            };
            graphic = new List<Vector2>()
            {
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

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
            };

            indicies = new List<uint>{
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

            BuildObject("molnia.jpg");
        }

        public void BuildObject(String path)
        {
            vao = new VAO();
            vao.Bind();

            vbo = new VBO(verts);
            vbo.Bind();
            vao.LinkToVAO(0, 3, vbo);

            uvVbo = new VBO(graphic);
            uvVbo.Bind();
            vao.LinkToVAO(1, 2, uvVbo);

            ibo = new IBO(indicies);

            texture = new Texture(path);
        }

        public void Render(ShaderProgram program)
        {
            program.Bind();
            vao.Bind();
            ibo.Bind();
            texture.Bind();
            GL.DrawElements(PrimitiveType.Triangles, indicies.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void Delete()
        {
            vao.Delete();
            vbo.Delete();
            uvVbo.Delete();
            ibo.Delete();
            texture.Delete();
        }



        public Vector3 CentPos = new Vector3(0,-8,-20);
        public void ChangePosition(Ball ball,KeyboardState input)
        {
            CheckInterSection(ball);
            if (input.IsKeyDown(Keys.D))
            {
                CentPos.X += (float)((1 / MathHelper.Sqrt(2)) / 100);
                CheckInterSection(ball);
            }
            else if (input.IsKeyDown(Keys.A))
            {
                CentPos.X -= (float)((1 / MathHelper.Sqrt(2)) / 100);
                CheckInterSection(ball);
            }
            CheckInterSection(ball);
        }
        public void CheckInterSection(Ball ball)
        {
            if(dist(ball.CentPos, CentPos) <= 3.4f)
            {
                ball.ChangeMove();
                ball.ChangePosition(2);
                return;
            }
            
        }

        float dist(Vector3 v,Vector3 u)
        {
            float x = (v.X - u.X) * (v.X - u.X) + (v.Y - u.Y) * (v.Y - u.Y);
            return (float)MathHelper.Sqrt(x);
        }
    }

    
    
}

    
   

   


