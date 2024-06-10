using Arcanoid.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Arcanoid
{
    internal class Ball
    {
        public List<Vector3> gameVerts;
        public List<Vector2> gameGraphic = new List<Vector2>();
        public List<uint> gameInd;

        VAO gameVAO;
        VBO gameVertexVBO;
        VBO gameGraphicVBO;
        IBO gameIBO;

        Texture texture;

        public Ball()
        {
            /*
            Вызов метода GeneratePlayerMesh, который генерирует вершины и индексы для мяча. 
            Радиус мяча — 1.0f. 
            Результаты сохраняются в списках gameVerts и gameInd.
             */
            GeneratePlayerMesh(1.0f, out gameVerts, out gameInd);
            //генерирует вершины и индексы для текстурирования. Далее натягиваем 
            for (int i = 0; i < gameVerts.Count; i++)
            {
                Vector3 n = gameVerts[i];
                //приведение длины вектора к 1.
                n.Normalize();
                float u = (float)(MathHelper.Atan2(n.X, n.Z) / (2 * MathHelper.Pi) + 0.5f);
                float v = n.Y * 0.5f + 0.5f;
                gameGraphic.Add(new Vector2(u, v));
            }
            BuildObject("wheel.jpg");
        }
        /*GenerateCoinMesh, который генерирует вершины и индексы для шара на основе заданного масштаба (scale), 
        количества сегментов по окружности (slices) и количества сегментов по высоте (stacks).
        Метод генерирует вершины, равномерно распределенные по сферической поверхности, 
        используя сферические координаты. Затем он генерирует индексы для соединения вершин в треугольники.*/
        private void GeneratePlayerMesh(float scale, out List<Vector3> vertices, out List<uint> indices)
        {
            vertices = new List<Vector3>();
            indices = new List<uint>();
            // Define the number of segments for the surface of the player mesh around the circumference
            int slices = 22;
            // Define the number of segments for the height of the player mesh
            int stacks = 10;
            // Generate vertices for the player mesh
            for (int i = 0; i <= stacks; i++)
            {
                // Calculate the vertical coordinate (v) from 0 to 1
                float v = (float)i / stacks;
                // Calculate the polar angle (phi) from 0 to 2pi
                float phi = v * (float)Math.PI;

                for (int j = 0; j <= slices; j++)
                {
                    // Calculate the horizontal coordinate (u) from 0 to 1
                    float u = (float)j / slices;
                    // Calculate the azimuthal angle (theta) from 0 to 2pi
                    float theta = u * 2f * (float)Math.PI;
                    // Calculate the x, y, z coordinates of the vertex
                    // using spherical coordinates
                    float x = (float)(Math.Cos(theta) * Math.Sin(phi));
                    float y = (float)Math.Cos(phi);
                    float z = (float)(Math.Sin(theta) * Math.Sin(phi));

                    vertices.Add(new Vector3(x, y, z) * scale);
                }
            }

            for (int i = 0; i < stacks; i++)
            {
                for (int j = 0; j < slices; j++)
                {
                    int first = (i * (slices + 1)) + j;
                    int second = first + slices + 1;

                    indices.Add((uint)first);
                    indices.Add((uint)second);
                    indices.Add((uint)(first + 1));

                    indices.Add((uint)second);
                    indices.Add((uint)(second + 1));
                    indices.Add((uint)(first + 1));
                }
            }
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

        static float sqrt2 = (float)((1 / MathHelper.Sqrt(2)) / 100);
        //sqrt - Она используется для определения движения шара в различных направлениях
        public Vector3 CentPos = new Vector3(0,-7,-20);//начальная позиция шара
       
        List<Vector3> vector = new List<Vector3> 
        //Каждый вектор задает направление движения
        //шара на одну из сторон квадрата, расположенного в плоскости XY.
        { new Vector3(sqrt2,sqrt2,0), 
          new Vector3(-sqrt2, sqrt2, 0), 
          new Vector3(-sqrt2, -sqrt2, 0), 
          new Vector3(sqrt2, -sqrt2, 0)
        };
        int move = 0;
        //move - переменная которая отслеживает текущее направление объекта

        //метод который меняет положение на основе вектора 
        public int ChangePosition(int coef=1)
        {
            CentPos += coef*vector[move];//считается смещение относвительно начальной позиции шара
            //Проверяется, находится ли абсолютное значение координаты X позиции шара(CentPos.X)
            //за пределами границ(больше или равно 11).
            if (MathHelper.Abs(CentPos.X)>=11)
            {
                //меняем направление движение шара
                ChangeMove();
                return 0;
            }
            if (MathHelper.Abs(CentPos.Y) >= 11)
            {
                ChangeMove();
                return 0;
            }
            return 0;
        }
        /*
        Это метод ChangeMove, который меняет текущее направление движения шара.
        Он увеличивает значение переменной move на 1 и берет остаток от деления на 4,
        чтобы циклически изменять направление движения шара.
         */
        public void ChangeMove()
        {
            move = (move + 1) % 4;
        }

    }

    
}
