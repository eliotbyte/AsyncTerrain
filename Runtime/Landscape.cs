using UnityEngine;

namespace EliotByte.AsyncTerrain
{
    public class Landscape : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        public bool Initialized { get; private set; } = false;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float[,] Heights { get; private set; }
        public Material Material { get; private set; }
        public Vector3 Size { get; private set; }
        public Mesh Mesh { get; private set; }
        public Vector3[] Vertices { get; private set; }

        public void Initialize(Material material)
        {
            if (Initialized)
                throw new System.InvalidOperationException("Terrain is already initialized.");

            Material = material;

            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshRenderer.material = Material;

            Initialized = true;
        }

        public void SetHeights(float[,] heights, Vector3 size)
        {
            if (!Initialized)
                throw new System.InvalidOperationException(
                    "Terrain has not been initialized. Call Initialize() before setting heights.");

            if (size.x <= 0 || size.y <= 0 || size.z <= 0)
                throw new System.ArgumentException("Size dimensions must be greater than zero.");

            Heights = heights;
            Size = size;
            int newWidth = Heights.GetLength(0);
            int newHeight = Heights.GetLength(1);

            if (newWidth != Width || newHeight != Height || Mesh == null)
            {
                Width = newWidth;
                Height = newHeight;
                Destroy(Mesh);
                Mesh = null;
                GenerateMesh();
            }
            else
            {
                UpdateMesh();
            }
        }

        private void GenerateMesh()
        {
            Mesh = new Mesh();
            Vertices = new Vector3[Width * Height];
            Vector2[] uv = new Vector2[Width * Height];
            int[] triangles = new int[(Width - 1) * (Height - 1) * 6];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = x + y * Width;
                    Vertices[index] = new Vector3(x * Size.x / (Width - 1), Heights[x, y] * Size.y,
                        y * Size.z / (Height - 1));
                    uv[index] = new Vector2((float)x / (Width - 1), (float)y / (Height - 1));
                }
            }

            int triIndex = 0;
            for (int x = 0; x < Width - 1; x++)
            {
                for (int y = 0; y < Height - 1; y++)
                {
                    int current = x + y * Width;
                    int next = current + Width;

                    triangles[triIndex] = current;
                    triangles[triIndex + 1] = next;
                    triangles[triIndex + 2] = current + 1;

                    triangles[triIndex + 3] = current + 1;
                    triangles[triIndex + 4] = next;
                    triangles[triIndex + 5] = next + 1;

                    triIndex += 6;
                }
            }

            Mesh.vertices = Vertices;
            Mesh.uv = uv;
            Mesh.triangles = triangles;
            Mesh.RecalculateNormals();

            meshFilter.mesh = Mesh;
        }

        private void UpdateMesh()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Vertices[x + y * Width].y = Heights[x, y] * Size.y;
                }
            }

            Mesh.vertices = Vertices;
            Mesh.RecalculateNormals();
            Mesh.RecalculateBounds();
        }

        private void OnDestroy()
        {
            if (Mesh != null)
            {
                Destroy(Mesh);
                Mesh = null;
            }
        }
    }
}
