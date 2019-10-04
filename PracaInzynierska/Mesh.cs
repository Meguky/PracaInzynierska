using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private uint Resolution { get; set; }
        private uint Size { get; set; }

        private float unitSize;

        public Mesh()
        {

        }
        public Mesh(uint _resolution, uint _size, Vector3 _origin, Vector3 _color)
        {
            if (_resolution == 0 || _size == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            Resolution = _resolution;
            Size = _size;
            originPoint = _origin;

            vertices = new Vector3[(Resolution + 1) * (Resolution + 1)];

            normals = new Vector3[vertices.Length];

            colors = new Vector3[vertices.Length];

            Random rand = new Random();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = _color;
            }

            verticesData = new Vector3[vertices.Length,3];

            indices = new uint[Resolution * Resolution * 2 * 3];

            unitSize = Size / (float) Resolution;

            middlePoint.X = originPoint.X +  Size / 2;
            middlePoint.Y = originPoint.Y;
            middlePoint.Z = originPoint.Z + Size / 2;
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
            }

            calculateNormals();
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
            }

            calculateNormals();
        }

        public Vector3[,] getVerticesData()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                verticesData[i, 0] = vertices[i];
                verticesData[i, 1] = normals[i];
                verticesData[i, 2] = colors[i];
            }

            return verticesData;
        }

        public uint[] getIndices()
        {
            return indices;
        }

        public Vector3[] getVertices()
        {
            return vertices;
        }

    }
}
