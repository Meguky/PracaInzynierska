using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PracaInzynierska
{
    class ConsoleLogger
    {
        private MeshesController meshesController;
        public bool active = true;
        public ConsoleLogger(MeshesController _meshesController)
        {
            meshesController = _meshesController;
            Thread consoleThread = new Thread(new ThreadStart(consoleThreadFunction));
            consoleThread.Start();
        }
        public void WriteInformation()
        {
            Console.WriteLine("---Mesh info---");
            Console.WriteLine("Mesh resolution: " + "                        ");
            Console.WriteLine("Mesh size: " + "                        ");
            Console.WriteLine("Mesh Triangles: " + "                        ");

            Console.WriteLine("---Map info---");
            Console.WriteLine("Total number of meshes: " + "                        ");
            Console.WriteLine("Render distance: " + "                        ");
            Console.WriteLine("Total number of meshes to delete: " + "                        ");
            Console.WriteLine("Total number of meshes to add: " + "                        ");
            Console.WriteLine("Total number of triangles: " + "                        ");

            Console.CursorLeft = 0;
            Console.CursorTop = 0;

            Console.WriteLine("---Mesh info---");
            Console.WriteLine("Mesh resolution: " + meshesController.meshResolution);
            Console.WriteLine("Mesh size: " + meshesController.meshSize);
            Console.WriteLine("Mesh Triangles: " + meshesController.meshResolution * meshesController.meshResolution * 2);

            Console.WriteLine("---Map info---");
            Console.WriteLine("Total number of meshes: " + meshesController.meshes.Count);
            Console.WriteLine("Render distance: " + meshesController.renderDistance);
            Console.WriteLine("Total number of meshes to delete: " + meshesController.meshesToDelete.Count);
            Console.WriteLine("Total number of meshes to add: " + meshesController.meshesToAdd.Count);
            Console.WriteLine("Total number of triangles: " + meshesController.meshResolution * meshesController.meshResolution * 2 * meshesController.meshes.Count);
        }

        private void consoleThreadFunction()
        {
            while (active)
            {
                WriteInformation();
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Thread.Sleep(100);
            }
        }
    }
}
