using UnityEngine;

namespace Amatib.ObjViewer.Presentation
{
    /// <summary>
    /// PointCloud最適化
    ///
    /// 参考
    /// https://qiita.com/Y0241-N/items/8a2cb1cc6600d7936dc8
    /// </summary>
    public class PCoptimisation : MonoBehaviour
    {
        [Range(0f, 100f)]
        public float PointSize;
        void Start()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/PointCloud_GL");
#else
            gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Point Cloud/Point");
#endif
            GetComponent<Renderer>().material.EnableKeyword("_PointSize");
            if (gameObject.GetComponent<MeshRenderer>().material.shader.name == "Custom/PointCloud_GL")
            {
                GetComponent<Renderer>().material.SetFloat("_PointSize", PointSize);
            }

            if (gameObject.GetComponent<MeshRenderer>().material.shader.name == "Point Cloud/Point")
            {
                GetComponent<Renderer>().material.SetFloat("_PointSize", PointSize);
            }
        }
    }
}