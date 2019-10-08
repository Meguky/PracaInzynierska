using PracaInzynierska;
using System;
using System.Linq;
using Xunit;
using OpenTK;

namespace PracaInzynierskaTests
{
    public class MeshesControllerTests
    {
        private PracaInzynierska.MeshesController meshesController;

        [Fact]
        public void MeshesGridGenerationSizeAndDistanceTwo()
        {
            Vector3[] actualData = new Vector3[4] {new Vector3(-2,0,-2), new Vector3(0, 0, -2), new Vector3(-2, 0, 0), new Vector3(0, 0, 0) };
            meshesController = new MeshesController(2,2,2, new Camera(new Vector3(0,0,0),0));
            Assert.Equal(actualData, meshesController.meshesOriginPointGrid);
        }

        [Fact]
        public void MeshesGridGenerationSizeAndDistanceThree()
        {
            Vector3[] actualData = new Vector3[9] { new Vector3(-4.5f, 0, -4.5f), new Vector3(-1.5f, 0, -4.5f), new Vector3(1.5f, 0, -4.5f), new Vector3(-4.5f, 0, -1.5f), new Vector3(-1.5f,0,-1.5f), new Vector3(1.5f,0,-1.5f), new Vector3(-4.5f,0,1.5f), new Vector3(-1.5f,0,1.5f), new Vector3(1.5f, 0, 1.5f) };
            meshesController = new MeshesController(2, 3, 3, new Camera(new Vector3(0, 0, 0), 0));
            Assert.Equal(actualData, meshesController.meshesOriginPointGrid);
        }
    }
}
