using Arcanoid.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;


namespace Arcanoid
{
    // Класс для создания и управления фоном в игре
    internal class BackGround
    {
        // Списки для хранения вершин, текстурных координат и индексов
        public List<Vector3> gameVerts;
        public List<Vector2> gameGraphic = new List<Vector2>();
        public List<uint> gameInd;

        // Объекты для работы с OpenGL: VAO, VBO, IBO и текстура
        VAO gameVAO;
        VBO gameVertexVBO;
        VBO gameGraphicVBO;
        IBO gameIBO;

        Texture texture;

        // Конструктор, который создает куб и загружает текстуру
        public BackGround()
        {
                MakeCube();
                BuildObject("b8b736bf7fec948155504401851f4719.jpeg");
        }

        // Метод для создания вершин и текстурных координат куба
        public void MakeCube()
        {
            gameVerts = new List<Vector3>()
            {
            new Vector3(-30f, 30f, -30f),
            new Vector3(30f, 30f, -30f),
            new Vector3(30f, -30f, -30f),
            new Vector3(-30f, -30f, -30f),

            new Vector3(30f, 30f, -30f),
            new Vector3(30f, 30f, -30f),
            new Vector3(30f, -30f, -30f),
            new Vector3(30f, -30f, -30f),

            new Vector3(30f, 30f, -30f),
            new Vector3(-30f, 30f, -30f),
            new Vector3(-30f, -30f, -30f),
            new Vector3(30f, -30f, -30f),

            new Vector3(-30f, 30f, -30f),
            new Vector3(-30f, 30f, -30f),
            new Vector3(-30f, -30f, -30f),
            new Vector3(-30f, -30f, -30f),

            new Vector3(-30f, 30f, -30f),
            new Vector3(30f, 30f, -30f),
            new Vector3(30f, 30f, -30f),
            new Vector3(-30f, 30f, -30f),

            new Vector3(-30f, -30f, -30f),
            new Vector3(30f, -30f, -30f),
            new Vector3(30f, -30f, -30f),
            new Vector3(-30f, -30f, -30f),
            };

            // Список текстурных координат для каждой вершины
            gameGraphic = new List<Vector2>()
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
        }
        // Метод для построения объекта: связывает вершинные и текстурные VBO, IBO и загружает текстуру
        public void BuildObject(String path)
        {
            /*
            Создаётся объект VAO и сохраняется его идентификатор.
            Привязка VAO для последующей настройки.
             */
            gameVAO = new VAO();
            gameVAO.Bind();

            /*
             Создаётся объект VBO для вершин с передачей списка вершин.
            Привязка VBO для передачи данных в видеопамять.
            Настройка VAO для использования этого VBO. LinkToVAO указывает, 
            что данные расположены в позиции 0, используются 3 компонента на вершину (x, y, z).
            */
            gameVertexVBO = new VBO(gameVerts);
            /*
             Создаётся новый объект VBO, который инициализируется 
             списком вершин gameVerts. В конструкторе VBO вызывается GL.GenBuffer()
             для передачи данных вершин в этот буфер.
            */
            gameVertexVBO.Bind();
            gameVAO.LinkToVAO(0, 3, gameVertexVBO);
            /*подключаем к VAO 
           Параметры:
            атрибут шейдера
            количество компонентов на одну вершину (в данном случае 3: x, y, z).
            объект VBO, содержащий данные вершин 
             */
            gameGraphicVBO = new VBO(gameGraphic);
            gameGraphicVBO.Bind();
            gameVAO.LinkToVAO(1, 2, gameGraphicVBO);

            gameIBO = new IBO(gameInd);

            texture = new Texture(path);
        }
        // Метод для отрисовки объекта с использованием шейдерной программы
        public void Render(ShaderProgram program)
        {
            program.Bind();
            gameVAO.Bind();
            gameIBO.Bind();
            texture.Bind();
            GL.DrawElements(PrimitiveType.Triangles, gameInd.Count, DrawElementsType.UnsignedInt, 0);
            /*
            рисуем примитивы: треугольники
            Количество элементов (индексов), которые будут использованы для отрисовки.
            Тип данных индексов в буфере индексов. 
            Смещение в буфере индексов. 
             */
        }
        // Метод для удаления всех OpenGL объектов
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