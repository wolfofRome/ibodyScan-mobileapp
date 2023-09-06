using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    // 3D人体のメッシュをプロットするためのViewクラス
    public sealed class ModelView : MonoBehaviour
    {
        [SerializeField] GameObject _monochromeModel;
        [SerializeField] GameObject _pointCloudModel;
        [SerializeField] MeshFilter _monochromeMeshFilter;
        [SerializeField] MeshFilter _pointCloudMeshFilter;
        [SerializeField] private Skeleton _skeleton;
        
        [Header("UI")] [SerializeField] Slider _skeletonSlider;

        public Vector3 Position => transform.position;
        public bool IsLoadMonochrome { get; private set; } = false;
        public bool IsLoadPointCloud { get; private set; } = false;
        public Vector3 Center => Vector3.zero;
        public bool MonochromeModelVisible { get { return _monochromeModel.activeSelf; } set { _monochromeModel.SetActive(value); } }
        public bool PointCloudModelVisible { get { return _pointCloudModel.activeSelf; } set { _pointCloudModel.SetActive(value); } }

        Vector3 _defaultPosition;

        private void Start()
        {
            _skeletonSlider.onValueChanged.AddListener((newValue => { OnUpdateAlphaSlider(newValue);}));
            _skeletonSlider.value = 0;
        }

        public void SetPosition(Vector3 value)
        {
            _defaultPosition = value;
            transform.position = value;
        }

        /// <summary>
        /// 非相同モデルのMeshをSet
        /// </summary>
        /// <param name="mesh"></param>
        public void SetMonochromeMesh(Mesh mesh)
        {
            IsLoadMonochrome = true;
            _monochromeMeshFilter.mesh = mesh;
        }

        /// <summary>
        /// 点群モデルのMeshをSet
        /// </summary>
        /// <param name="mesh"></param>
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
        
        /// <summary>
        ///
        /// 透明度スライダーの値の変化に対するリスナー
        /// </summary>
        /// <param name="value"></param>
        public void OnUpdateAlphaSlider(float alpha)
        {
            Color newColor = _monochromeModel.GetComponent<Renderer>().material.color;
            newColor.a = 1 - alpha;
            _monochromeModel.GetComponent<Renderer>().material.color = newColor;
            _pointCloudModel.GetComponent<Renderer>().material.color = newColor; // TODO: そもそもこれ効いてないよね
            _skeleton.SetAlpha(alpha);
        }

        public void UpdateChestMesh(int index, Vector3 value)
        {
            Debug.Log($"_monochromeMeshFilter.mesh.vertices[index]: {_monochromeMeshFilter.mesh.vertices[index]}, newValue: {value}");
            // _monochromeMeshFilter.mesh.vertices[index] = value;
            List<Vector3> newValues = new List<Vector3>();
            _monochromeMeshFilter.mesh.GetVertices(newValues);
            newValues[index] = value;
            _monochromeMeshFilter.mesh.SetVertices(newValues);
        }

        /// <summary>
        /// SkeletonのTransformを渡す（Bonesの整形ロジック時にScaleを書き換えるため）
        /// </summary>
        /// <returns></returns>
        public Transform GetSkeletonTransform()
        {
            return _skeleton.transform;
        }
        
        // 以下、AvatarViewからの移植
        
        Vector3[] _objPoints;

        public Transform[] GetBones()
        {
            int length = Enum.GetNames(typeof(AvatarBones)).Length;

            Transform[] bones = new Transform[length];

            for (int i = 0; i < length; i++)
            {
                string objectName = AvatarBonesExtension.GetName((AvatarBones)i);
                var bone = GetFirstChildByName(objectName);

                if (bone != null)
                {
                    bones[i] = bone;
                }
            }

            return bones;
        }

        Transform GetFirstChildByName(string childName)
        {
            if (string.IsNullOrEmpty(childName))
            {
                return null;
            }

            Transform boneTransform = GetFirstChildByNameSub(_skeleton.transform, childName);

            if (boneTransform == null)
            {
                throw new UnityException("Root bone is not found!");
            }

            return boneTransform;
        }

        Transform GetFirstChildByNameSub(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }

                Transform target = GetFirstChildByNameSub(child, childName);

                if (target != null) return target;
            }

            return null;
        }

        public void DisplayBoneLines(Vector3[] objPoints, Dictionary<AvatarBones, int?[]> avatarBonesObjRows)
        {
            _objPoints = objPoints;

            GameObject gameObject = new GameObject("DisplayBone");

            for (int i = 0; i < _objPoints.Length; i++)
            {
                AvatarBones bone = (AvatarBones)i;

                if (avatarBonesObjRows[bone] == null) continue;

                string name = Enum.GetNames(typeof(AvatarBones))[i];

                GameObject sphere = DrawSphere(bone, _objPoints[i], Color.green, name, gameObject.transform);

                DrawLines(bone, sphere);
            }
        }

        GameObject DrawSphere(AvatarBones bone, Vector3 position, Color color, string gameObjectName, Transform parent)
        {
            return AddPrimitiveGameObject(position * AppConst.ObjLoadScale, new Vector3(0.02f, 0.02f, 0.02f), color, PrimitiveType.Sphere, gameObjectName, parent);
        }

        GameObject AddPrimitiveGameObject(Vector3 position, Vector3 scale, Color color, PrimitiveType type, string name, Transform parent)
        {
            GameObject gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.parent = parent;
            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;
            gameObject.SetActive(true);

            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material.color = color;

            return gameObject;
        }

        void DrawLines(AvatarBones bone, GameObject gameObject)
        {
            switch (bone)
            {
                case AvatarBones.Hips:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Hips], _objPoints[(int)AvatarBones.SpineWaist] });
                    break;
                case AvatarBones.LeftUpLeg:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Hips], _objPoints[(int)AvatarBones.LeftUpLeg], _objPoints[(int)AvatarBones.LeftLeg] });
                    break;
                case AvatarBones.LeftLeg:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.LeftLeg], _objPoints[(int)AvatarBones.LeftFoot] });
                    break;
                case AvatarBones.LeftFoot:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.LeftFoot], _objPoints[(int)AvatarBones.LeftToe] });
                    break;
                case AvatarBones.RightUpLeg:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Hips], _objPoints[(int)AvatarBones.RightUpLeg], _objPoints[(int)AvatarBones.RightLeg] });
                    break;
                case AvatarBones.RightLeg:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.RightLeg], _objPoints[(int)AvatarBones.RightFoot] });
                    break;
                case AvatarBones.RightFoot:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.RightFoot], _objPoints[(int)AvatarBones.RightToe] });
                    break;
                case AvatarBones.SpineWaist:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.SpineWaist], _objPoints[(int)AvatarBones.Spine1] });
                    break;
                case AvatarBones.Spine1:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Spine1], _objPoints[(int)AvatarBones.Spine2] });
                    break;
                case AvatarBones.Spine2:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Spine2], _objPoints[(int)AvatarBones.Neck1] });
                    break;
                case AvatarBones.Neck1:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Neck1], _objPoints[(int)AvatarBones.Neck2] });
                    break;
                case AvatarBones.Neck2:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Neck2], _objPoints[(int)AvatarBones.Neck3] });
                    break;
                case AvatarBones.Neck3:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Neck3], _objPoints[(int)AvatarBones.Head] });
                    break;
                case AvatarBones.LeftShoulder:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Spine2], _objPoints[(int)AvatarBones.LeftShoulder], _objPoints[(int)AvatarBones.LeftArm] });
                    break;
                case AvatarBones.LeftArm:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.LeftArm], _objPoints[(int)AvatarBones.LeftForeArm] });
                    break;
                case AvatarBones.LeftForeArm:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.LeftForeArm], _objPoints[(int)AvatarBones.LeftHand] });
                    break;
                case AvatarBones.LeftHand:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.LeftHand], _objPoints[(int)AvatarBones.LeftHand2] });
                    break;
                case AvatarBones.RightShoulder:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.Spine2], _objPoints[(int)AvatarBones.RightShoulder], _objPoints[(int)AvatarBones.RightArm] });
                    break;
                case AvatarBones.RightArm:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.RightArm], _objPoints[(int)AvatarBones.RightForeArm] });
                    break;
                case AvatarBones.RightForeArm:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.RightForeArm], _objPoints[(int)AvatarBones.RightHand] });
                    break;
                case AvatarBones.RightHand:
                    DrawLines(gameObject, new[] { _objPoints[(int)AvatarBones.RightHand], _objPoints[(int)AvatarBones.RightHand2] });
                    break;
            }
        }

        void DrawLines(GameObject gameObject, Vector3[] points)
        {
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

            if (lineRenderer == null) return;

            lineRenderer.material = new Material(Shader.Find("Diffuse")) { color = Color.green };
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.positionCount = points.Length;

            int count = points.Length;

            for (int i = 0; i < count; i++)
            {
                lineRenderer.SetPosition(i, points[i] * AppConst.ObjLoadScale);
            }
        }
    }
}