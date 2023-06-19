using Codice.CM.Client.Differences.Merge;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FitAndShape
{
    public interface IAvatarModel
    {
        float GetScale();
        void UpdateBones();
        Vector3? GetPoint(AvatarBones bone);
        Quaternion? GetRotation(AvatarBones bone);
        Dictionary<RowData, Vector3> GetSrcPoint(AvatarBones bone);
        Dictionary<AvatarBones, int?[]> GetAvatarBonesObjRows();
        Vector3[] ObjPoints { get; }
        Vector3 ParseVertexData(int rowNumber);
    }

    public sealed class AvatarModel : IAvatarModel
    {
        readonly Vector3 _objOffset;
        readonly Transform[] _bones;
        readonly bool _isMale;
        readonly float _bodyScale;
        readonly Dictionary<AvatarBones, int?[]> _maleAvatarBonesObjRows;
        readonly Dictionary<AvatarBones, int?[]> _femaleAvatarBonesObjRows;
        readonly IReadOnlyList<string> _objLines;
        readonly int _rowsOffset;
        readonly Dictionary<RowData, Vector3>[] _objSrcPoints;

        public Vector3[] ObjPoints { get; }

        public AvatarModel(IReadOnlyList<string> objLines, Transform[] bones, Vector3 objOffset, bool isMale, float bodyScale)
        {
            int length = Enum.GetNames(typeof(AvatarBones)).Length;

            _objLines = objLines;

            _bodyScale = bodyScale;

            _maleAvatarBonesObjRows = GetMaleAvatarBonesObjRows();
            _femaleAvatarBonesObjRows = GetFemaleAvatarBonesObjRows();

            _rowsOffset = GetRowsOffset();

            _objSrcPoints = LoadSrcPoints(length);

            ObjPoints = new Vector3[length];

            _bones = bones;

            _isMale = isMale;

            _objOffset = objOffset;

            SetPoints(length);

            SetBoneRotation(length);

            UpdateBones();
        }

        public float GetScale()
        {
            float scale = ((ObjPoints[(int)AvatarBones.Neck1].y * _bodyScale) - _objOffset.y) / _bones[(int)AvatarBones.Neck1].position.y;
            return scale;
        }

        int GetRowsOffset()
        {
            Regex regex = new Regex(@"v -?\d+.\d+ -?\d+.\d+ -?\d+.\d+");

            int offset = 0;

            foreach (string line in _objLines)
            {
                if (regex.IsMatch(line)) break;
                offset++;
            }

            return offset - 1;
        }

        void SetBoneRotation(int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (_bones[i] != null)
                {
                    _bones[i].localRotation = Quaternion.identity;
                }
            }
        }

        void SetPoints(int length)
        {
            for (int i = 0; i < length; i++)
            {
                AvatarBones bone = (AvatarBones)i;

                int?[] row = GetAvatarBonesObjRows()[bone];

                switch (bone)
                {
                    case AvatarBones.Head:
                    case AvatarBones.LeftEarlobe:
                    case AvatarBones.RightEarlobe:
                    case AvatarBones.LeftAcromion:
                    case AvatarBones.RightAcromion:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Up]);
                            ObjPoints[i] = v;
                        }
                        break;
                    case AvatarBones.Neck1:
                        ObjPoints[i] = CalcSpineLineVertex(row, _objLines, 0.5f, 0.5f);
                        break;
                    case AvatarBones.LeftShoulder:
                    case AvatarBones.RightShoulder:
                        {
                            float sumZ = 0.0f;
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward]);
                            float forwardX = v.x;
                            float forwardY = v.y;
                            sumZ += v.z;
                            v = ParseVertexData((int)row[(int)RowData.Up]);
                            sumZ += v.z;
                            ObjPoints[i] = new Vector3(forwardX, forwardY, sumZ / 2.0f);
                        }
                        break;
                    case AvatarBones.LeftArm:
                    case AvatarBones.LeftUpScale1:
                    case AvatarBones.RightArm:
                    case AvatarBones.RightUpScale1:
                        {
                            float sumX = 0.0f;
                            float sumY = 0.0f;
                            float sumZ = 0.0f;
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward]);
                            sumX += v.x;
                            sumY += v.y;
                            sumZ += v.z;
                            v = ParseVertexData((int)row[(int)RowData.Back]);
                            sumX += v.x;
                            sumY += v.y;
                            sumZ += v.z;
                            ObjPoints[i] = new Vector3(sumX / 2.0f, sumY / 2.0f, sumZ / 2.0f);
                        }
                        break;
                    case AvatarBones.LeftForeArm:
                    case AvatarBones.RightForeArm:
                        ObjPoints[i] = CalcLimbVertex(row, 0.6f);
                        break;
                    case AvatarBones.LeftHand2:
                    case AvatarBones.RightHand2:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward]);
                            ObjPoints[i] = v;
                        }
                        break;
                    case AvatarBones.Spine2:
                        ObjPoints[i] = CalcSpineLineVertex(row, _objLines, 0.5f, 0.6f);
                        break;
                    case AvatarBones.Spine1:
                        ObjPoints[i] = CalcSpineLineVertex(row, _objLines, 0.5f, 0.7f);
                        break;
                    case AvatarBones.SpineWaist:
                        ObjPoints[i] = CalcSpineLineVertex(row, _objLines, 0.5f, 0.6f);
                        break;
                    case AvatarBones.LeftUpLeg:
                    case AvatarBones.LeftDownScale1:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward]);
                            float forwardX = v.x;
                            float forwardY = v.y;
                            v = ParseVertexData((int)row[(int)RowData.Left]);
                            float leftZ = v.z;
                            ObjPoints[i] = new Vector3(forwardX, forwardY, leftZ);
                        }
                        break;
                    case AvatarBones.RightUpLeg:
                    case AvatarBones.RightDownScale1:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward]);
                            float forwardX = v.x;
                            float forwardY = v.y;
                            v = ParseVertexData((int)row[(int)RowData.Right]);
                            float rightZ = v.z;
                            ObjPoints[i] = new Vector3(forwardX, forwardY, rightZ);
                        }
                        break;
                    case AvatarBones.LeftLeg:
                    case AvatarBones.RightLeg:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward]);
                            float forwardX = v.x;
                            float forwardY = v.y;
                            float forwardZ = v.z;
                            v = ParseVertexData((int)row[(int)RowData.Back]);
                            float backZ = v.z;
                            ObjPoints[i] = new Vector3(forwardX, forwardY, Mathf.Lerp(forwardZ, backZ, 0.4f));
                        }
                        break;
                    case AvatarBones.LeftFoot:
                    case AvatarBones.RightFoot:
                        ObjPoints[i] = CalcLimbVertex(row, 0.45f);
                        break;
                    case AvatarBones.LeftToe:
                    case AvatarBones.RightToe:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Up]);
                            float upX = v.x;
                            float upY = v.y;
                            v = ParseVertexData((int)row[(int)RowData.Forward]);
                            float forwardZ = v.z;
                            ObjPoints[i] = new Vector3(upX, upY, forwardZ);
                        }
                        break;
                    case AvatarBones.LeftDownScale2:
                    case AvatarBones.RightDownScale2:
                    case AvatarBones.Neck2:
                    case AvatarBones.Neck3:
                    case AvatarBones.LeftHand:
                    case AvatarBones.RightHand:
                    case AvatarBones.Hips:
                        {
                            int counter = 0;
                            float sumX = 0.0f;
                            float sumY = 0.0f;
                            float sumZ = 0.0f;

                            if (row[(int)RowData.Left] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Left]);
                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;
                                counter++;
                            }

                            if (row[(int)RowData.Right] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Right]);
                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;
                                counter++;
                            }

                            if (row[(int)RowData.Forward] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Forward]);
                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;
                                counter++;
                            }

                            if (row[(int)RowData.Back] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Back]);
                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;
                                counter++;
                            }

                            if (row[(int)RowData.Up] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Up]);
                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;
                                counter++;
                            }

                            if (row[(int)RowData.Down] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Down]);
                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;
                                counter++;
                            }

                            if (counter > 0)
                            {
                                ObjPoints[i] = new Vector3(sumX / counter, sumY / counter, sumZ / counter);
                            }
                        }
                        break;
                    case AvatarBones.Root:
                        break;
                }
            }
        }

        public void UpdateBones()
        {
            {
                Vector3 ribRight = ParseVertexData(436);
                Vector3 ribLeft = ParseVertexData(1398);
                Vector3 ribForward = ParseVertexData(921);
                Vector3 ribBack = ParseVertexData(920);

                float ribXDistance = (ribRight - ribLeft).magnitude;
                float ribXScale = ribXDistance / (_isMale ? 280.0f : 220.0f);

                float ribZDistance = (ribForward - ribBack).magnitude;
                float ribZScale = ribZDistance / (_isMale ? 210.0f : 180.0f);

                int index = (int)AvatarBones.SpineWaist;
                _bones[index].localScale = new Vector3(_bones[index].localScale.x * ribXScale, _bones[index].localScale.y, _bones[index].localScale.z * ribZScale);

                index = (int)AvatarBones.Neck2;
                _bones[index].localScale = new Vector3(_bones[index].localScale.x / ribXScale, _bones[index].localScale.y, _bones[index].localScale.z / ribZScale);
            }

            float headDistanceBefore = (_bones[(int)AvatarBones.Head].position - _bones[(int)AvatarBones.Neck2].position).magnitude;

            Quaternion inverseQ = Quaternion.identity;
            Quaternion hipsInverseQ = Quaternion.identity;

            {
                int index = (int)AvatarBones.Hips;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 leftUpLegPos = ObjPoints[(int)AvatarBones.LeftUpLeg] * _bodyScale;
                Vector3 rightUpLegPos = ObjPoints[(int)AvatarBones.RightUpLeg] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.Hips, AvatarBones.SpineWaist);
                to = new Vector3(0.0f, to.y, to.z);

                Quaternion qX = Quaternion.FromToRotation(Vector3.up, to);

                Quaternion qY;
                {
                    float radian = Mathf.Atan2(rightUpLegPos.z - leftUpLegPos.z, rightUpLegPos.x - leftUpLegPos.x);
                    qY = Quaternion.AngleAxis(radian * Mathf.Rad2Deg, Vector3.up);
                }

                Quaternion qZ;
                {
                    float radian = Mathf.Atan2(rightUpLegPos.y - leftUpLegPos.y, rightUpLegPos.x - leftUpLegPos.x);
                    qZ = Quaternion.AngleAxis(radian * Mathf.Rad2Deg, Vector3.forward);
                }

                _bones[index].localRotation *= (qX * qY * qZ);

                inverseQ = Quaternion.Inverse(_bones[index].rotation);
                hipsInverseQ = inverseQ;
            }
            {
                int index = (int)AvatarBones.SpineWaist;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.SpineWaist, AvatarBones.Spine1);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(19.4f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.Spine1;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.Spine1, AvatarBones.Spine2);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q);
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.Spine2;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.Spine2, AvatarBones.Neck1);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-15.7f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.Neck1;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.Neck1, AvatarBones.Neck2);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-25.6f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.Neck2;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.Neck2, AvatarBones.Neck3);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-1.3f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }

            {
                int index = (int)AvatarBones.Neck3;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.Neck2, AvatarBones.Head);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);

                Quaternion qY;
                {
                    Vector3 leftEarlobePos = ObjPoints[(int)AvatarBones.LeftEarlobe] * _bodyScale;
                    Vector3 rightEarlobePos = ObjPoints[(int)AvatarBones.RightEarlobe] * _bodyScale;

                    float radian = -Mathf.Atan2(rightEarlobePos.z - leftEarlobePos.z, rightEarlobePos.x - leftEarlobePos.x);
                    qY = Quaternion.AngleAxis(radian * Mathf.Rad2Deg, Vector3.up);
                }

                _bones[index].localRotation *= (inverseQ * q * qY);
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }

            {
                int index = (int)AvatarBones.Head;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.LeftShoulder;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftShoulder, AvatarBones.LeftArm);

                Quaternion q = Quaternion.FromToRotation(Vector3.left, to);
                _bones[index].localRotation *= (q * Quaternion.Euler(0.0f, 17.5f, -3.3f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftArm;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftArm, AvatarBones.LeftForeArm);

                Quaternion q = Quaternion.FromToRotation(Vector3.left, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, 4.3f, 4.7f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftUpScale1;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.LeftForeArm;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftForeArm, AvatarBones.LeftHand);

                Quaternion q = Quaternion.FromToRotation(Vector3.left, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, -6.3f, 1.6f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftHand;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftHand, AvatarBones.LeftHand2);

                Quaternion q = Quaternion.FromToRotation(Vector3.left, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, -1.6f, 2.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftHand2;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.RightShoulder;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightShoulder, AvatarBones.RightArm);

                Quaternion q = Quaternion.FromToRotation(Vector3.right, to);
                _bones[index].localRotation *= (q * Quaternion.Euler(0.0f, -17.5f, 3.3f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightArm;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightArm, AvatarBones.RightForeArm);

                Quaternion q = Quaternion.FromToRotation(Vector3.right, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, -4.3f, -4.7f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightUpScale1;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.RightForeArm;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightForeArm, AvatarBones.RightHand);

                Quaternion q = Quaternion.FromToRotation(Vector3.right, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, 6.3f, -1.6f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightHand;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightHand, AvatarBones.RightHand2);

                Quaternion q = Quaternion.FromToRotation(Vector3.right, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, 1.6f, -2.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightHand2;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.LeftUpLeg;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftUpLeg, AvatarBones.LeftLeg);

                Quaternion q = Quaternion.FromToRotation(Vector3.down, to);
                _bones[index].localRotation *= (q * Quaternion.Euler(-5.4f, 0.0f, -1.0f) * hipsInverseQ);
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftDownScale1;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.LeftDownScale2;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.LeftLeg;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftLeg, AvatarBones.LeftFoot);

                Quaternion q = Quaternion.FromToRotation(Vector3.down, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-7.1f, 0.0f, -0.4f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftFoot;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftFoot, AvatarBones.LeftToe);

                Quaternion q = Quaternion.FromToRotation(Vector3.forward, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-19.5f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftToe;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.RightUpLeg;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightUpLeg, AvatarBones.RightLeg);

                Quaternion q = Quaternion.FromToRotation(Vector3.down, to);
                _bones[index].localRotation *= (q * Quaternion.Euler(-5.4f, 0.0f, 1.0f) * hipsInverseQ);
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightDownScale1;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.RightDownScale2;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }
            {
                int index = (int)AvatarBones.RightLeg;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightLeg, AvatarBones.RightFoot);

                Quaternion q = Quaternion.FromToRotation(Vector3.down, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-7.1f, 0.0f, 0.4f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightFoot;
                _bones[index].position = ObjPoints[index] * _bodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightFoot, AvatarBones.RightToe);

                Quaternion q = Quaternion.FromToRotation(Vector3.forward, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-19.5f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightToe;
                _bones[index].position = ObjPoints[index] * _bodyScale;
            }

            {
                float headDistanceAfter = (_bones[(int)AvatarBones.Head].position - _bones[(int)AvatarBones.Neck2].position).magnitude;

                if ((headDistanceAfter * 0.9f) < headDistanceBefore)
                {
                    float coefficient = (headDistanceAfter / headDistanceBefore) * 0.9f;
                    int index = (int)AvatarBones.Head;
                    _bones[index].localScale = new Vector3(_bones[index].localScale.x * coefficient, _bones[index].localScale.y * coefficient, _bones[index].localScale.z * coefficient);
                }
            }
        }

        public Vector3? GetPoint(AvatarBones bone)
        {
            Vector3? bonePoint = _bones[(int)bone] != null ? (Vector3?)_bones[(int)bone].position / _bodyScale : null;
            Vector3? objPoint = ObjPoints[(int)bone] != Vector3.zero ? (Vector3?)ObjPoints[(int)bone] : null;
            return bonePoint ?? objPoint;
        }

        public Quaternion? GetRotation(AvatarBones bone)
        {
            return _bones[(int)bone] != null ? (Quaternion?)_bones[(int)bone].rotation : null;
        }

        public Dictionary<RowData, Vector3> GetSrcPoint(AvatarBones bone)
        {
            return _objSrcPoints[(int)bone];
        }

        public Vector3 ParseVertexData(int rowNumber)
        {
            string rowString = _objLines[rowNumber + _rowsOffset];
            string[] elements = rowString.Split(' ');
            return new Vector3(-float.Parse(elements[1]), float.Parse(elements[2]), float.Parse(elements[3])) + (_objOffset / _bodyScale);
        }

        Dictionary<RowData, Vector3>[] LoadSrcPoints(int length)
        {
            var result = new Dictionary<RowData, Vector3>[length];
            Dictionary<AvatarBones, int?[]> avatarBonesObjRows = GetAvatarBonesObjRows();

            for (int i = 0; i < length; i++)
            {
                int?[] row = avatarBonesObjRows[(AvatarBones)i];

                foreach (RowData data in Enum.GetValues(typeof(RowData)))
                {
                    if (row?[(int)data] == null) continue;
                    Vector3 v = ParseVertexData((int)row[(int)data]);
                    if (result[i] == null) result[i] = new Dictionary<RowData, Vector3>();
                    result[i][data] = v;
                }
            }

            return result;
        }

        Vector3 CalcSpineLineVertex(IReadOnlyList<int?> row, IReadOnlyList<string> objInfo, float yRate, float zRate)
        {
            float sumX = 0.0f;

            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward]);
            sumX += v.x;
            float forwardY = v.y;
            float forwardZ = v.z;

            v = ParseVertexData((int)row[(int)RowData.Back]);
            sumX += v.x;
            float backY = v.y;
            float backZ = v.z;

            return new Vector3(sumX / 2.0f, Mathf.Lerp(forwardY, backY, yRate), Mathf.Lerp(forwardZ, backZ, zRate));
        }

        Vector3 CalcDirection(AvatarBones from, AvatarBones to)
        {
            Vector3 fromV = ObjPoints[(int)from];
            Vector3 toV = ObjPoints[(int)to];

            Vector3 direction = toV - fromV;
            direction = direction.normalized;
            return direction;
        }

        Vector3 CalcLimbVertex(int?[] row, float zRate)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));
            float sumX = 0.0f;
            float sumY = 0.0f;

            Vector3 v = ParseVertexData((int)row[(int)RowData.Left]);
            sumX += v.x;
            sumY += v.y;

            v = ParseVertexData((int)row[(int)RowData.Right]);
            sumX += v.x;
            sumY += v.y;

            v = ParseVertexData((int)row[(int)RowData.Forward]);
            float forwardZ = v.z;

            v = ParseVertexData((int)row[(int)RowData.Back]);
            float backZ = v.z;

            return new Vector3(sumX / 2.0f, sumY / 2.0f, Mathf.Lerp(forwardZ, backZ, zRate));
        }

        public Dictionary<AvatarBones, int?[]> GetAvatarBonesObjRows()
        {
            return _isMale ? _maleAvatarBonesObjRows : _femaleAvatarBonesObjRows;
        }

        Dictionary<AvatarBones, int?[]> GetMaleAvatarBonesObjRows()
        {
            return new Dictionary<AvatarBones, int?[]>
            {
                {AvatarBones.Hips, new int?[] {null, null, 6602, 26148, null, null } },
                {AvatarBones.Jushin, null },
                {AvatarBones.LeftUpLeg, new int?[] { 4988, null, 22954, null, null, null } },
                {AvatarBones.LeftDownScale1, new int?[] { 4988, null, 22954, 4505, null, null } },
                {AvatarBones.LeftDownScale2, new int?[] { null, null, 22798, null, null, null } },
                {AvatarBones.LeftGreaterTrochanter, null },
                {AvatarBones.LeftLeg, new int?[] { null, null, 15881, 23121, null, null } },
                {AvatarBones.LeftPatella, null },
                {AvatarBones.LeftFoot, new int?[] { 5379, 5377, 16740, 22864, null, null } },
                {AvatarBones.LeftAnkle, null },
                {AvatarBones.LeftToe, new int?[] { null, null, 18199, null, 18204, null } },
                {AvatarBones.RightUpLeg, new int?[] { null, 10729, 14546, null, null, null } },
                {AvatarBones.RightDownScale1, new int?[] { null, 10729, 14546, null, null, null } },
                {AvatarBones.RightDownScale2, new int?[] { null, null, 14386, null, null, null } },
                {AvatarBones.RightGreaterTrochanter, null },
                {AvatarBones.RightLeg, new int?[] { null, null, 7470, 14712, null, null } },
                {AvatarBones.RightPatella, null },
                {AvatarBones.RightFoot, new int?[] { 3250, 3252, 8342, 14457, null, null } },
                {AvatarBones.RightAnkle, null },
                {AvatarBones.RightToe, new int?[] { null, null, 9789, null, 9791, null } },
                {AvatarBones.SpineWaist, new int?[] { null, null, 26259, 11860, null, null } },
                {AvatarBones.Spine1, new int?[] { null, null, 26141, 26358, null, null } },
                {AvatarBones.Spine2, new int?[] { null, null, 3202, 8010, null, null } },
                {AvatarBones.Neck1, new int?[] { null, null, 3570, 924, null, null } },
                {AvatarBones.Neck2, new int?[] { 19717, 11300, null, null, null, null } },
                {AvatarBones.Neck3, new int?[] { 4692, 2560, null, null, null, null } },
                {AvatarBones.LeftEarlobe, new int?[] { null, null, null, null, 24044, null } },
                {AvatarBones.RightEarlobe, new int?[] { null, null, null, null, 15645, null } },
                {AvatarBones.Head, new int?[] { null, null, null, null, 3225, null } },
                {AvatarBones.LeftShoulder, new int?[] { null, null, 5124, null, 1058, null } },
                {AvatarBones.LeftAcromion, new int?[] { null, null, null, null, 19645, null } },
                {AvatarBones.LeftArm, new int?[] { null, null, 16419, 5575, null, null } },
                {AvatarBones.LeftUpScale1, new int?[] {  null, null, 16419, 5575, null, null } },
                {AvatarBones.LeftForeArm, new int?[] { 25805, 1662, 26016, 25443, null, null } },
                {AvatarBones.LeftHand, new int?[] { null, null, 6244, 6213, null, null } },
                {AvatarBones.LeftHand2, new int?[] { null, null, 24296, null, null, null } },
                {AvatarBones.RightShoulder, new int?[] { null, null, 2988, null, 776, null } },
                {AvatarBones.RightAcromion, new int?[] { null, null, null, null, 11232, null } },
                {AvatarBones.RightArm, new int?[] { null, null, 8017, 3448, null, null } },
                {AvatarBones.RightUpScale1, new int?[] { null, null, 8017, 3448, null, null } },
                {AvatarBones.RightForeArm, new int?[] { 7084, 28157, 28609, 7028, null, null } },
                {AvatarBones.RightHand, new int?[] { null, null, 6853, 27220, null, null } },
                {AvatarBones.RightHand2, new int?[] { null, null, 26898, null, null, null } },
                {AvatarBones.Root, null }
            };
        }

        Dictionary<AvatarBones, int?[]> GetFemaleAvatarBonesObjRows()
        {
            return new Dictionary<AvatarBones, int?[]>
            {
                {AvatarBones.Hips, new int?[] {null, null, 6602, 26148, null, null } },
                {AvatarBones.Jushin, null },
                {AvatarBones.LeftUpLeg, new int?[] { 4988, null, 22954, null, null, null } },
                {AvatarBones.LeftDownScale1, new int?[] { 4988, null, 22954, 4505, null, null } },
                {AvatarBones.LeftDownScale2, new int?[] { null, null, 22798, null, null, null } },
                {AvatarBones.LeftGreaterTrochanter, null },
                {AvatarBones.LeftLeg, new int?[] { null, null, 15881, 23121, null, null } },
                {AvatarBones.LeftPatella, null },
                {AvatarBones.LeftFoot, new int?[] { 5379, 5377, 16740, 22864, null, null } },
                {AvatarBones.LeftAnkle, null },
                {AvatarBones.LeftToe, new int?[] { null, null, 18199, null, 18204, null } },
                {AvatarBones.RightUpLeg, new int?[] { null, 10729, 14546, null, null, null } },
                {AvatarBones.RightDownScale1, new int?[] { null, 10729, 14546, null, null, null } },
                {AvatarBones.RightDownScale2, new int?[] { null, null, 14386, null, null, null } },
                {AvatarBones.RightGreaterTrochanter, null },
                {AvatarBones.RightLeg, new int?[] { null, null, 7470, 14712, null, null } },
                {AvatarBones.RightPatella, null },
                {AvatarBones.RightFoot, new int?[] { 3250, 3252, 8342, 14457, null, null } },
                {AvatarBones.RightAnkle, null },
                {AvatarBones.RightToe, new int?[] { null, null, 9789, null, 9791, null } },
                {AvatarBones.SpineWaist, new int?[] { null, null, 26259, 11860, null, null } },
                {AvatarBones.Spine1, new int?[] { null, null, 26141, 26358, null, null } },
                {AvatarBones.Spine2, new int?[] { null, null, 3202, 8010, null, null } },
                {AvatarBones.Neck1, new int?[] { null, null, 3570, 924, null, null } },
                {AvatarBones.Neck2, new int?[] { 19717, 11300, null, null, null, null } },
                {AvatarBones.Neck3, new int?[] { 4692, 2560, null, null, null, null } },
                {AvatarBones.LeftEarlobe, new int?[] { null, null, null, null, 24044, null } },
                {AvatarBones.RightEarlobe, new int?[] { null, null, null, null, 15645, null } },
                {AvatarBones.Head, new int?[] { null, null, null, null, 3225, null } },
                {AvatarBones.LeftShoulder, new int?[] { null, null, 5124, null, 1058, null } },
                {AvatarBones.LeftAcromion, new int?[] { null, null, null, null, 19645, null } },
                {AvatarBones.LeftArm, new int?[] { null, null, 4189, 23237, null, null } },
                {AvatarBones.LeftUpScale1, new int?[] { null, null, 4189, 23237, null, null } },
                {AvatarBones.LeftForeArm, new int?[] { 25805, 1662, 26016, 25443, null, null } },
                {AvatarBones.LeftHand, new int?[] { null, null, 6244, 6213, null, null } },
                {AvatarBones.LeftHand2, new int?[] { null, null, 24296, null, null, null } },
                {AvatarBones.RightShoulder, new int?[] { null, null, 2988, null, 776, null } },
                {AvatarBones.RightAcromion, new int?[] { null, null, null, null, 11232, null } },
                {AvatarBones.RightArm, new int?[] { null, null, 2055, 14829, null, null } },
                {AvatarBones.RightUpScale1, new int?[] { null, null, 2055, 14829, null, null } },
                {AvatarBones.RightForeArm, new int?[] { 7084, 28157, 28609, 7028, null, null } },
                {AvatarBones.RightHand, new int?[] { null, null, 6853, 27220, null, null } },
                {AvatarBones.RightHand2, new int?[] { null, null, 26898, null, null, null } },
                {AvatarBones.Root, null }
            };
        }
    }
}
