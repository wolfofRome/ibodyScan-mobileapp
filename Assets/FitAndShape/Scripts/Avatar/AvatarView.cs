﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace FitAndShape
{
    public sealed class AvatarView : MonoBehaviour
    {
        Vector3[] _objPoints;

        public Transform[] GetBones()
        {
            int length = Enum.GetNames(typeof(AvatarBones)).Length;

            Transform[] bones = new Transform[length];

            for (int i = 0; i < length; i++)
            {
                var bone = GetFirstChildByName(((AvatarBones)i).GetName());

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

            Transform boneTransform = GetFirstChildByNameSub(transform, childName);

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