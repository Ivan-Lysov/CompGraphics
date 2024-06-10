using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Arcanoid.Graphics;


namespace Arcanoid
{
    internal class Game : GameWindow
    {
        int Height;
        int Width;
        ShaderProgram program;
        float temp = 0;

        Ball ball;
        Platform platform;
        Brick brick;
        BackGround backGround;
        int[,] was =new int[3,10];

        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(width, height));
            Height = height;
            Width = width;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.Width = e.Width;
            this.Height = e.Height;
        }


        protected override void OnLoad()
        {
            base.OnLoad();
            ball = new Ball();
            platform = new Platform();
            brick = new Brick();
            backGround = new BackGround();
            program = new ShaderProgram("Default.vert", "Default.frag");
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            ball.Delete();
            platform.Delete();
            brick.Delete();
            backGround.Delete();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.5f, 0.6f, 0.3f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            KeyboardState input = KeyboardState;


            // transformation matrices
            Matrix4 model = Matrix4.Identity;
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), Width / Height, 0.1f, 100.0f);

            int modelLocation = GL.GetUniformLocation(program.ID, "model");
            int projectionLocation = GL.GetUniformLocation(program.ID, "projection");
            GL.UniformMatrix4(projectionLocation, true, ref projection);
            GL.UniformMatrix4(modelLocation, true, ref model);
            backGround.Render(program);
            for (float i = 0; i < 3; i++)
            {
                for  (float j = 0; j < 10; ++j)
                {
                    model = Matrix4.Identity;
                    model *= Matrix4.CreateTranslation(-11 + 4 * j + 1, 11 - 4 * i - 1, 0);
                    GL.UniformMatrix4(modelLocation, true, ref model);
                    if (was[(int)i, (int)j] == 1) continue;
                    was[(int)i, (int)j] = CheckIntersec(-11 + 4 * j + 1, 11 - 4 * i - 1, ref ball);
                    if (was[(int)i, (int)j] == 1) continue;
                    brick.Render(program);
                }
            }
            temp += 0.02f;
            model = Matrix4.CreateRotationX(temp);
            model *= Matrix4.CreateTranslation(ball.CentPos);
            GL.UniformMatrix4(modelLocation, true, ref model);
            ball.Render(program);
            ball.ChangePosition();

            //temp += 0.002f;
            //model = Matrix4.CreateRotationY(temp);
            model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(platform.CentPos);
            GL.UniformMatrix4(modelLocation, true, ref model);
            platform.Render(program);
            ball.ChangePosition();
            platform.ChangePosition(ball,input);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        public int CheckIntersec(float x,float y,ref Ball ball)
        {
            Vector3 CentPos = new Vector3(x,y,0); 
            if (-2.2f + x <= ball.CentPos.X && ball.CentPos.X <= 2.2f + x)
            {
                if (dist(ball.CentPos, CentPos) <= 2.2f)
                {
                    ball.ChangeMove();
                    return 1;
                }
            }
            if (-1.2f + y <= ball.CentPos.Y && ball.CentPos.Y <= 1.2f + y)
            {
                if (dist(ball.CentPos, CentPos) <= 3.2f)
                {
                    ball.ChangeMove();
                    return 1;
                }
            }
            return 0;
            
        }
        float dist(Vector3 v, Vector3 u)
        {
            float x = (v.X - u.X) * (v.X - u.X) + (v.Y - u.Y) * (v.Y - u.Y);
            return (float)MathHelper.Sqrt(x);
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            KeyboardState input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }


    }


}