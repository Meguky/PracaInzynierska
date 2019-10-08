using PracaInzynierska;
using System;
using System.Linq;
using Xunit;
using OpenTK;

namespace PracaInzynierskaTests
{
    public class MeshTests
    {
        private PracaInzynierska.Mesh mesh;


        [Theory]
        [InlineData(0,0)]
        [InlineData(0, 20)]
        [InlineData(20, 0)]
        public void MeshGenerationArgumentsCheck(uint resolution, uint size)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PracaInzynierska.Mesh(resolution, size, new Vector3(0, 0, 0), new Vector3(0, 0, 0)));
        }

        [Theory]
        [InlineData(2,9)]
        [InlineData(3, 16)]
        [InlineData(4, 25)]
        [InlineData(10, 121)]
        public void MeshGenerationLength(uint resolution, uint actualLength)
        {
            mesh = new PracaInzynierska.Mesh(resolution, 2, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            Assert.Equal(actualLength, (uint)mesh.getVertices().Length);

        }

        [Fact]
        public void MeshGenerationResolutionOneVertices()
        {
            Vector3[] actualVertices = new Vector3[4] { new Vector3(0f, 0f, 0f), new Vector3(2f, 0f, 0f), new Vector3(0f, 0f, 2f), new Vector3(2f, 0f, 2f) };
            mesh = new PracaInzynierska.Mesh(1, 2, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            mesh.generateMesh();
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

            mesh = new PracaInzynierska.Mesh(2, 2, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            mesh.generateMesh();
            Assert.Equal(actualIndices, mesh.getIndices());
        }
    }
}
