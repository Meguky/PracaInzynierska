using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace PracaInzynierska
{
    public class MeshesController
    {

        public Mesh[] meshes;
        private uint renderDistance;
        private uint meshResolution;
        private uint meshSize;
        private Camera camera;
        public Vector3[] meshesOriginPointGrid;
        public Vector3[] lastMeshesOriginPointGrid;
        public Vector3[] meshesToAddOriginPoints;


        public MeshesController(uint _meshResolution, uint _meshSize, uint _renderDistance, Camera _camera)
        {
            renderDistance = _renderDistance;
            camera = _camera;
            meshResolution = _meshResolution;
            meshSize = _meshSize;

            generateGrid();

            meshes = new Mesh[meshesOriginPointGrid.Length];

            Mesh mesh;
            OpenSimplexNoise n = new OpenSimplexNoise();


            for (int i = 0; i < meshesOriginPointGrid.Length; i++)
            {
                mesh = new Mesh(meshResolution, meshSize, meshesOriginPointGrid[i], new Vector3(1.0f, 1.0f, 1.0f));
                mesh.generateMesh();
                float[] noiseValues = n.getNoise(mesh.getVertices(), 0.5f, 0);
                mesh.applyNoise(noiseValues);
                meshes[i] = mesh;
            }

        }

        private void generateGrid()
        {

            float cameraOffsetX = camera.Position.X % meshSize;
            float cameraOffsetZ = camera.Position.Z % meshSize;
            Vector3 cameraNearestMesh = new Vector3(camera.Position.X - cameraOffsetX, camera.Position.Y, camera.Position.Z - cameraOffsetZ);

            meshesOriginPointGrid = new Vector3[renderDistance * renderDistance];

            for (int i = 0; i < renderDistance; i++)
            {
                for (int j = 0; j < renderDistance; j++)
                {
                    meshesOriginPointGrid[j + i * renderDistance] = new Vector3((j * meshSize) - meshSize * renderDistance / 2.0f + cameraNearestMesh.X, 0, (i * meshSize) - meshSize * renderDistance / 2.0f + cameraNearestMesh.Z);
                }
            }
        }

        public void drawAllMeshes(bool toggleNormals, Vector3 ambientLightColor, float ambientStrength, Vector3 lightPosition)
        {
            foreach (Mesh mesh in meshes)
            {
                mesh.draw(toggleNormals, ambientLightColor, ambientStrength, lightPosition, camera);
            }
        }

        public void deleteMeshesGLData()
        {
            foreach (Mesh mesh in meshes)
            {
                mesh.deleteGLStructures();
            }
        }
    }
}
