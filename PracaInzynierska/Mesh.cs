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

        public Vertex[] vertices;
        public float[] verticesFloats;
        public uint[] indices;

        public void generateMesh(uint resolution, int size)
        {
            if (resolution == 0 || size == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            vertices = new Vertex[(resolution + 1) * (resolution + 1)];
            indices = new uint[resolution * resolution * 2 * 3];
            verticesFloats = new float[(resolution + 1) * (resolution + 1) * 3];
            float unitSize = size / (float)resolution;

            //Generate vertices (Vertices array)
            int vertexIndex = 0;
            float z = 0;
            for(int i = 0; i < resolution + 1; i++)
            {
                float x = 0;
                for (int j = 0; j < resolution + 1; j++)
                {
                    vertices[vertexIndex] = new Vertex(x, 0f, z);
                    x += unitSize;
                    vertexIndex++;
                }
                z += unitSize;
            }

            //Pure float array generation
            /*vertexIndex = 0;
            z = 0;
            for (int i = 0; i < resolution + 1; i++)
            {
                float x = 0;
                for (int j = 0; j < resolution + 1; j++)
                {
                    verticesFloats[vertexIndex] = x;
                    verticesFloats[vertexIndex + 1] = -1f;
                    verticesFloats[vertexIndex + 2] = z;
                    x += unitSize;
                    vertexIndex += 3;
                }
                z += unitSize;
            }*/

            //Generate indices
            uint indiceIndex = 0;
            uint indiceOffset = 0;
            for(uint i = 0; i < resolution; i++)
            {
                for(uint j = 0; j < resolution; j++)
                {
                    //First triangle
                    indices[indiceIndex] = j + resolution + 2 + indiceOffset;
                    indices[indiceIndex + 1] = j + resolution + 1 + indiceOffset;
                    indices[indiceIndex + 2] = j + 1 + indiceOffset;
                    //Second triangle
                    indices[indiceIndex + 3] = j + 1 + resolution + indiceOffset;
                    indices[indiceIndex + 4] = j + indiceOffset;
                    indices[indiceIndex + 5] = j + 1 + indiceOffset;
                    indiceIndex += 6;
                }

                indiceOffset += resolution + 1;
            }
        }

        public Vertex[] getVertices()
        {
            return vertices;
        }

        public uint[] getIndices()
        {
            return indices;
        }

        public float[] getVerticesFloats()
        {
            return verticesFloats;
        }
    }
}
