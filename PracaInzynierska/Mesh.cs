using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska
{
    public class Mesh
    {

        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector3[] colors;
        private Vector3[,] verticesData;
        private uint[] indices;

        public Vector3 originPoint;
        public Vector3 middlePoint;

        int vertexBufferObject;
        int vertexArrayObject;
        int elementBufferObject;
        Shader shader;
        Shader normalsShader;
        private string shaderFolderPath;

        private uint Resolution { get; set; }
        private uint Size { get; set; }

        private float unitSize;

        public Mesh(uint _resolution, uint _size, Vector3 _origin, Vector3 _color, string _shaderFolderPath)
        {
            if (_resolution == 0 || _size == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            Resolution = _resolution;
            Size = _size;
            originPoint = _origin;
            shaderFolderPath = _shaderFolderPath;

            vertices = new Vector3[(Resolution + 1) * (Resolution + 1)];

            normals = new Vector3[vertices.Length];

            colors = new Vector3[vertices.Length];

            verticesData = new Vector3[vertices.Length, 3];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = _color;
                verticesData[i, 2] = _color;
            }

            indices = new uint[Resolution * Resolution * 2 * 3];

            unitSize = Size / (float) Resolution;

            middlePoint.X = originPoint.X +  Size / 2;
            middlePoint.Y = originPoint.Y;
            middlePoint.Z = originPoint.Z + Size / 2;

            generateMesh();

            bindBuffers();
            
        }

        public void generateMesh()
        {

            //Generate vertices
            int vertexIndex = 0;
            float z = 0;
            for(int i = 0; i < Resolution + 1; i++)
            {
                float x = 0;
                for (int j = 0; j < Resolution + 1; j++)
                {
                    vertices[vertexIndex] = new Vector3(x, 0f, z) + originPoint;
                    verticesData[vertexIndex, 0] = new Vector3(x, 0f, z) + originPoint;
                    x += unitSize;
                    vertexIndex++;
                }
                z += unitSize;
            }

            //Generate indices
            uint indiceIndex = 0;
            uint indiceOffset = 0;
            for(uint i = 0; i < Resolution; i++)
            {
                for(uint j = 0; j < Resolution; j++)
                {
                    //First triangle
                    indices[indiceIndex] = j + Resolution + 2 + indiceOffset;
                    indices[indiceIndex + 1] = j + Resolution + 1 + indiceOffset;
                    indices[indiceIndex + 2] = j + 1 + indiceOffset;
                    //Second triangle
                    indices[indiceIndex + 3] = j + 1 + Resolution + indiceOffset;
                    indices[indiceIndex + 4] = j + indiceOffset;
                    indices[indiceIndex + 5] = j + 1 + indiceOffset;
                    indiceIndex += 6;
                }

                indiceOffset += Resolution + 1;
            }

            calculateNormals();
        }

        private void calculateNormals()
        {
            int triangleIndex = 0;
            for (int i = 0; i < normals.Length - 2 ; i++)
            {
                Vector3 A = vertices[indices[triangleIndex]];
                Vector3 B = vertices[indices[triangleIndex + 1]];
                Vector3 C = vertices[indices[triangleIndex + 2]];

                Vector3 norm = Vector3.Normalize(Vector3.Cross( C - A, B - A));

                normals[i] += norm;
                normals[i + 1] += norm;
                normals[i + 2] += norm;

                normals[i] = Vector3.Normalize(normals[i]);
                normals[i + 1] = Vector3.Normalize(normals[i + 1]);
                normals[i + 2] = Vector3.Normalize(normals[i + 2]);

                verticesData[i, 1] += norm;
                verticesData[i + 1, 1] += norm;
                verticesData[i + 2, 1] += norm;

                verticesData[i, 1] = Vector3.Normalize(verticesData[i, 1]);
                verticesData[i + 1, 1] = Vector3.Normalize(verticesData[i + 1, 1]);
                verticesData[i + 2, 1] = Vector3.Normalize(verticesData[i + 2, 1]);

                triangleIndex += 3;
            }

        }

        public void applyNoise(float[] noise)
        {
            if(noise.Length != vertices.Length)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Y = noise[i];
                verticesData[i,0].Y = noise[i];
            }

            calculateNormals();

            bindBuffers(false);
        }

        public void addNoise(float[] noise, float strength)
        {
            if (noise.Length != vertices.Length)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Y += noise[i] * strength;
                verticesData[i, 0].Y += noise[i] * strength;
            }

            calculateNormals();

            bindBuffers(false);
        }

        public void draw(bool normalsActive, Vector3 ambientLightColor, float ambientStrength, Vector3 lightPosition, Camera camera)
        {
            shader.SetMatrix4("model", Matrix4.Identity);
            normalsShader.SetMatrix4("model", Matrix4.Identity);

            GL.BindVertexArray(vertexArrayObject);

            shader.Use();
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            shader.SetVector3("lightColor", ambientLightColor);
            shader.SetFloat("ambientStrength", ambientStrength);
            shader.SetVector3("lightPos", lightPosition);


            if (normalsActive)
            {

                normalsShader.Use();
                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);


            }


            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            normalsShader.SetMatrix4("view", camera.GetViewMatrix());
            normalsShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        }

        public Vector3[] getVertices()
        {
            return vertices;
        }

        public uint[] getIndices()
        {
            return indices;
        }

        private void bindBuffers(bool firstGeneration = true)
        {
            if (!firstGeneration)
            {
                GL.DeleteBuffer(vertexBufferObject);
                GL.DeleteBuffer(elementBufferObject);
                GL.DeleteVertexArray(vertexArrayObject);
            }
            else
            {
                shader = new Shader(shaderFolderPath + "/shader.vert", shaderFolderPath + "/shader.frag");
                normalsShader = new Shader(shaderFolderPath + "/normalsShader.vert", shaderFolderPath + "/normalsShader.frag", shaderFolderPath + "/normalsShader.geom");
            }

            vertexArrayObject = GL.GenVertexArray();
            vertexBufferObject = GL.GenBuffer();
            elementBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesData.Length * Marshal.SizeOf(typeof(Vector3)), verticesData, BufferUsageHint.StaticDraw);


            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);

            shader.Use();

            GL.EnableVertexAttribArray(shader.GetAttribLocation("aPosition"));
            GL.VertexAttribPointer(shader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, 0);

            GL.EnableVertexAttribArray(shader.GetAttribLocation("aNormal"));
            GL.VertexAttribPointer(shader.GetAttribLocation("aNormal"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, sizeof(float) * 3);

            GL.EnableVertexAttribArray(shader.GetAttribLocation("aColor"));
            GL.VertexAttribPointer(shader.GetAttribLocation("aColor"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, sizeof(float) * 6);

            //INIT NORMALS SHADER
            normalsShader.Use();

            GL.EnableVertexAttribArray(normalsShader.GetAttribLocation("aPosition"));
            GL.VertexAttribPointer(normalsShader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, 0);

            GL.EnableVertexAttribArray(normalsShader.GetAttribLocation("aNormal"));
            GL.VertexAttribPointer(normalsShader.GetAttribLocation("aNormal"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 9, sizeof(float) * 3);

            GL.BindVertexArray(0);

        }

        public void deleteGLStructures()
        {
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteVertexArray(vertexArrayObject);
            GL.DeleteShader(shader.Handle);
            GL.DeleteShader(normalsShader.Handle);
            shader.Dispose();
            normalsShader.Dispose();
        }
    }
}
