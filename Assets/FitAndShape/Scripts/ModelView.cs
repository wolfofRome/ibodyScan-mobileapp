using UnityEngine;

namespace FitAndShape
{
    public sealed class ModelView : MonoBehaviour
    {
        [SerializeField] GameObject _monochromeModel;
        [SerializeField] GameObject _pointCloudModel;
        [SerializeField] MeshFilter _monochromeMeshFilter;
        [SerializeField] MeshFilter _pointCloudMeshFilter;

        public Vector3 Position => transform.position;
        public bool IsLoadMonochrome { get; private set; } = false;
        public bool IsLoadPointCloud { get; private set; } = false;
        public Vector3 Center => Vector3.zero;
        public bool MonochromeModelVisible { get { return _monochromeModel.activeSelf; } set { _monochromeModel.SetActive(value); } }
        public bool PointCloudModelVisible { get { return _pointCloudModel.activeSelf; } set { _pointCloudModel.SetActive(value); } }

        Vector3 _defaultPosition;

        public void SetPosition(Vector3 value)
        {
            _defaultPosition = value;
            transform.position = value;
        }

        public void SetMonochromeMesh(Mesh mesh)
        {
            IsLoadMonochrome = true;
            _monochromeMeshFilter.mesh = mesh;
        }

        public void SetPointCloudMesh(Mesh mesh)
        {
            IsLoadPointCloud = true;
            _pointCloudMeshFilter.mesh = mesh;
        }

        public void OnReset()
        {
            transform.position = _defaultPosition;
            transform.rotation = Quaternion.identity;
        }

        public void Clear()
        {
            IsLoadMonochrome = false;
            _monochromeMeshFilter.mesh = null;

            IsLoadPointCloud = false;
            _pointCloudMeshFilter.mesh = null;
        }
    }
}