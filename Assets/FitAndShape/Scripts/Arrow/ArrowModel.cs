using System.Collections.Generic;
using UnityEngine;

namespace FitAndShape
{
    public interface IArrowModel
    {

    }

    public sealed class ArrowModel : IArrowModel
    {
        //readonly float _bodyScale;

        //Vector3 _neck2SrcRight = Vector3.zero;
        //Vector3 _neck2SrcLeft = Vector3.zero;
        //Vector3 _armRightCorrection = Vector2.zero;
        //Vector3 _armLeftCorrection = Vector3.zero;
        //float _armRightCorrectionValue = 0;
        //float _armLeftCorrectionValue = 0;

        ///// <summary>
        ///// 採寸個所矢印のサイズ調整用のポイント情報.
        ///// AvatarControllerのポーズ調整完了後、矢印のサイズと向きを調整する.
        ///// </summary>
        //private readonly Dictionary<MeasurementPartArrow, ArrowRenderInfo> _avatarBonesPontsDic = new Dictionary<MeasurementPartArrow, ArrowRenderInfo>() {
        //    { MeasurementPartArrow.Hieght,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftToe, AvatarBones.RightToe }, new AvatarBones[]{ AvatarBones.Head },ArrowType.Line) },
        //    { MeasurementPartArrow.InseamLength,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftToe, AvatarBones.RightToe },new AvatarBones[]{ AvatarBones.Hips },ArrowType.Line) },
        //    { MeasurementPartArrow.NeckCircumference,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Neck2 },null,ArrowType.Circle) },
        //    { MeasurementPartArrow.ChestCircumference,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Spine1 },null,ArrowType.Circle) },
        //    { MeasurementPartArrow.WaistCircumference,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.SpineWaist },null,ArrowType.Circle) },
        //    { MeasurementPartArrow.HipsCircumference,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Hips },null,ArrowType.Circle) },
        //    { MeasurementPartArrow.LeftThighCircumference,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftLeg },new AvatarBones[]{ AvatarBones.LeftUpLeg },ArrowType.Circle) },
        //    { MeasurementPartArrow.RightThighCircumference, new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.RightLeg },new AvatarBones[]{ AvatarBones.RightUpLeg },ArrowType.Circle) },
        //    { MeasurementPartArrow.LeftShoulderLength,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Neck1, AvatarBones.Neck1, AvatarBones.Neck2},new AvatarBones[]{ AvatarBones.LeftArm },ArrowType.Line) },
        //    { MeasurementPartArrow.RightShoulderLength,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Neck1, AvatarBones.Neck1, AvatarBones.Neck2 },new AvatarBones[]{ AvatarBones.RightArm },ArrowType.Line) },
        //    { MeasurementPartArrow.LeftSleeveLength,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftHand },new AvatarBones[]{ AvatarBones.LeftArm },ArrowType.Line) },
        //    { MeasurementPartArrow.RightSleeveLength,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.RightHand },new AvatarBones[]{ AvatarBones.RightArm },ArrowType.Line) },
        //    { MeasurementPartArrow.LeftArmCircumference,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftForeArm },new AvatarBones[]{ AvatarBones.LeftArm },ArrowType.Circle ) },
        //    { MeasurementPartArrow.RightArmCircumference,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.RightForeArm },new AvatarBones[]{ AvatarBones.RightArm },ArrowType.Circle ) },
        //    { MeasurementPartArrow.LeftCalfCircumference,new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftFoot },new AvatarBones[]{ AvatarBones.LeftLeg },ArrowType.Circle) },
        //    { MeasurementPartArrow.RightCalfCircumference, new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.RightFoot },new AvatarBones[]{ AvatarBones.RightLeg },ArrowType.Circle) },
        //};

        //public ArrowModel(IArrowView arrowView, IAvatarModel avatar, IMeasurement measurement, float bodyScale)
        //{
        //    _bodyScale = bodyScale;

        //    var arrowsDic = arrowView.GetArrowDictionary();

        //    OnAvatarUpdated(avatar);

        //    UpdateArrays(avatar, measurement, arrowsDic);
        //}

        ///// <summary>
        ///// 3Dモデルの更新イベント.
        ///// </summary>
        ///// <param name="avatar"></param>
        //void OnAvatarUpdated(IAvatarModel avatar)
        //{
        //    var neck2Src = avatar.GetSrcPoint(AvatarBones.Neck2);

        //    if (neck2Src != null)
        //    {
        //        if (neck2Src.ContainsKey(RowData.Right)) _neck2SrcRight = neck2Src[RowData.Right];
        //        if (neck2Src.ContainsKey(RowData.Left)) _neck2SrcLeft = neck2Src[RowData.Left];
        //    }

        //    var armRightSrc = avatar.GetSrcPoint(AvatarBones.RightArm);

        //    if (armRightSrc != null)
        //    {
        //        // 矢印の表示調整用の値を算出.
        //        _armRightCorrectionValue = GetMinDistance(avatar.GetPoint(AvatarBones.RightArm), armRightSrc) / 2;
        //        _armRightCorrection = Vector3.zero;
        //        _armRightCorrection.y = _armRightCorrectionValue;
        //    }

        //    var armLeftSrc = avatar.GetSrcPoint(AvatarBones.LeftArm);

        //    if (armLeftSrc != null)
        //    {
        //        // 矢印の表示調整用の値を算出.
        //        _armLeftCorrectionValue = GetMinDistance(avatar.GetPoint(AvatarBones.LeftArm), armLeftSrc) / 2;
        //        _armLeftCorrection = Vector3.zero;
        //        _armLeftCorrection.y = _armLeftCorrectionValue;
        //    }
        //}

        ///// <summary>
        ///// 矢印のサイズ、及び角度の更新.
        ///// </summary>
        ///// <param name="avatar"></param>
        ///// <param name="measurement"></param>
        //void UpdateArrays(IAvatarModel avatar, IMeasurement measurement, Dictionary<MeasurementPartArrow, SpriteRenderer> arrowsDic)
        //{
        //    foreach (KeyValuePair<MeasurementPartArrow, SpriteRenderer> pair in arrowsDic)
        //    {
        //        SpriteRenderer spriteRender = pair.Value;

        //        ArrowRenderInfo arrowRenderInfo = _avatarBonesPontsDic[pair.Key];

        //        Vector3? start = arrowRenderInfo.GetStartPoint(avatar);

        //        Vector3? end = arrowRenderInfo.GetEndPoint(avatar);

        //        float zPos = arrowRenderInfo.GetZValue(avatar);

        //        if (start != null && end != null)
        //        {
        //            // 2点指定時は位置、角度、スケールを調整.
        //            Vector3 startPosition = (Vector3)start;
        //            Vector3 endPosition = (Vector3)end;

        //            float distance;

        //            switch (pair.Key)
        //            {
        //                case MeasurementPartArrow.LeftSleeveLength:
        //                    endPosition += _armLeftCorrection;
        //                    distance = Vector2.Distance(endPosition, startPosition);
        //                    break;
        //                case MeasurementPartArrow.RightSleeveLength:
        //                    endPosition += _armRightCorrection;
        //                    distance = Vector2.Distance(endPosition, startPosition);
        //                    break;
        //                case MeasurementPartArrow.LeftShoulderLength:
        //                    startPosition.x = _neck2SrcLeft.x;
        //                    endPosition.x -= _armRightCorrectionValue;
        //                    endPosition += _armLeftCorrection;
        //                    startPosition += _armLeftCorrection;
        //                    distance = Vector2.Distance(endPosition, startPosition);
        //                    break;
        //                case MeasurementPartArrow.RightShoulderLength:
        //                    startPosition.x = _neck2SrcRight.x;
        //                    endPosition.x += _armLeftCorrectionValue;
        //                    endPosition += _armRightCorrection;
        //                    startPosition += _armRightCorrection;
        //                    distance = Vector2.Distance(endPosition, startPosition);
        //                    break;
        //                case MeasurementPartArrow.InseamLength:
        //                    distance = measurement.Inseam;
        //                    break;
        //                default:
        //                    distance = Vector2.Distance(endPosition, startPosition);
        //                    break;
        //            }

        //            startPosition.z = zPos;

        //            switch (arrowRenderInfo.ArrowType)
        //            {
        //                case ArrowType.Line:
        //                    // 2点間の距離と矢印のサイズからスケールを算出.
        //                    var scale = spriteRender.transform.lossyScale;
        //                    scale.y *= (distance * _bodyScale) / spriteRender.bounds.size.y;
        //                    spriteRender.transform.localScale = scale;
        //                    spriteRender.transform.position = startPosition * _bodyScale;
        //                    spriteRender.transform.rotation = Quaternion.Euler(0, 0, GetAngle(startPosition, endPosition) - 90);
        //                    break;
        //                case ArrowType.Circle:
        //                    spriteRender.transform.position = (startPosition + endPosition) / 2 * _bodyScale;
        //                    spriteRender.transform.rotation = Quaternion.Euler(0, 0, GetAngle(startPosition, endPosition) - 90);
        //                    break;
        //            }
        //        }
        //        else if (start != null)
        //        {
        //            // 1点指定時は位置のみ調整.
        //            spriteRender.transform.position = (Vector3)start * _bodyScale;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 2点間の角度取得.
        ///// </summary>
        ///// <param name="p1"></param>
        ///// <param name="p2"></param>
        ///// <returns></returns>
        //float GetAngle(Vector2 p1, Vector2 p2)
        //{
        //    float dx = p2.x - p1.x;
        //    float dy = p2.y - p1.y;
        //    return Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        //}

        ///// <summary>
        ///// 基準点からの最短距離の取得.
        ///// </summary>
        ///// <param name="center"></param>
        ///// <param name="src"></param>
        ///// <returns></returns>
        //float GetMinDistance(Vector3? center, Dictionary<RowData, Vector3> src)
        //{
        //    float average = 0;
        //    if (src != null && src.Count > 0 && center != null)
        //    {
        //        average = float.MaxValue;
        //        foreach (var v in src.Values)
        //        {
        //            average = Mathf.Min(Vector3.Distance((Vector3)center, v), average);
        //        }
        //    }
        //    return average;
        //}
    }
}
