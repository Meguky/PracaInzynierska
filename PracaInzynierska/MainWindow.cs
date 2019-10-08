using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PracaInzynierska
{
    class MainWindow : GameWindow
    {
        

        private bool toggleNormals = false;

        Camera camera;
        float time;
        bool firstMove = true;
        Vector2 lastPos;

        Vector3 ambientLightColor = new Vector3(1f,1f,1f);
        private Vector3 lightPosition = new Vector3(50f, 100f, 50f);
        private float ambientStrength = 0.05f;

        private uint resolution = 32;
        private uint size = 10;
        private uint renderDistance = 5;

        private MeshesController meshController;

        public MainWindow(int width, int height, string title) : base(width,height,GraphicsMode.Default, title) { }

        protected override void OnLoad(EventArgs e)
        {

            camera = new Camera(Vector3.UnitY * 3, Width / (float)Height);

            GL.ClearColor(0.2f, 0.6f, 1f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            /*log.WriteToConsole("Mesh size: " + size);
            log.WriteToConsole("Mesh resolution: " + resolution);
            log.WriteToConsole("Mesh tris: " + mesh.getIndices().Length/3.0f);
            log.WriteToConsole("Normals: " + (toggleNormals ? "ON" : "OFF"));*/

            meshController = new MeshesController(resolution, size, renderDistance, camera);

            CursorVisible = false;
            
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            time += 2.0f * (float)e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            meshController.drawAllMeshes(toggleNormals, ambientLightColor, ambientStrength, lightPosition);

            Title = "Generacja terenu " + (1f / e.Time).ToString("0.") + " FPS";

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            camera.AspectRatio = Width / (float)Height;

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused)
            {
                return;
            }

            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.W))
                camera.Position += camera.front * camera.speed * (float)e.Time; 
            if (input.IsKeyDown(Key.S))
                camera.Position -= camera.front * camera.speed * (float)e.Time;
            if (input.IsKeyDown(Key.A))
                camera.Position -= camera.right * camera.speed * (float)e.Time;
            if (input.IsKeyDown(Key.D))
                camera.Position += camera.right * camera.speed * (float)e.Time;
            if (input.IsKeyDown(Key.Space))
                camera.Position += camera.up * camera.speed * (float)e.Time;
            if (input.IsKeyDown(Key.LShift))
                camera.Position -= camera.up * camera.speed * (float)e.Time;
            if (input.IsKeyDown(Key.E))
                camera.speed = MathHelper.Clamp(camera.speed + 0.1f, 0, 10) ;
            if (input.IsKeyDown(Key.Q))
                camera.speed = MathHelper.Clamp(camera.speed - 0.1f, 0, 10);
            if (input.IsKeyDown(Key.G))
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            if (input.IsKeyDown(Key.B))
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            var mouse = Mouse.GetState();

            if (firstMove)
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - lastPos.X;
                var deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

                camera.Yaw += deltaX * camera.sensitivity;
                camera.Pitch -= deltaY * camera.sensitivity;
            }

            meshController.generateGrid();

            base.OnUpdateFrame(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.N))
            {
                toggleNormals = !toggleNormals;
            }

            base.OnKeyDown(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused)
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }
            base.OnMouseMove(e);
        }


        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            meshController.deleteMeshesGLData();

            GL.UseProgram(0);
            GL.BindVertexArray(0);

            base.OnUnload(e);
        }
    }
}
