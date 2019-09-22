using PracaInzynierska;
using System;
using Xunit;

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
        public void MeshGenerationArgumentsCheck(int resolution, int size)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => mesh.generateMesh(resolution, size));
        }
        [Fact]
        public void MeshGenerationResolutionOneVertices()
        {
            Vertex[] actualVertices = new Vertex[4] { new Vertex(0f, 0f, 0f), new Vertex(2f, 0f, 0f), new Vertex(0f, 2f, 0f), new Vertex(2f, 2f, 0f) };
            mesh.generateMesh(1,2);
            Assert.Equal(actualVertices, mesh.getVertices());
        }

        [Fact]
        public void MeshGenerationResolutionTwoIndices()
        {
            int[] actualIndices = new int[24] { 1, 0, 4, 0, 3, 4, 2, 1, 5, 1, 4, 5, 4, 3, 7, 3, 6, 7, 5, 4, 8, 4, 7, 8 };
            mesh.generateMesh(2, 2);
            Assert.Equal(actualIndices, mesh.getIndices());
        }
    }
}
