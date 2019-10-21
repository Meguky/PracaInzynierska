using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public List<Mesh> meshesToAdd;
        public List<Mesh> meshesToDelete;
        public uint renderDistance;
        public uint meshResolution;
        public uint meshSize;
        public Camera camera;
        public bool active = true;
        private List<Vector3> meshesOriginPointGrid;
        private Shader shader;
        private Shader normalsShader;
        private readonly object meshesLock = new object();
        private readonly object renderDistanceLock = new object();

        private long openSimplexNoiseSeed = 0934580934580934509;
        private Thread generationThread;
        public MeshesController(uint _meshResolution, uint _meshSize, uint _renderDistance, Camera _camera)
        {
            renderDistance = _renderDistance;
            camera = _camera;
            meshResolution = _meshResolution;
            meshSize = _meshSize;

            meshes = new List<Mesh>();
            meshesToAdd = new List<Mesh>();
            meshesToDelete = new List<Mesh>();
            meshesOriginPointGrid = new List<Vector3>();

            shader = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");
            normalsShader = new Shader("../../Shaders/normalsShader.vert", "../../Shaders/normalsShader.frag", "../../Shaders/normalsShader.geom");

            generationThread = new Thread(new ThreadStart(generationLoop));
            generationThread.Start();

        }

        public void changeResolution(uint _resolution)
        {
            active = false;
            generationThread.Join();

            deleteMeshesGLData();

            meshResolution = _resolution;

            initAfterParameterChange();
        }

        public void changeSize(uint _size)
        {
            active = false;
            generationThread.Join();

            deleteMeshesGLData();

            meshSize = _size;

            initAfterParameterChange();
        }

        public void changeRenderDistance(uint _renderDistance)
        {
            lock (renderDistanceLock)
            {
                renderDistance = _renderDistance;
            }
        }

        private void initAfterParameterChange()
        {
            meshes = new List<Mesh>();
            meshesToAdd = new List<Mesh>();
            meshesToDelete = new List<Mesh>();
            meshesOriginPointGrid = new List<Vector3>();

            active = true;

            generationThread = new Thread(new ThreadStart(generationLoop));
            generationThread.Start();
        }

        private void generationLoop()
        {
            while (active)
            {
                generateGrid();
                generateMeshes();
                generateDeleteGrid();
                Thread.Sleep(1000);
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

            uint gridRenderDistance;
            lock (renderDistanceLock)
            {
                gridRenderDistance = renderDistance + 2;
            }

            for (int i = 0; i < gridRenderDistance; i++)
            {
                for (int j = 0; j < gridRenderDistance; j++)
                {
                    meshesOriginPointGrid.Add(new Vector3((j * meshSize) - meshSize * gridRenderDistance / 2 + cameraNearestMesh.X, 0, (i * meshSize) - meshSize * gridRenderDistance / 2 + cameraNearestMesh.Z));
                }
            }
        }

        public void generateMeshes()
        {
            bool meshIsOnMap = false;
            OpenSimplexNoise n = new OpenSimplexNoise(openSimplexNoiseSeed);

            for (int i = 0; i < meshesOriginPointGrid.Count; i++)
            {
                lock (meshesLock)
                {
                    foreach (Mesh mesh in meshes)
                    {
                        meshIsOnMap = false;

                        if (mesh.originPoint == meshesOriginPointGrid[i])
                        {
                            meshIsOnMap = true;
                            break;
                        }
                    }
                }

                if (!meshIsOnMap && (meshesOriginPointGrid[i] - camera.Position).Length < (float) meshSize * renderDistance)
                {
                    Mesh meshToAdd = new Mesh(meshResolution, meshSize, meshesOriginPointGrid[i], new Vector3(1.0f, 1.0f, 1.0f), shader, normalsShader);
                    meshToAdd.generateMesh();
                    float[] noiseValues = n.getNoise(meshToAdd.getVertices(), 0.5f);
                    meshToAdd.applyNoise(noiseValues);
                    meshesToAdd.Add(meshToAdd);
                }
            }

            meshesOriginPointGrid.Clear();
        }

        public void generateDeleteGrid()
        {
            lock (meshesLock)
            {
                meshesToDelete = meshes.Where(mesh => (mesh.middlePoint - camera.Position).Length > (float) meshSize * renderDistance).ToList();
            }
        }

        public void applyMeshes()
        {
            lock (meshesLock)
            {
                for (int i = 0; i < meshesToDelete.Count; i++)
                {
                    meshesToDelete[i].deleteGLStructures();
                    meshes.Find(mesh => mesh == meshesToDelete[i]).deleteGLStructures();
                    meshes.Remove(meshesToDelete[i]);
                }
                
                for (int i = 0; i < meshesToAdd.Count; i++)
                {
                    meshes.Add(meshesToAdd[i]);
                }
            }
            meshesToAdd.Clear();
            meshesToDelete.Clear();
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
