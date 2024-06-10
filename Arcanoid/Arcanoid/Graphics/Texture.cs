using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Arcanoid.Graphics
{
    //Создает внутренний класс Texture, который будет использоваться
    //для работы с текстурами в OpenGL. Поле ID хранит идентификатор текстуры.
    internal class Texture
    {
        public int ID;

        public Texture(String filepath)
        {
            ID = GL.GenTexture();//Генерирует новую текстуру и сохраняет ее идентификатор в поле ID.

            GL.ActiveTexture(TextureUnit.Texture0);
            //Это позволяет работать с несколькими текстурами одновременно.
            GL.BindTexture(TextureTarget.Texture2D, ID);
            //Привязывает текстуру, чтобы все последующие операции с текстурами применялись к этой текстуре.

            // texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
            //GL.TexParameter:
            //TextureWrapS и TextureWrapT определяют режим обертки для координат S и T соответственно.
            //MirroredRepeat означает, что текстура будет повторяться, и каждый второй раз будет зеркально отражаться.
            //TextureMinFilter и TextureMagFilter определяют методы фильтрации текстуры.
            //Nearest означает, что будет использоваться ближайший сосед для текстурных выборок.

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);



            StbImage.stbi_set_flip_vertically_on_load(1);
            //станавливает флаг, который указывает,
            //что изображение должно быть перевернуто по вертикали при загрузке.
            ImageResult dirtTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/" + filepath), ColorComponents.RedGreenBlueAlpha);
            //Загружает изображение из файла и сохраняет его данные в объекте ImageResult.
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0 , PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);
            /*
             Параметры:
            TextureTarget.Texture2D: Тип текстуры.
            0: Уровень MIP-карты (0 означает базовый уровень).
            PixelInternalFormat.Rgba: Внутренний формат хранения.
            dirtTexture.Width и dirtTexture.Height: Размеры изображения.
            0: Граница текстуры (должна быть 0).
            PixelFormat.Rgba: Формат исходных данных.
            PixelType.UnsignedByte: Тип данных.
            dirtTexture.Data: Данные изображения.
             */

            // unbind the texture
            Unbind();
        }

        public void Bind() { GL.BindTexture(TextureTarget.Texture2D, ID); }
        public void Unbind() { GL.BindTexture(TextureTarget.Texture2D, 0); }
        public void Delete() { GL.DeleteTexture(ID); }
    }
}
