using OpenTK.Graphics.OpenGL4;

namespace Arcanoid.Graphics
{
    // Класс для работы с индексным буфером (Index Buffer Object, IBO) в OpenGL.
    internal class IBO
    {
        // Идентификатор буфера.
        public int ID;
        // Конструктор, создающий IBO и загружающий данные в буфер.
        public IBO(List<uint> data)
        {
            // Генерация буфера и сохранение его идентификатора.
            ID = GL.GenBuffer();
            // Привязка буфера к цели ElementArrayBuffer.
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
            // Загрузка данных в буфер. 
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Count * sizeof(uint), data.ToArray(), BufferUsageHint.StaticDraw);
        }
        // Метод для привязки буфера
        public void Bind() { GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID); }
        // Метод для отвязки буфера.
        public void Unbind() { GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); }
        // Метод для удаления буфера и освобождения ресурсов.
        public void Delete() { GL.DeleteBuffer(ID); }
    }
}