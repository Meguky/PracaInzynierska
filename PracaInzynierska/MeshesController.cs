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

        public List<Mesh> meshes;
        private uint renderDistance;
        private uint meshResolution;
        private uint meshSize;
        private Camera camera;
        public List<Vector3> meshesOriginPointGrid;
        public List<Vector3> lastMeshesOriginPointGrid;
        public List<Vector3> meshesToAddOriginPoints;
        private bool active = true;
        private Shader shader;
        private Shader normalsShader;

        public MeshesController(uint _meshResolution, uint _meshSize, uint _renderDistance, Camera _camera)
        {
            renderDistance = _renderDistance;
            camera = _camera;
            meshResolution = _meshResolution;
            meshSize = _meshSize;

            meshes = new List<Mesh>();
            lastMeshesOriginPointGrid = new List<Vector3>();
            meshesOriginPointGrid = new List<Vector3>();
            meshesToAddOriginPoints = new List<Vector3>();

            shader = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");
            normalsShader = new Shader("../../Shaders/normalsShader.vert", "../../Shaders/normalsShader.frag", "../../Shaders/normalsShader.geom");

            generateGrid();

            Mesh mesh;
            OpenSimplexNoise n = new OpenSimplexNoise();

            for (int i = 0; i < meshesOriginPointGrid.Count; i++)
            {
                mesh = new Mesh(meshResolution, meshSize, meshesOriginPointGrid[i], new Vector3(1.0f, 1.0f, 1.0f), shader, normalsShader);
                mesh.generateMesh();
                float[] noiseValues = n.getNoise(mesh.getVertices(), 0.5f, 0);
                mesh.applyNoise(noiseValues);
                meshes.Add(mesh);
            }

            //Thread t = new Thread(new ThreadStart(generationLoop));
            //t.Start();

        }
        private void generationLoop()
        {
            while (active)
            {
                Console.Write("Generating meshes...");
                generateGrid();
                Thread.Sleep(5000);
            }
        }

        public void generateGrid()
        {

            float cameraOffsetX;
            if (camera.Position.X < 0)
            {
                cameraOffsetX = camera.Position.X - (meshSize - Math.Abs(camera.Position.X) % meshSize);
            }
            else
            {
                cameraOffsetX = camera.Position.X - camera.Position.X % meshSize;
            }

            float cameraOffsetZ;
            if (camera.Position.Z < 0)
            {
                cameraOffsetZ = camera.Position.Z - (meshSize - Math.Abs(camera.Position.Z) % meshSize);
            }
            else
            {
                cameraOffsetZ = camera.Position.Z - camera.Position.Z % meshSize;
            }

            Vector3 cameraNearestMesh = new Vector3( cameraOffsetX, camera.Position.Y, cameraOffsetZ);

            meshesOriginPointGrid.Clear();

            for (int i = 0; i < renderDistance; i++)
            {
                for (int j = 0; j < renderDistance; j++)
                {
                    meshesOriginPointGrid.Add(new Vector3((j * meshSize) - meshSize * renderDistance / 2 + cameraNearestMesh.X, 0, (i * meshSize) - meshSize * renderDistance / 2 + cameraNearestMesh.Z));
                }
            }

            if (lastMeshesOriginPointGrid.Count != 0 && !lastMeshesOriginPointGrid.SequenceEqual(meshesOriginPointGrid))
            {
                foreach (Vector3 lastPoint in meshesOriginPointGrid)
                {
                    bool isInCollection = false;
                    for (int i = 0; i < lastMeshesOriginPointGrid.Count; i++)
                    {
                        if (lastMeshesOriginPointGrid[i] == lastPoint)
                        {
                            isInCollection = true;
                            break;
                        }
                    }

                    if (!isInCollection)
                    {
                        meshesToAddOriginPoints.Add(lastPoint);
                        Console.WriteLine(meshesToAddOriginPoints.Count);
                    }
                }

                
                applyMeshes();
            }

            lastMeshesOriginPointGrid.Clear();

            foreach (Vector3 point in meshesOriginPointGrid)
            {
                lastMeshesOriginPointGrid.Add(point);
            }

        }

        private void applyMeshes()
        {
            Mesh mesh;
            OpenSimplexNoise n = new OpenSimplexNoise();

            for (int i = meshesOriginPointGrid.Count - 1; i > 0; i--)
            {
                mesh = new Mesh(meshResolution, meshSize, meshesOriginPointGrid[i], new Vector3(1.0f, 1.0f, 1.0f), shader, normalsShader);
                mesh.generateMesh();
                float[] noiseValues = n.getNoise(mesh.getVertices(), 0.5f, 0);
                mesh.applyNoise(noiseValues);
                meshes.Add(mesh);
                
            }
            meshesOriginPointGrid.Clear();
        }

        public void drawAllMeshes(bool toggleNormals, Vector3 ambientLightColor, float ambientStrength, Vector3 lightPosition)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].draw(toggleNormals, ambientLightColor, ambientStrength, lightPosition, camera);
            }
        }

        public void deleteMeshesGLData()
        {
            foreach (Mesh mesh in meshes)
            {
                mesh.deleteGLStructures();
            }

            active = false;
        }
    }
}
