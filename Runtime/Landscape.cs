using UnityEngine;

namespace EliotByte.AsyncTerrain
{
    public class Landscape : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public bool Initialized { get; private set; } = false;
        public float[,] Heights { get; private set; }
        public Material Material { get; private set; }
        public Vector3 Size { get; private set; }
        public Mesh Mesh { get; private set; }

        public void Initialize(Material material)
        {
            if (Initialized)
                throw new System.InvalidOperationException("Terrain is already initialized.");

            Material = material;

            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();

            _meshRenderer.material = Material;

            Initialized = true;
        }

        public void SetHeights(MeshPlane meshPlane)
        {
            if (!Initialized)
                throw new System.InvalidOperationException(
                    "Terrain has not been initialized. Call Initialize() before setting heights.");

            if (Mesh != null )
                Destroy(Mesh);

            Mesh = meshPlane.CreateMesh();
            _meshFilter.mesh = Mesh;
        }

        private void OnDestroy()
        {
            if (Mesh == null)
                return;

            Destroy(Mesh);
            Mesh = null;
        }
    }
}
