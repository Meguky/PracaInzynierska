using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace PracaInzynierska
{
    class MeshesController
    {
        

        public Mesh[] meshes;
        private float renderDistance;
        private uint meshResolution;
        private uint meshSize;
        private Camera camera;
        public MeshesController(uint _meshResolution, uint _meshSize, float _renderDistance, Camera _camera)
        {
            renderDistance = _renderDistance;
            camera = _camera;
            meshResolution = _meshResolution;
            meshSize = _meshSize;
            
            meshes = new Mesh[30];

            OpenSimplexNoise n = new OpenSimplexNoise();

            for (int i = 0; i < 30; i++)
            {
                Mesh mesh;

                if (i < 10)
                {
                    mesh = new Mesh(meshResolution, meshSize, new Vector3(i * meshSize,0,0), new Vector3(1.0f, 0f, 0f) );
                    mesh.generateMesh();
                    meshes[i] = mesh;
                    float[] noiseValues = n.getNoise(meshes[i].getVertices(), 0.5f, meshSize * i);
                    mesh.applyNoise(noiseValues);
                }
                else if(i >= 10 && i < 20)
                {
                    mesh = new Mesh(meshResolution, meshSize, new Vector3((i - 10) * meshSize, 0, meshSize), new Vector3(0.0f, 1.0f, 0f));
                    mesh.generateMesh();
                    meshes[i] = mesh;
                    float[] noiseValues = n.getNoise(meshes[i].getVertices(), 0.5f, meshSize * i);
                    mesh.applyNoise(noiseValues);
                }
                else if (i >= 20 && i < 30)
                {
                    mesh = new Mesh(meshResolution, meshSize, new Vector3((i - 20) * meshSize, 0, meshSize * 2), new Vector3(0.0f, 0f, 1.0f));
                    mesh.generateMesh();
                    meshes[i] = mesh;
                    float[] noiseValues = n.getNoise(meshes[i].getVertices(), 0.5f, meshSize * i);
                    mesh.applyNoise(noiseValues);
                }
            }

        }
    }
}
