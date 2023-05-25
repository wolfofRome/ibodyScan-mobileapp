using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Amatib.ObjViewer.Presentation.Loaders
{
    /// <summary>
    /// OBJファイルローダー
    /// 
    /// OBJImporterをWebGLで利用すると非常にメモリを食うので、独自のOBJファイルローダーを作成
    /// マテリアル、テクスチャ、法線情報を無視
    /// 
    /// Runtime OBJ Importer
    /// https://assetstore.unity.com/packages/tools/modeling/runtime-obj-importer-49547?locale=ja-JP
    /// Wavefront.objファイル（Wikipedia）
    /// http://ja.wikipedia.org/w/index.php?curid=3417965
    /// 読み込みコード作成サンプル（Qiita）
    /// https://qiita.com/oho-sugu/items/e7441b1f1aebee143628
    /// Objファイル基礎
    /// https://yttm-work.jp/model_render/model_render_0001.html
    /// </summary>
    public static class ObjLoader
    {
        public static string[] LoadAsStream(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd().Split("\n");
        }

        public static Mesh LoadAsMesh(string[] lines, Vector3 scale)
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();

            foreach (string line in lines)
            {
                var tokens = line.Split(' ');
                switch (tokens[0])
                {
                    case "v":
                        // 頂点（vertex） syntax: v x y z
                        vertices.Add(new Vector3(float.Parse(tokens[1]) * scale.x, float.Parse(tokens[2]) * scale.y, float.Parse(tokens[3]) * scale.z));
                        break;
                    case "f":
                        // 頂点インデックス（index） syntax: f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3
                        indices.Add(int.Parse(tokens[1].Split('/', 2)[0]) - 1);
                        indices.Add(int.Parse(tokens[2].Split('/', 2)[0]) - 1);
                        indices.Add(int.Parse(tokens[3].Split('/', 2)[0]) - 1);
                        break;
                }
            }

            var mesh = new Mesh
            {
                name = "default"
            };
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Reverse(mesh);

            return mesh;
        }

        static void Reverse(Mesh mesh)
        {
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = temp;
            }
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }
    }
}