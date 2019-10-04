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
using System.Threading.Tasks;

namespace PracaInzynierska
{
    class MainWindow : GameWindow
    {
        int[] vertexBufferObject;
        int[] vertexArrayObject;
        int[] elementBufferObject;
        Shader[] shader;

        private Shader[] normalsShader;
        private bool toggleNormals = false;

        Camera camera;
        float time;
        bool firstMove = true;
        Vector2 lastPos;

        Vector3 ambientLightColor = new Vector3(1f,1f,1f);
        private Vector3 lightPosition = new Vector3(50f, 100f, 50f);
        private float ambientStrength = 0.05f;

        private uint resolution = 16;
        private uint size = 5;
        private uint meshCount = 30;

        private MeshesController meshController;

        public MainWindow(int width, int height, string title) : base(width,height,GraphicsMode.Default, title) { }

        protected override void OnLoad(EventArgs e)
        {
            vertexBufferObject = new int[meshCount];
            vertexArrayObject = new int[meshCount];
            elementBufferObject = new int[meshCount];

            shader = new Shader[meshCount];
            normalsShader = new Shader[meshCount];

            for (int i = 0; i < meshCount; i++)
            {
                vertexArrayObject[i] = GL.GenVertexArray();
            }

            

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            /*log.WriteToConsole("Mesh size: " + size);
            log.WriteToConsole("Mesh resolution: " + resolution);
            log.WriteToConsole("Mesh tris: " + mesh.getIndices().Length/3.0f);
            log.WriteToConsole("Normals: " + (toggleNormals ? "ON" : "OFF"));*/
            meshController = new MeshesController(resolution, size, 3, camera);

            for (int i = 0; i < meshCount; i++)
            {
                vertexBufferObject[i] = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject[i]);
                GL.BufferData(BufferTarget.ArrayBuffer, meshController.meshes[i].getVerticesData().Length * Marshal.SizeOf(typeof(Vector3)), meshController.meshes[i].getVerticesData(), BufferUsageHint.StaticDraw);

                elementBufferObject[i] = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject[i]);
                GL.BufferData(BufferTarget.ElementArrayBuffer,  meshController.meshes[i].getIndices().Length * sizeof(uint), meshController.meshes[i].getIndices(), BufferUsageHint.StaticDraw);

                shader[i] = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");
                normalsShader[i] = new Shader("../../Shaders/normalsShader.vert", "../../Shaders/normalsShader.frag", "../../Shaders/normalsShader.geom");

                shader[i].Use();

                GL.BindVertexArray(vertexArrayObject[i]);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject[i]);

                GL.EnableVertexAttribArray(shader[i].GetAttribLocation("aPosition"));
                GL.VertexAttribPointer(shader[i].GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, 0);

                GL.EnableVertexAttribArray(shader[i].GetAttribLocation("aNormal"));
                GL.VertexAttribPointer(shader[i].GetAttribLocation("aNormal"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, sizeof(float) * 3);

                GL.EnableVertexAttribArray(shader[i].GetAttribLocation("aColor"));
                GL.VertexAttribPointer(shader[i].GetAttribLocation("aColor"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, sizeof(float) * 6);

                //INIT NORMALS SHADER
                normalsShader[i].Use();

                GL.EnableVertexAttribArray(normalsShader[i].GetAttribLocation("aPosition"));
                GL.VertexAttribPointer(normalsShader[i].GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, 0);

                GL.EnableVertexAttribArray(normalsShader[i].GetAttribLocation("aNormal"));
                GL.VertexAttribPointer(normalsShader[i].GetAttribLocation("aNormal"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, sizeof(float) * 3);
            }

            GL.BindVertexArray(0);

            camera = new Camera(Vector3.UnitZ * 3, Width / (float)Height);

            CursorVisible = false;
            
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            time += 2.0f * (float)e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int i = 0; i < meshCount; i++)
            {

                shader[i].SetMatrix4("model", Matrix4.Identity);
                normalsShader[i].SetMatrix4("model", Matrix4.Identity);

                GL.BindVertexArray(vertexArrayObject[i]);



                shader[i].Use();
                GL.DrawElements(PrimitiveType.Triangles, meshController.meshes[i].getIndices().Length, DrawElementsType.UnsignedInt, 0);
                shader[i].SetVector3("lightColor", ambientLightColor);
                shader[i].SetFloat("ambientStrength", ambientStrength);
                shader[i].SetVector3("lightPos", lightPosition);


                if (toggleNormals)
                {

                    normalsShader[i].Use();
                    GL.DrawElements(PrimitiveType.Triangles, meshController.meshes[i].getIndices().Length, DrawElementsType.UnsignedInt, 0);


                }


                shader[i].SetMatrix4("view", camera.GetViewMatrix());
                shader[i].SetMatrix4("projection", camera.GetProjectionMatrix());

                normalsShader[i].SetMatrix4("view", camera.GetViewMatrix());
                normalsShader[i].SetMatrix4("projection", camera.GetProjectionMatrix());
            }


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
            if (input.IsKeyDown(Key.F))
                lightPosition = camera.Position;

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
            for (int i = 0; i < meshCount; i++)
            {
                GL.DeleteBuffer(vertexBufferObject[i]);
                GL.DeleteVertexArray(vertexArrayObject[i]);
                GL.DeleteShader(shader[i].Handle);
                GL.DeleteShader(normalsShader[i].Handle);
                shader[i].Dispose();
                normalsShader[i].Dispose();
            }
            
            GL.BindVertexArray(0);
            
            GL.UseProgram(0);
            
            base.OnUnload(e);
        }
    }
}
