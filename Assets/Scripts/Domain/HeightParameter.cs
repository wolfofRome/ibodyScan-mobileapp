using System.Linq;
using UnityEngine;

namespace Amatib.ObjViewer.Domain
{
    public sealed class HeightParameter
    {
        public readonly float Height;

        public HeightParameter(Mesh mesh)
        {
            var max = mesh.vertices.Select(n => n.y).Max();
            var min = mesh.vertices.Select(n => n.y).Min();
            Height = max - min;
        }
    }
}
