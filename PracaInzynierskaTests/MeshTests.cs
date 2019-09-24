using PracaInzynierska;
using System;
using System.Linq;
using Xunit;
using OpenTK;

namespace PracaInzynierskaTests
{
    public class MeshTests
    {
        private readonly PracaInzynierska.Mesh mesh;

        public MeshTests()
        {
            mesh = new PracaInzynierska.Mesh();
        }

        [Theory]
        [InlineData(0,0)]
        [InlineData(0, 20)]
        [InlineData(20, 0)]
        public void MeshGenerationArgumentsCheck(uint resolution, int size)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => mesh.generateMesh(resolution, size));
        }

        [Theory]
        [InlineData(2,9)]
        [InlineData(3, 16)]
        [InlineData(4, 25)]
        [InlineData(10, 121)]
        public void MeshGenerationLength(uint resolution, int actualLength)
        {
            mesh.generateMesh(resolution, 2);
            Assert.Equal(actualLength, mesh.getVertices().Length);

        }

        [Fact]
        public void MeshGenerationResolutionOneVertices()
        {
            Vector3[] actualVertices = new Vector3[4] { new Vector3(0f, 0f, 0f), new Vector3(2f, 0f, 0f), new Vector3(0f, 0f, 2f), new Vector3(2f, 0f, 2f) };
            mesh.generateMesh(1,2);
            Assert.Equal(actualVertices, mesh.getVertices());

        }

        [Fact]
        public void MeshGenerationResolutionTwoIndices()
        {
            //Tested working data
            uint[] actualIndices = new uint[24] {
                4, 3, 1,
                3, 0, 1,
                5, 4, 2,
                4, 1, 2,
                7, 6, 4,
                6, 3, 4,
                8, 7, 5,
                7, 4, 5
            };

            mesh.generateMesh(2, 2);
            Assert.Equal(actualIndices, mesh.getIndices());
        }
    }
}
