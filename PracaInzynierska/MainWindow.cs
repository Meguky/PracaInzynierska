using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska
{
    class MainWindow : GameWindow
    {


        uint[] indices = {
            0, 1, 3,
            1, 2, 3
        };

        float[] vertices = {
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

        float[] texCoords = {
            0.0f, 0.0f,
            1.0f, 0.0f,
            0.5f, 1.0f
        };

        int vertexBufferObject;
        int vertexArrayObject;
        int elementBufferObject;
        Shader shader;
        Texture texture;
        public MainWindow(int width, int height, string title) : base(width,height,GraphicsMode.Default, title) { }

        protected override void OnLoad(EventArgs e)
        {

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            shader = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");
            shader.Use();

            texture = new Texture("../../container.png");
            texture.Use();

            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);

            GL.EnableVertexAttribArray(shader.GetAttribLocation("aPosition"));
            GL.VertexAttribPointer(shader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
           
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            texture.Use();
            shader.Use();
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            if (Keyboard.GetState().IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferObject);
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayObject);
            GL.UseProgram(0);
            GL.DeleteShader(shader.Handle);
            GL.DeleteTexture(texture.Handle);
            shader.Dispose();
            base.OnUnload(e);
        }
    }
}
