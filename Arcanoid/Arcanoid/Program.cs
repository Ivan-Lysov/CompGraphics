using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Arcanoid
{
    // Статический класс Program - точка входа в приложение.
    public static class Program
    {
        // Основной метод Main - точка входа в приложение.
        public static void Main()
        {
            // Создание и использование объекта Game с размерами окна 1000x1000 пикселей.
            using (Game game = new Game(1000, 1000))
            {
                // Запуск игры.
                game.Run();
            }
            // Объект Game будет автоматически утилизирован после завершения блока using.
        }
    }
}