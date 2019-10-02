using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska
{
    class MainWindow : GameWindow
    {
        int vertexBufferObject;
        int vertexArrayObject;
        int elementBufferObject;
        Shader shader;

        private Shader normalsShader;

        Camera camera;
        float time;
        bool firstMove = true;
        Vector2 lastPos;

        Vector3 ambientLightColor = new Vector3(1f,1f,1f);
        private Vector3 lightPosition = new Vector3(50f, 15f, 50f);
        private float ambientStrength = 0.05f;
        private Mesh mesh;
        private uint resolution = 100;
        private int size = 10;

        public MainWindow(int width, int height, string title) : base(width,height,GraphicsMode.Default, title) { }

        protected override void OnLoad(EventArgs e)
        {
            vertexArrayObject = GL.GenVertexArray();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            mesh = new Mesh(resolution, size);
            Noise noise = new Noise(resolution + 1, 0.01f);
            OpenSimplexNoise n = new OpenSimplexNoise();
            

            float[] noiseValues = n.getNoise(resolution, 0.2f);



            mesh.generateMesh();

            mesh.applyNoise(noiseValues);

            noiseValues = noise.GetNoise();
            mesh.addNoise(noiseValues, 1f);

            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.getVerticesData().Length * Marshal.SizeOf(typeof(Vector3)), mesh.getVerticesData(), BufferUsageHint.StaticDraw);

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.getIndices().Length * sizeof(uint), mesh.getIndices(), BufferUsageHint.StaticDraw);

            shader = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");
            normalsShader = new Shader("../../Shaders/normalsShader.vert", "../../Shaders/normalsShader.frag", "../../Shaders/normalsShader.geom");

            shader.Use();

            GL.BindVertexArray(vertexArrayObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            

            GL.EnableVertexAttribArray(shader.GetAttribLocation("aPosition"));
            GL.VertexAttribPointer(shader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, 0);

            GL.EnableVertexAttribArray(shader.GetAttribLocation("aNormal"));
            GL.VertexAttribPointer(shader.GetAttribLocation("aNormal"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, sizeof(float) * 3);

            GL.EnableVertexAttribArray(shader.GetAttribLocation("aColor"));
            GL.VertexAttribPointer(shader.GetAttribLocation("aColor"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, sizeof(float) * 6);

            normalsShader.Use();

            GL.EnableVertexAttribArray(normalsShader.GetAttribLocation("aPosition"));
            GL.VertexAttribPointer(normalsShader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, 0);

            GL.EnableVertexAttribArray(normalsShader.GetAttribLocation("aNormal"));
            GL.VertexAttribPointer(normalsShader.GetAttribLocation("aNormal"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, sizeof(float) * 3);

            GL.BindVertexArray(0);

            camera = new Camera(Vector3.UnitZ * 3, Width / (float)Height);

            CursorVisible = false;
            
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            time += 2.0f * (float)e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.BindVertexArray(vertexArrayObject);
            Matrix4 model;

            {
                model = Matrix4.Identity;
                shader.SetMatrix4("model", model);
                normalsShader.SetMatrix4("model", model);
                shader.Use();
                GL.DrawElements(PrimitiveType.Triangles, mesh.getIndices().Length, DrawElementsType.UnsignedInt, 0);
                normalsShader.Use();
                GL.DrawElements(PrimitiveType.Triangles, mesh.getIndices().Length, DrawElementsType.UnsignedInt, 0);

            }

            shader.SetVector3("lightColor", ambientLightColor);
            shader.SetFloat("ambientStrength", ambientStrength);
            shader.SetVector3("lightPos", lightPosition);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());
            normalsShader.SetMatrix4("view", camera.GetViewMatrix());
            normalsShader.SetMatrix4("projection", camera.GetProjectionMatrix());

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

            if (Keyboard.GetState().IsKeyDown(Key.Escape))
            {
                Exit();
            }

            var input = Keyboard.GetState();

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
                camera.speed += 0.5f;
            if (input.IsKeyDown(Key.Q))
                camera.speed -= 0.5f;
            if (input.IsKeyDown(Key.G))
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            if (input.IsKeyDown(Key.B))
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            if (input.IsKeyDown(Key.F))
            {
                lightPosition = camera.Position;
            }

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
            GL.DeleteBuffer(vertexBufferObject);
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayObject);
            GL.UseProgram(0);
            GL.DeleteShader(shader.Handle);
            GL.DeleteShader(normalsShader.Handle);
            shader.Dispose();
            normalsShader.Dispose();
            base.OnUnload(e);
        }
    }
}
