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

        public int[] indices;

        public void generateMesh(int resolution, int size)
        {
            vertices = new Vertex[(resolution + 1) * (resolution + 1)];
            indices = new int[resolution * resolution * 2 * 3];

            if(resolution == 0 || size == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            float unitSize = size / (float)resolution;
            //Generate vertices
            int vertexIndex = 0;
            float y = 0;
            for(int i = 0; i < resolution + 1; i++)
            {
                float x = 0;
                for (int j = 0; j < resolution + 1; j++)
                {
                    vertices[vertexIndex] = new Vertex(x, y, 0f);
                    x += unitSize;
                    vertexIndex++;
                }
                y += unitSize;
            }
            //Generate indices
            int indiceIndex = 0;
            int indiceOffset = 0;
            for(int i = 0; i < resolution; i++)
            {
                for(int j = 0; j < resolution; j++)
                {
                    //First triangle
                    indices[indiceIndex] = j + 1 + indiceOffset;
                    indices[indiceIndex + 1] = j + indiceOffset;
                    indices[indiceIndex + 2] = j + 4 + indiceOffset;
                    //Second triangle
                    indices[indiceIndex + 3] = j + indiceOffset;
                    indices[indiceIndex + 4] = j + 3 + indiceOffset;
                    indices[indiceIndex + 5] = j + 4 + indiceOffset;
                    indiceIndex += 6;
                }
                indiceOffset += 3;
                
            }
        }

        public Vertex[] getVertices()
        {
            return vertices;
        }

        public int[] getIndices()
        {
            return indices;
        }
    }
}
