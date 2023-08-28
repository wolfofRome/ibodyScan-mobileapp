using System;
using System.Collections.Generic;
using UnityEngine;

namespace FitAndShape
{
    public interface IPostureVerifyerModel
    {
        Dictionary<PostureVerifyPoint, Result> ResultMap { get; }
        Result[] GetAbnormalResults();
        Result[] GetAbnormalResultsByAngle(Angle angle);
    }

    public sealed class PostureVerifyerModel : IPostureVerifyerModel
    {
        readonly static float MaxPercentage = 100f;

        readonly IAvatarModel avatarModel;
        readonly IMeasurementCsvLoader measurementCsvLoader;
        readonly Dictionary<PostureVerifyPoint, Result> resultMap;

        Dictionary<PostureVerifyPoint, Result> IPostureVerifyerModel.ResultMap => resultMap;

        public PostureVerifyerModel(IAvatarModel avatarModel, IMeasurementCsvLoader measurementCsvLoader)
        {
            this.avatarModel = avatarModel;
            this.measurementCsvLoader = measurementCsvLoader;

            resultMap = new Dictionary<PostureVerifyPoint, Result>();

            VerifyPostureOfSideAngle();

            VerifyPostureOfFrontAngle();

            VerifyPostureOfTopAngle();

            VerifyPostureOfUnderAngle();
        }

        Result[] IPostureVerifyerModel.GetAbnormalResults()
        {
            List<Result> results = new List<Result>();

            foreach (Angle angle in Enum.GetValues(typeof(Angle)))
            {
                results.AddRange(GetAbnormalResultsByAngle(angle));
            }

            return results.ToArray();
        }

        /// <summary>
        /// 各視点における異常個所の取得.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        Result[] IPostureVerifyerModel.GetAbnormalResultsByAngle(Angle angle)
        {
            return GetAbnormalResultsByAngle(angle);
        }

        Result[] GetAbnormalResultsByAngle(Angle angle)
        {
            PostureVerifyPoint[] points = null;

            List<Result> resultList = new List<Result>();

            switch (angle)
            {
                case Angle.Left:
                case Angle.Right:
                    points = new PostureVerifyPoint[]
                    {
                        PostureVerifyPoint.NeckCurveAngle,
                        PostureVerifyPoint.UpperInclinationRatio,
                        PostureVerifyPoint.PelvisInclinationAngle,
                        PostureVerifyPoint.LumbarVertebraAngle,
                        PostureVerifyPoint.ThoracicVertebraAngle,
                    };
                    break;
                case Angle.Front:
                    points = new PostureVerifyPoint[]
                    {
                        PostureVerifyPoint.ShoulderInclinationRatio,
                        PostureVerifyPoint.PelvisInclinationRatio,
                    };
                    break;
                case Angle.Back:
                    points = new PostureVerifyPoint[]
                    {
                        PostureVerifyPoint.UpperLegLengthRatio,
                        PostureVerifyPoint.KneeJointAngle,
                    };
                    break;
                case Angle.Top:
                    points = new PostureVerifyPoint[]
                    {
                        PostureVerifyPoint.NeckDistortionAngle,
                        PostureVerifyPoint.ShoulderDistortionAngle,
                        PostureVerifyPoint.UpperBodyDistortionAngle,
                    };
                    break;
                case Angle.Under:
                    points = new PostureVerifyPoint[]
                    {
                        PostureVerifyPoint.KneeAnteroposteriorDiffRatio,
                    };
                    break;
            }

            if (points != null)
            {
                Result result = null;

                foreach (var point in points)
                {
                    if (resultMap.TryGetValue(point, out result) && result.Condition != PostureCondition.Normal)
                    {
                        resultList.Add(result);
                    }
                }
            }

            return resultList.ToArray();
        }

        /// <summary>
        /// 横アングルにおける姿勢検証.
        /// </summary>
        void VerifyPostureOfSideAngle()
        {
            // 1.重心Y軸ラインと耳の乖離.
            {
                PostureVerifyPoint point = PostureVerifyPoint.UpperInclinationRatio;
                RangeThreshold threshold = point.ToRangeThreshold();

                Vector3 leftEarlobe = (Vector3)avatarModel.GetPoint(AvatarBones.LeftEarlobe);
                Vector3 rightEarlobe = (Vector3)avatarModel.GetPoint(AvatarBones.RightEarlobe);
                Vector3 jushin = (Vector3)avatarModel.GetPoint(AvatarBones.Jushin);
                float leftDiff = (leftEarlobe.z - jushin.z);
                float rightDiff = (rightEarlobe.z - jushin.z);

                float diffLength = (leftDiff + rightDiff) / 2;
                float diffPerHeight = MaxPercentage * diffLength / (float)measurementCsvLoader.GetValue(MeasurementPart.Hieght);

                int ret = threshold.CompareTo(diffPerHeight);
                Vector3 basePoint = leftDiff > rightDiff ? leftEarlobe : rightEarlobe;

                Result result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.ForwardTiltHead : PostureCondition.BackwardTiltHead),
                    FocusPoints = new Vector3[] { basePoint },
                    RawValues = new float[] { diffPerHeight },
                    DispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { jushin, new Vector3(basePoint.x, basePoint.y, jushin.z) });
                result.AddMeasurementLinePoints(new Vector3[] { jushin, basePoint });

                resultMap[point] = result;
            }

            // 2.重心Y軸ラインとヒザの乖離
            {
                PostureVerifyPoint point = PostureVerifyPoint.LowerInclinationRatio;
                var threshold = point.ToRangeThreshold();

                var leftPatella = (Vector3)avatarModel.GetPoint(AvatarBones.LeftPatella);
                var rightPatella = (Vector3)avatarModel.GetPoint(AvatarBones.RightPatella);
                var jushin = (Vector3)avatarModel.GetPoint(AvatarBones.Jushin);
                var leftDiff = (leftPatella.z - jushin.z);
                var rightDiff = (rightPatella.z - jushin.z);

                var diffLength = (leftDiff + rightDiff) / 2;
                var diffPerHeight = MaxPercentage * diffLength / (float)measurementCsvLoader.GetValue(MeasurementPart.Hieght);

                var ret = threshold.CompareTo(diffPerHeight);
                var basePoint = leftDiff > rightDiff ? leftPatella : rightPatella;

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.ForwardTiltKnee : PostureCondition.BackwardTiltKnee),
                    FocusPoints = new Vector3[] { basePoint },
                    RawValues = new float[] { diffPerHeight },
                    DispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { jushin, new Vector3(basePoint.x, basePoint.y, jushin.z) });
                result.AddMeasurementLinePoints(new Vector3[] { jushin, basePoint });
                resultMap[point] = result;
            }

            // 3.骨盤の前後傾斜角度.
            {
                PostureVerifyPoint point = PostureVerifyPoint.PelvisInclinationAngle;
                var threshold = point.ToRangeThreshold();

                var hips = (Vector3)avatarModel.GetPoint(AvatarBones.Hips);
                var hipsRotation = ((Quaternion)avatarModel.GetRotation(AvatarBones.Hips)).eulerAngles.x;

                var ret = threshold.CompareTo(hipsRotation);
                var diff = threshold.Diff(hipsRotation);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.ForwardTiltPelvis : PostureCondition.BackwardTiltPelvis),
                    FocusPoints = new Vector3[] { (Vector3)avatarModel.GetPoint(AvatarBones.Hips) },
                    RawValues = new float[] { hipsRotation },
                    DispValues = new float[] { diff }
                };

                var lineEndPoint = hips + new Vector3(0, 1 / AppConst.ObjLoadScale, 0);
                var measurementPoint = RotateAround(hipsRotation, hips, lineEndPoint, Vector3.right);
                var normalPoint = RotateAround(-diff, hips, measurementPoint, Vector3.right);

                result.AddBaseLinePoints(new Vector3[] { hips, normalPoint });
                result.AddMeasurementLinePoints(new Vector3[] { hips, measurementPoint });

                resultMap[point] = result;
            }

            // 4.背骨(胸椎)角度の正常値からの誤差.
            {
                PostureVerifyPoint point = PostureVerifyPoint.ThoracicVertebraAngle;
                var threshold = point.ToRangeThreshold();

                var spine1 = (Vector3)avatarModel.GetPoint(AvatarBones.Spine1);
                var spine1Rotation = ((Quaternion)avatarModel.GetRotation(AvatarBones.Spine1)).eulerAngles.x;

                var ret = threshold.CompareTo(spine1Rotation);
                var diff = threshold.Diff(spine1Rotation);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.Stoop : PostureCondition.FlatBack),
                    FocusPoints = new Vector3[] { (Vector3)avatarModel.GetPoint(AvatarBones.Spine1) },
                    RawValues = new float[] { spine1Rotation },
                    DispValues = new float[] { diff }
                };

                var lineEndPoint = spine1 + new Vector3(0, 1 / AppConst.ObjLoadScale, 0);
                var measurementPoint = RotateAround(spine1Rotation, spine1, lineEndPoint, Vector3.right);
                var normalPoint = RotateAround(-diff, spine1, measurementPoint, Vector3.right);

                result.AddBaseLinePoints(new Vector3[] { spine1, normalPoint });
                result.AddMeasurementLinePoints(new Vector3[] { spine1, measurementPoint });

                resultMap[point] = result;
            }

            // 5.背骨(腰椎上部)角度の正常値からの誤差.
            {
                PostureVerifyPoint point = PostureVerifyPoint.LumbarVertebraAngle;
                var threshold = point.ToRangeThreshold();

                var spineWaist = (Vector3)avatarModel.GetPoint(AvatarBones.SpineWaist);
                var spineWaistRotation = ((Quaternion)avatarModel.GetRotation(AvatarBones.SpineWaist)).eulerAngles.x;

                var ret = threshold.CompareTo(spineWaistRotation);
                var diff = threshold.Diff(spineWaistRotation);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LumbarFlat : PostureCondition.StomachProtruding),
                    FocusPoints = new Vector3[] { (Vector3)avatarModel.GetPoint(AvatarBones.SpineWaist) },
                    RawValues = new float[] { spineWaistRotation },
                    DispValues = new float[] { diff }
                };

                var lineEndPoint = spineWaist + new Vector3(0, 1 / AppConst.ObjLoadScale, 0);
                var measurementPoint = RotateAround(spineWaistRotation, spineWaist, lineEndPoint, Vector3.right);
                var normalPoint = RotateAround(-diff, spineWaist, measurementPoint, Vector3.right);

                result.AddBaseLinePoints(new Vector3[] { spineWaist, normalPoint });
                result.AddMeasurementLinePoints(new Vector3[] { spineWaist, measurementPoint });

                resultMap[point] = result;
            }

            // 6.首の湾曲角度の正常値からの誤差.
            {
                PostureVerifyPoint point = PostureVerifyPoint.NeckCurveAngle;
                var threshold = point.ToRangeThreshold();

                var neck2 = (Vector3)avatarModel.GetPoint(AvatarBones.Neck2);
                var neck2Rotation = ((Quaternion)avatarModel.GetRotation(AvatarBones.Neck2)).eulerAngles.x;

                var ret = threshold.CompareTo(neck2Rotation);
                var diff = threshold.Diff(neck2Rotation);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.StraightNeck : PostureCondition.NeckBackFlexion),
                    FocusPoints = new Vector3[] { (Vector3)avatarModel.GetPoint(AvatarBones.Neck2) },
                    RawValues = new float[] { neck2Rotation },
                    DispValues = new float[] { diff }
                };

                var lineEndPoint = neck2 + new Vector3(0, 1 / AppConst.ObjLoadScale, 0);
                var measurementPoint = RotateAround(neck2Rotation, neck2, lineEndPoint, Vector3.right);
                var normalPoint = RotateAround(-diff, neck2, measurementPoint, Vector3.right);

                result.AddBaseLinePoints(new Vector3[] { neck2, normalPoint });
                result.AddMeasurementLinePoints(new Vector3[] { neck2, measurementPoint });

                resultMap[point] = result;
            }
        }

        /// <summary>
        /// 前面アングルにおける姿勢検証.
        /// </summary>
        void VerifyPostureOfFrontAngle()
        {
            // 1.重心Y軸ラインと頭頂との乖離.
            {
                PostureVerifyPoint point = PostureVerifyPoint.BodyInclinationRatio;
                var threshold = point.ToRangeThreshold();

                var head = (Vector3)avatarModel.GetPoint(AvatarBones.Head);
                var jushin = (Vector3)avatarModel.GetPoint(AvatarBones.Jushin);

                var diffLength = head.x - jushin.x;
                var diffPerHeight = MaxPercentage * diffLength / (float)measurementCsvLoader.GetValue(MeasurementPart.Hieght);

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftTiltBody : PostureCondition.RightTiltBody),
                    FocusPoints = new Vector3[] { (head + jushin) / 2 },
                    RawValues = new float[] { diffPerHeight },
                    DispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { new Vector3(head.x, 0, 0), new Vector3(head.x, head.y, 0) });
                result.AddMeasurementLinePoints(new Vector3[] { new Vector3(head.x, jushin.y, 0), new Vector3(jushin.x, head.y, 0) });
                resultMap[point] = result;
            }

            // 2.大腿骨大転子から膝頭までの両足間の差 （右ー左）.
            {
                PostureVerifyPoint point = PostureVerifyPoint.UpperLegLengthRatio;
                var threshold = point.ToRangeThreshold();

                var leftGreaterTrochanter = (Vector3)avatarModel.GetPoint(AvatarBones.LeftGreaterTrochanter);
                var leftPatella = (Vector3)avatarModel.GetPoint(AvatarBones.LeftPatella);
                var leftLength = Vector3.Distance(leftGreaterTrochanter, leftPatella);

                var rightGreaterTrochanter = (Vector3)avatarModel.GetPoint(AvatarBones.RightGreaterTrochanter);
                var rightPatella = (Vector3)avatarModel.GetPoint(AvatarBones.RightPatella);
                var rightLength = Vector3.Distance(rightGreaterTrochanter, rightPatella);

                var diffLength = rightLength - leftLength;
                var diffPerHeight = MaxPercentage * diffLength / (float)measurementCsvLoader.GetValue(MeasurementPart.Hieght);

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftThighShort : PostureCondition.RightThighShort),
                    FocusPoints = new Vector3[] { (leftGreaterTrochanter + rightGreaterTrochanter) / 2 },
                    RawValues = new float[] { diffPerHeight },
                    DispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { leftGreaterTrochanter, leftPatella });
                result.AddBaseLinePoints(new Vector3[] { rightGreaterTrochanter, rightPatella });
                resultMap[point] = result;
            }

            // 3.膝頭から足首までの高さの両足間の差.
            {
                PostureVerifyPoint point = PostureVerifyPoint.LowerLegLengthRatio;
                var threshold = point.ToRangeThreshold();

                var leftPatella = (Vector3)avatarModel.GetPoint(AvatarBones.LeftPatella);
                var leftAnkle = (Vector3)avatarModel.GetPoint(AvatarBones.LeftAnkle);
                var leftLength = Vector3.Distance(leftPatella, leftAnkle);

                var rightPatella = (Vector3)avatarModel.GetPoint(AvatarBones.RightPatella);
                var rightAnkle = (Vector3)avatarModel.GetPoint(AvatarBones.RightAnkle);
                var rightLength = Vector3.Distance(rightPatella, rightAnkle);

                var diffLength = rightLength - leftLength;
                var diffPerHeight = MaxPercentage * diffLength / (float)measurementCsvLoader.GetValue(MeasurementPart.Hieght);

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftLowerLegLengthShort : PostureCondition.RightLowerLegLengthShort),
                    FocusPoints = new Vector3[] { ((leftPatella + rightPatella) / 2 + (leftAnkle + rightAnkle) / 2) / 2 },
                    RawValues = new float[] { diffPerHeight },
                    DispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { leftPatella, leftAnkle });
                result.AddBaseLinePoints(new Vector3[] { rightPatella, rightAnkle });
                resultMap[point] = result;
            }

            // 4.肩峰の左右の高さの差（右ー左）.
            {
                PostureVerifyPoint point = PostureVerifyPoint.ShoulderInclinationRatio;
                var threshold = point.ToRangeThreshold();

                var left = (Vector3)avatarModel.GetPoint(AvatarBones.LeftAcromion);
                var right = (Vector3)avatarModel.GetPoint(AvatarBones.RightAcromion);

                var diffLength = right.y - left.y;
                var diffPerHeight = MaxPercentage * diffLength / (float)measurementCsvLoader.GetValue(MeasurementPart.Hieght);

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftTiltAcromion : PostureCondition.RightTiltAcromion),
                    FocusPoints = new Vector3[] { (right + left) / 2 },
                    RawValues = new float[] { diffPerHeight },
                    DispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { right, new Vector3(left.x, right.y, right.z) });
                result.AddMeasurementLinePoints(new Vector3[] { right, left });
                resultMap[point] = result;
            }

            // 5.左右の上前腸骨棘の高さの差（右ー左）.
            {
                PostureVerifyPoint point = PostureVerifyPoint.PelvisInclinationRatio;
                var threshold = point.ToRangeThreshold();

                var right = (Vector3)avatarModel.GetPoint(AvatarBones.RightDownScale2);
                var left = (Vector3)avatarModel.GetPoint(AvatarBones.LeftDownScale2);

                var diffLength = right.y - left.y;
                var diffPerHeight = MaxPercentage * diffLength / (float)measurementCsvLoader.GetValue(MeasurementPart.Hieght);

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftTiltPelvis : PostureCondition.RightTiltPelvis),
                    FocusPoints = new Vector3[] { (right + left) / 2 },
                    RawValues = new float[] { diffPerHeight },
                    DispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { right, new Vector3(left.x, right.y, right.z) });
                result.AddMeasurementLinePoints(new Vector3[] { right, left });
                resultMap[point] = result;
            }

            // 6.ヒザ関節の外反角度（FTA）.
            {
                PostureVerifyPoint point = PostureVerifyPoint.KneeJointAngle;
                var threshold = point.ToRangeThreshold();

                var leftGreaterTrochanter = (Vector3)avatarModel.GetPoint(AvatarBones.LeftGreaterTrochanter);
                var leftPatella = (Vector3)avatarModel.GetPoint(AvatarBones.LeftPatella);
                var leftAnkle = (Vector3)avatarModel.GetPoint(AvatarBones.LeftAnkle);
                var leftFTA = GetFTAAngle(leftGreaterTrochanter, leftPatella, leftAnkle);
                var leftResult = threshold.CompareTo(leftFTA);
                var leftDiff = threshold.Diff(leftFTA);

                var rightGreaterTrochanter = (Vector3)avatarModel.GetPoint(AvatarBones.RightGreaterTrochanter);
                var rightPatella = (Vector3)avatarModel.GetPoint(AvatarBones.RightPatella);
                var rightAnkle = (Vector3)avatarModel.GetPoint(AvatarBones.RightAnkle);
                var rightFTA = GetFTAAngle(rightGreaterTrochanter, rightPatella, rightAnkle);
                var rightResult = threshold.CompareTo(rightFTA);
                var rightDiff = threshold.Diff(rightFTA);

                var result = new Result
                {
                    Point = point,
                    Condition = (leftResult > 0 || rightResult > 0) ? PostureCondition.BowLegs : ((leftResult < 0 || rightResult < 0) ? PostureCondition.KnockKnees : PostureCondition.Normal),
                };

                var focusPointList = new List<Vector3>();
                var rawValueList = new List<float>();
                var dispValueList = new List<float>();
                if (leftResult != 0)
                {
                    focusPointList.Add(leftPatella);
                    rawValueList.Add(leftFTA);
                    dispValueList.Add(leftDiff);

                    result.AddBaseLinePoints(new Vector3[] { RotateAround(leftDiff / 2, leftPatella, leftGreaterTrochanter, Vector3.forward), leftPatella, RotateAround(-leftDiff / 2, leftPatella, leftAnkle, Vector3.forward) });
                    result.AddMeasurementLinePoints(new Vector3[] { leftGreaterTrochanter, leftPatella, leftAnkle });
                }

                if (rightResult != 0)
                {
                    focusPointList.Add(rightPatella);
                    rawValueList.Add(rightFTA);
                    dispValueList.Add(rightDiff);
                    result.AddBaseLinePoints(new Vector3[] { RotateAround(-rightDiff / 2, rightPatella, rightGreaterTrochanter, Vector3.forward), rightPatella, RotateAround(rightDiff / 2, rightPatella, rightAnkle, Vector3.forward) });
                    result.AddMeasurementLinePoints(new Vector3[] { rightGreaterTrochanter, rightPatella, rightAnkle });
                }

                result.FocusPoints = focusPointList.ToArray();
                result.RawValues = rawValueList.ToArray();
                result.DispValues = dispValueList.ToArray();

                resultMap[point] = result;
            }
        }

        /// <summary>
        /// 上アングルにおける姿勢検証.
        /// </summary>
        void VerifyPostureOfTopAngle()
        {
            // 1.左右の鎖骨角度の正常角度からの差.
            {
                PostureVerifyPoint point = PostureVerifyPoint.ShoulderDistortionAngle;
                var threshold = point.ToRangeThreshold();

                var leftArm = (Vector3)avatarModel.GetPoint(AvatarBones.LeftArm);
                var leftShoulder = (Vector3)avatarModel.GetPoint(AvatarBones.LeftShoulder);
                var leftRotation = ((Quaternion)avatarModel.GetRotation(AvatarBones.LeftShoulder)).eulerAngles.y;
                var leftResult = threshold.CompareTo(leftRotation);

                var rightArm = (Vector3)avatarModel.GetPoint(AvatarBones.RightArm);
                var rightShoulder = (Vector3)avatarModel.GetPoint(AvatarBones.RightShoulder);
                var rightRotation = ((Quaternion)avatarModel.GetRotation(AvatarBones.RightShoulder)).eulerAngles.y;
                var rightResult = threshold.CompareTo(rightRotation);

                var leftDiff = threshold.Diff(leftRotation);
                var rightDiff = threshold.Diff(rightRotation);

                var result = new Result
                {
                    Point = point,
                    Condition = ((leftResult > 0 && rightResult > 0) ? PostureCondition.RightTwistShoulder : (leftResult < 0 && rightResult < 0) ? PostureCondition.LeftTwistShoulder : PostureCondition.Normal),
                    FocusPoints = new Vector3[] { (leftShoulder + leftArm) / 2, (rightShoulder + rightArm) / 2 },
                    RawValues = new float[] { leftRotation, rightRotation },
                    DispValues = new float[] { leftDiff, rightDiff }
                };

                var leftArmNormal = RotateAround(-leftDiff, leftShoulder, leftArm, Vector3.up);
                var rightArmNormal = RotateAround(-rightDiff, rightShoulder, rightArm, Vector3.up);

                result.AddBaseLinePoints(new Vector3[] { leftShoulder, new Vector3(leftArmNormal.x, leftShoulder.y, leftArmNormal.z) });
                result.AddBaseLinePoints(new Vector3[] { rightShoulder, new Vector3(rightArmNormal.x, rightShoulder.y, rightArmNormal.z) });

                result.AddMeasurementLinePoints(new Vector3[] { leftShoulder, leftArm });
                result.AddMeasurementLinePoints(new Vector3[] { rightShoulder, rightArm });

                resultMap[point] = result;
            }

            // 2.首の左右回転角度.
            {
                PostureVerifyPoint point = PostureVerifyPoint.NeckDistortionAngle;
                var threshold = point.ToRangeThreshold();

                var neck3 = (Vector3)avatarModel.GetPoint(AvatarBones.Neck3);
                var neck3Rotation = ((Quaternion)avatarModel.GetRotation(AvatarBones.Neck3)).eulerAngles.y;

                var ret = threshold.CompareTo(neck3Rotation);
                var diff = threshold.Diff(neck3Rotation);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.RightTwistNeck : PostureCondition.LeftTwistNeck),
                    FocusPoints = new Vector3[] { (Vector3)avatarModel.GetPoint(AvatarBones.Neck3) },
                    RawValues = new float[] { neck3Rotation },
                    DispValues = new float[] { diff }
                };

                var linePoint = new Vector3(neck3.x, neck3.y, neck3.x + 200);
                result.AddBaseLinePoints(new Vector3[] { neck3, linePoint });
                result.AddMeasurementLinePoints(new Vector3[] { neck3, RotateAround(diff, neck3, linePoint, Vector3.up) });

                resultMap[PostureVerifyPoint.NeckDistortionAngle] = result;
            }
        }

        /// <summary>
        /// 下アングルにおける姿勢検証.
        /// </summary>
        void VerifyPostureOfUnderAngle()
        {
            // 1.腰の左右を結ぶ線と、肩の左右を結ぶ線の角度差（腰に対する肩）.
            {
                PostureVerifyPoint point = PostureVerifyPoint.UpperBodyDistortionAngle;
                var threshold = point.ToRangeThreshold();

                var jushinRotationY = ((Quaternion)avatarModel.GetRotation(AvatarBones.Jushin)).eulerAngles.y;

                var leftAcromion = (Vector3)avatarModel.GetPoint(AvatarBones.LeftAcromion);
                var rightAcromion = (Vector3)avatarModel.GetPoint(AvatarBones.RightAcromion);
                var leftRightDiff = leftAcromion - rightAcromion;
                var acromionAngle = Mathf.Atan2(leftRightDiff.z, leftRightDiff.x);

                var acromionAngleDiff = acromionAngle - jushinRotationY;

                var ret = threshold.CompareTo(acromionAngleDiff);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.RightTwistWaist : PostureCondition.LeftTwistWaist),
                    FocusPoints = new Vector3[] { (leftAcromion + rightAcromion) / 2 },
                    RawValues = new float[] { acromionAngleDiff },
                    DispValues = new float[] { threshold.Diff(acromionAngleDiff) }
                };
                var halfLength = Vector3.Distance(leftAcromion, rightAcromion) / 2;
                var z = halfLength * Mathf.Sin(jushinRotationY * Mathf.Deg2Rad);
                var baseStartPoint = new Vector3(-halfLength, rightAcromion.y, -z);
                var baseEndPoint = new Vector3(halfLength, rightAcromion.y, z);
                result.AddBaseLinePoints(new Vector3[] { baseStartPoint, baseEndPoint });
                result.AddMeasurementLinePoints(new Vector3[] { leftAcromion, rightAcromion });
                resultMap[point] = result;
            }

            // 2.膝頭の前後差（右ー左）.
            {
                PostureVerifyPoint point = PostureVerifyPoint.KneeAnteroposteriorDiffRatio;
                var threshold = point.ToRangeThreshold();

                var left = (Vector3)avatarModel.GetPoint(AvatarBones.LeftPatella);
                var right = (Vector3)avatarModel.GetPoint(AvatarBones.RightPatella);

                var diffLength = right.z - left.z;
                var diffPerHeight = MaxPercentage * diffLength / (float)measurementCsvLoader.GetValue(MeasurementPart.Hieght);

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    Point = point,
                    Condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.RightKneeProtruding : PostureCondition.LeftKneeProtruding),
                    FocusPoints = new Vector3[] { (right + left) / 2 },
                    RawValues = new float[] { diffPerHeight },
                    DispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { right, new Vector3(left.x, right.y, right.z) });
                result.AddMeasurementLinePoints(new Vector3[] { right, left });
                resultMap[point] = result;
            }
        }

        /// <summary>
        /// 膝関節のFTA角度の取得.
        /// </summary>
        /// <param name="greaterTrochanter"></param>
        /// <param name="patella"></param>
        /// <param name="ankle"></param>
        /// <returns></returns>
        float GetFTAAngle(Vector3 greaterTrochanter, Vector3 patella, Vector3 ankle)
        {
            greaterTrochanter.z = 0;
            patella.z = 0;
            ankle.z = 0;
            return Vector3.Angle(greaterTrochanter - patella, ankle - patella);
        }

        /// <summary>
        /// 任意のポイント(target)をpivot周りにangle分回転した際の座標取得.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="pivot"></param>
        /// <param name="target"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        Vector3 RotateAround(float angle, Vector3 pivot, Vector3 target, Vector3 axis)
        {
            return pivot + Quaternion.AngleAxis(angle, axis) * (target - pivot);
        }
    }
}
