using Arcanoid.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Arcanoid.Graphics
{
    //Создает внутренний класс VAO, который будет использоваться для работы с объектами Vertex Array Object(VAO) в OpenGL.
    internal class VAO
    {
        //Объявляет публичное поле ID, которое будет хранить идентификатор VAO.
        public int ID;
        
        public VAO()
        {
            ID = GL.GenVertexArray();//Генерирует новый VAO и сохраняет его идентификатор в поле ID.
            GL.BindVertexArray(ID);// Привязывает VAO с этим идентификатором. 
        }
        public void LinkToVAO(int location, int size, VBO vbo)
        {
            Bind(); //Привязывает VAO, чтобы последующие операции были применены к нему.
            vbo.Bind();//Привязывает VBO, чтобы данные из него могли быть использованы.
            GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(location);//Включает использование атрибута вершины.
            Unbind();
        }
        public void Bind() { GL.BindVertexArray(ID); }//Привязывает текущий VAO.
        public void Unbind() { GL.BindVertexArray(0); } //Отвязывает текущий VAO, устанавливая привязку как 0 (нулевой VAO).
        public void Delete() { GL.DeleteVertexArray(ID); }//Удаляет VAO с использованием его идентификатора ID.
    }
}