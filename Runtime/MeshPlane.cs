using UnityEngine;

namespace EliotByte.AsyncTerrain
{
    public class MeshPlane
    {
        private float[,] _heights;
        private readonly Vector3 _meshSize;
        private readonly Vector2Int _gridSize;
        private readonly Vector3[] _vertices;
        private readonly int[] _triangles;
        private readonly Vector2[] _uvs;

        public MeshPlane(float[,] heights, Vector3 meshSize)
        {
            if (meshSize.x <= 0 || meshSize.y <= 0 || meshSize.z <= 0)
                throw new System.ArgumentException("Size dimensions must be greater than zero.");

            _heights = heights;
            _meshSize = meshSize;
            _gridSize = new Vector2Int(heights.GetLength(0), heights.GetLength(1));
            _vertices = new Vector3[_gridSize.x * _gridSize.y];
            _triangles = new int[(_gridSize.x - 1) * (_gridSize.y - 1) * 6];
            _uvs = new Vector2[_gridSize.x * _gridSize.y];
        }

        public void Generate()
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    int index = x + y * _gridSize.x;
                    _vertices[index] = new Vector3(x * _meshSize.x / (_gridSize.x - 1), _heights[x, y] * _meshSize.y,
                        y * _meshSize.z / (_gridSize.y - 1));
                    _uvs[index] = new Vector2((float)x / (_gridSize.x - 1), (float)y / (_gridSize.y - 1));
                }
            }

            int triIndex = 0;
            for (int x = 0; x < _gridSize.x - 1; x++)
            {
                for (int y = 0; y < _gridSize.y - 1; y++)
                {
                    int current = x + y * _gridSize.x;
                    int next = current + _gridSize.x;

                    _triangles[triIndex] = current;
                    _triangles[triIndex + 1] = next;
                    _triangles[triIndex + 2] = current + 1;

                    _triangles[triIndex + 3] = current + 1;
                    _triangles[triIndex + 4] = next;
                    _triangles[triIndex + 5] = next + 1;

                    triIndex += 6;
                }
            }
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new() {vertices = _vertices, uv = _uvs, triangles = _triangles};
            mesh.RecalculateNormals();

            return mesh;
        }

        private void Update(float[,] heights)
        {
            _heights = heights;

            for (int x = 0; x < _gridSize.x; x++)
            for (int y = 0; y < _gridSize.y; y++)
                _vertices[x + y * _gridSize.x].y = _heights[x, y] * _meshSize.y;
        }
    }
}
