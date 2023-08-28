using UnityEngine;

namespace FitAndShape
{
    public struct ArrowRenderInfo
    {
        public AvatarBones[] StartPoints { get; }
        public AvatarBones[] EndPoints { get; }
        public ArrowType ArrowType { get; }

        public ArrowRenderInfo(AvatarBones[] start, AvatarBones[] end, ArrowType type)
        {
            StartPoints = start;
            EndPoints = end;
            ArrowType = type;
        }

        /// <summary>
        /// 矢印の開始点取得.
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public Vector3? GetStartPoint(IAvatarModel avatar)
        {
            return GetMidPoint(StartPoints, avatar);
        }

        /// <summary>
        /// 矢印の終了点の取得.
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public Vector3? GetEndPoint(IAvatarModel avatar)
        {
            return GetMidPoint(EndPoints, avatar);
        }

        /// <summary>
        /// 開始点と終了点の中点のZ座標取得.
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public float GetZValue(IAvatarModel avatar)
        {
            float MaxZ = float.MinValue;
            float MinZ = float.MaxValue;
            if (StartPoints != null)
            {
                foreach (AvatarBones b in StartPoints)
                {
                    var p = avatar.GetPoint(b);
                    if (p != null)
                    {
                        MaxZ = Mathf.Max(MaxZ, ((Vector3)p).z);
                        MinZ = Mathf.Min(MinZ, ((Vector3)p).z);
                    }
                }
            }
            if (EndPoints != null)
            {
                foreach (AvatarBones b in EndPoints)
                {
                    var p = avatar.GetPoint(b);
                    if (p != null)
                    {
                        MaxZ = Mathf.Max(MaxZ, ((Vector3)p).z);
                        MinZ = Mathf.Min(MinZ, ((Vector3)p).z);
                    }
                }
            }
            return (MinZ + MaxZ) / 2;
        }

        /// <summary>
        /// 中点の取得.
        /// </summary>
        /// <param name="bones"></param>
        /// <param name="avatar"></param>
        /// <returns></returns>
        private Vector3? GetMidPoint(AvatarBones[] bones, IAvatarModel avatar)
        {
            if (bones == null) return null;

            Vector3 point = Vector3.zero;

            foreach (AvatarBones b in bones)
            {
                point += avatar.GetPoint(b) ?? Vector3.zero;
            }

            return point / bones.Length;
        }
    }
}
