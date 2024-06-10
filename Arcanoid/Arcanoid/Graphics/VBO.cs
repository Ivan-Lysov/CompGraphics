using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;


namespace Arcanoid.Graphics
{
    internal class VBO
    {
        // Идентификатор буфера
        public int ID; //ID - хранит идентификатор буфера
        // Конструктор, принимающий список индексов
        public VBO(List<Vector3> data)
        {
            // Генерация буфера и сохранение его ID
            ID = GL.GenBuffer();
            // Привязка буфера как элементного массива
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
            // Передача данных в буфер
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Vector3.SizeInBytes, data.ToArray(), BufferUsageHint.StaticDraw);
            /*Параметры: тип буфера(массив вершин),размер хранилища (число элементов в списке * размер одного элемента типа Vector3 в байтах,
               data.ToArray(), что преобразует список data в массив и передает указатель на этот массив. , Указывает, как данные будут использоваться. */
        }
        public VBO(List<Vector2> data)
        {
            ID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Vector2.SizeInBytes, data.ToArray(), BufferUsageHint.StaticDraw);
        }
        // Метод для привязки буфера
        public void Bind() { GL.BindBuffer(BufferTarget.ArrayBuffer, ID); }
        // Метод для отвязки буфера
        public void Unbind() { GL.BindBuffer(BufferTarget.ArrayBuffer, 0); }
        // Метод для удаления буфера
        public void Delete() { GL.DeleteBuffer(ID); }
    }
}