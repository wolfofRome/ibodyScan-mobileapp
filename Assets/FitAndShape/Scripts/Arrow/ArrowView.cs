using Amatib.ObjViewer.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FitAndShape
{
    public sealed class ArrowView : MonoBehaviour
    {
        [SerializeField] Material _defaultMaterial;
        [SerializeField] Material _selectMaterial;
        [SerializeField] ArrowHead _arrowHeadPrefab;
        [SerializeField] Color _defaultColor;
        [SerializeField] Color _selectColor;
        [SerializeField] ArrowPart _arrowPartPrefab;
        [SerializeField] List<Camera> _cameraList;
        [SerializeField] Transform _arrowPartTransform;
        [SerializeField] CameraManager _cameraManager;

        List<ArrowPart> _arrowPartList = new List<ArrowPart>();
        float _defaultZoomValue;

        public CameraManager CameraManager => _cameraManager;
        public Vector3 MiddlePosition { get; private set; }
        public bool ArrowPartVisible { get { return _arrowPartTransform.gameObject.activeSelf; } set { _arrowPartTransform.gameObject.SetActive(value); } }
        public Vector3 Position { get { return transform.position; } set { transform.position = value; } }

        public void SetFieldOfView(float value)
        {
            foreach (Camera target in _cameraList)
            {
                target.fieldOfView = value;
            }
        }

        public void SetArrowPartPosition(Vector3 value)
        {
            _arrowPartTransform.position = value;
        }

        public void SetCameraViewPortRect(Vector2 value)
        {
            foreach (Camera camera in _cameraList)
            {
                camera.rect = new Rect(value.x, value.y, 1, 1);
            }
        }

        public void SetZoomValue()
        {
            _defaultZoomValue = _cameraManager.Zoom;
        }

        public void OnReset()
        {
            _cameraManager.Zoom = _defaultZoomValue;
        }

        public void SetCameraPosition(Vector3 position)
        {
            foreach (var target in _cameraList)
            {
                target.transform.position = position;
            }
        }

        public void Select(MeasurementPart measurementPart)
        {
            Clear();

            ArrowPart activeArrowPart = _arrowPartList.Where(n => n.MeasurementPart == measurementPart).FirstOrDefault();

            if (activeArrowPart == null)
            {
                return;
            }

            activeArrowPart.SetColor(_selectColor, _selectMaterial, "ActiveLine");
        }

        public void Clear()
        {
            foreach (var arrowPart in _arrowPartList)
            {
                arrowPart.SetColor(_defaultColor, _defaultMaterial, "Line");
            }
        }

        public void DrawArrow(IAvatarModel avatarModel, float bodyScale)
        {
            foreach (var item in _arrowPartList)
            {
                Destroy(item.gameObject);
            }

            _arrowPartList.Clear();

            MiddlePosition = GetMiddlePosition(avatarModel, bodyScale);

            int length = Enum.GetValues(typeof(MeasurementPart)).Length;

            for (int index = 0; index < length; index++)
            {
                MeasurementPart measurementPart = (MeasurementPart)index;

                if (!measurementPart.IsDisplay())
                {
                    continue;
                }

                SetCircumference(avatarModel, measurementPart, bodyScale);

                SetLine(avatarModel, measurementPart, bodyScale);

                SetHip(avatarModel, measurementPart, bodyScale);
            }
        }

        Vector3 GetMiddlePosition(IAvatarModel avatarModel, float bodyScale)
        {
            var hieghtPosition = GetHieghtPosition(avatarModel, bodyScale);

            var position = Vector3.Lerp(hieghtPosition.StartPosition, hieghtPosition.EndPosition, 0.5f);
            position.y *= -1f;

            return position;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void SetCircumference(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            int rowNumber = 0;

            switch (measurementPart)
            {
                case MeasurementPart.NeckCircumference:
                    rowNumber = 6581;
                    break;
                case MeasurementPart.ChestCircumference:
                    rowNumber = 11915;
                    break;
                case MeasurementPart.BustTopCircumference:
                    rowNumber = 915;
                    break;
                case MeasurementPart.WaistMaxCircumference:
                    rowNumber = 20198;
                    break;
                case MeasurementPart.WaistMinCircumference:
                    rowNumber = 26252;
                    break;
                case MeasurementPart.Hip1Circumference:
                    rowNumber = 902;
                    break;
                case MeasurementPart.Hip5Circumference:
                    rowNumber = 6665;
                    break;
                case MeasurementPart.RightUpperArmCircumference:
                    rowNumber = 29234;
                    break;
                case MeasurementPart.LeftUpperArmCircumference:
                    rowNumber = 28930;
                    break;
                case MeasurementPart.RightThighCircumference:
                    rowNumber = 14357;
                    break;
                case MeasurementPart.LeftThighCircumference:
                    rowNumber = 22765;
                    break;
                case MeasurementPart.RightLowerLegMaxCircumference:
                    rowNumber = 440;
                    break;
                case MeasurementPart.LeftLowerLegMaxCircumference:
                    rowNumber = 1394;
                    break;
                case MeasurementPart.RightLowerLegMinCircumference:
                    rowNumber = 11460;
                    break;
                case MeasurementPart.LeftLowerLegMinCircumference:
                    rowNumber = 19879;
                    break;
                case MeasurementPart.RightWristCircumference:
                    rowNumber = 27387;
                    break;
                case MeasurementPart.LeftWristCircumference:
                    rowNumber = 24783;
                    break;
                default:
                    break;
            }

            if (rowNumber == 0) return;

            Vector3 position = avatarModel.ParseVertexData(rowNumber) * bodyScale;

            ArrowPart arrowPart = Instantiate(_arrowPartPrefab, _arrowPartTransform);
            arrowPart.Initialize(ArrowPartType.Circumference, measurementPart);
            arrowPart.SetSpriteColor(_defaultColor, "Line");
            arrowPart.Position = position;

            if (measurementPart == MeasurementPart.Hip1Circumference)
                arrowPart.transform.Translate(0f, -0.4f, 0f);  //HERE!
            if (measurementPart == MeasurementPart.Hip5Circumference)
                arrowPart.transform.Translate(0f, -0.2f, 0f);

            if (measurementPart == MeasurementPart.LeftThighCircumference)
                arrowPart.transform.Translate(0.15f, 0f, 0f);
            if (measurementPart == MeasurementPart.RightThighCircumference)
                arrowPart.transform.Translate(-0.15f, 0f, 0f);
            if (measurementPart == MeasurementPart.LeftUpperArmCircumference)
                arrowPart.transform.Translate(0.15f, 0f, 0f);
            if (measurementPart == MeasurementPart.RightUpperArmCircumference)
                arrowPart.transform.Translate(-0.15f, 0f, 0f);
            _arrowPartList.Add(arrowPart);
        }

        void SetHip(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            switch (measurementPart)
            {
                case MeasurementPart.Hip2Circumference:
                case MeasurementPart.Hip3Circumference:
                case MeasurementPart.Hip4Circumference:
                    break;
                default:
                    return;
            }

            List<Vector3> positionList = new List<Vector3> { avatarModel.ParseVertexData(902) * bodyScale, avatarModel.ParseVertexData(6665) * bodyScale };

            float maxY = positionList.Select(n => n.y).Max();
            float minY = positionList.Select(n => n.y).Min();

            float difference = MathF.Abs(maxY - minY) / 4f;

            float y;

            Vector3 position = avatarModel.ParseVertexData(902) * bodyScale;

            switch (measurementPart)
            {
                case MeasurementPart.Hip2Circumference:
                    y = maxY - difference * 1f;
                    break;
                case MeasurementPart.Hip3Circumference:
                    y = maxY - difference * 2f;
                    break;
                case MeasurementPart.Hip4Circumference:
                    y = maxY - difference * 3f;
                    break;
                default:
                    y = 0;
                    break;
            }

            position.y = y;

            ArrowPart arrowPart = Instantiate(_arrowPartPrefab, _arrowPartTransform);
            arrowPart.Initialize(ArrowPartType.Circumference, measurementPart);
            arrowPart.SetSpriteColor(_defaultColor, "Line");
            arrowPart.Position = position;

            switch (measurementPart)
            {
                case MeasurementPart.Hip2Circumference:
                    arrowPart.transform.Translate(0f, -0.35f, 0f);
                    break;
                case MeasurementPart.Hip3Circumference:
                    arrowPart.transform.Translate(0f, -0.3f, 0f);
                    break;
                case MeasurementPart.Hip4Circumference:
                    arrowPart.transform.Translate(0f, -0.25f, 0f);
                    break;
            }
            
            _arrowPartList.Add(arrowPart);
        }

        void SetLine(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            switch (measurementPart)
            {
                case MeasurementPart.Hieght:
                    SetHieght(avatarModel, measurementPart, bodyScale);
                    break;
                case MeasurementPart.BackShoulderWidth:
                    SetBackShoulderWidth(avatarModel, measurementPart, bodyScale);
                    break;
                case MeasurementPart.InseamHeight:
                    SetInseamHeight(avatarModel, measurementPart, bodyScale);
                    break;
                case MeasurementPart.RightShoulder:
                    SetRightShoulder(avatarModel, measurementPart, bodyScale);
                    break;
                case MeasurementPart.LeftShoulder:
                    SetLeftShoulder(avatarModel, measurementPart, bodyScale);
                    break;
                case MeasurementPart.RightArmLength:
                    SetRightArmLength(avatarModel, measurementPart, bodyScale);
                    break;
                case MeasurementPart.LeftArmLength:
                    SetLeftArmLength(avatarModel, measurementPart, bodyScale);
                    break;
                case MeasurementPart.FirstLumbarHeight:
                    SetFirstLumbarHeight(avatarModel, measurementPart, bodyScale);
                    break;
                default:
                    return;
            }
        }

        void SetHieght(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            var hieghtPosition = GetHieghtPosition(avatarModel, bodyScale);

            ArrowPart arrowPart = SetLineRenderer(measurementPart, hieghtPosition.StartPosition, hieghtPosition.EndPosition, null, 2.0f);

            SetHieghtSideLine(measurementPart, arrowPart, hieghtPosition.StartPosition, "start", 2.0f);
            SetHieghtSideLine(measurementPart, arrowPart, hieghtPosition.EndPosition, "end", 2.0f);
        }

        public (Vector3 StartPosition, Vector3 EndPosition) GetHieghtPosition(IAvatarModel avatarModel, float bodyScale)
        {
            float maxY = avatarModel.ObjPoints.Select(n => n.y).Max();
            float minY = avatarModel.ObjPoints.Select(n => n.y).Min();

            Vector3 startPosition = new Vector3(0, maxY, 0) * bodyScale;
            Vector3 endPosition = new Vector3(0, minY, 0) * bodyScale;

            return (startPosition, endPosition);
        }

        void SetInseamHeight(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            Vector3 startPosition = avatarModel.ParseVertexData(890);
            startPosition.x = 0;
            startPosition.z = 0;

            float minY = avatarModel.ObjPoints.Select(n => n.y).Min();

            Vector3 endPosition = new Vector3(0, minY, 0) * bodyScale;

            startPosition = startPosition * bodyScale;

            SetLineRenderer(measurementPart, startPosition, endPosition, null, -0.15f);
        }

        void SetRightShoulder(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            Vector3 startPosition = avatarModel.ParseVertexData(597) * bodyScale;
            Vector3 endPosition = avatarModel.ParseVertexData(11201) * bodyScale;

            SetLineRenderer(measurementPart, startPosition, endPosition);
        }

        void SetLeftShoulder(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            Vector3 startPosition = avatarModel.ParseVertexData(1237) * bodyScale;
            Vector3 endPosition = avatarModel.ParseVertexData(19617) * bodyScale;

            SetLineRenderer(measurementPart, startPosition, endPosition);
        }

        void SetRightArmLength(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            Vector3 startPosition = avatarModel.ParseVertexData(11201) * bodyScale;
            Vector3 endPosition = avatarModel.ParseVertexData(24) * bodyScale;

            SetLineRenderer(measurementPart, startPosition, endPosition);
        }

        void SetLeftArmLength(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            Vector3 startPosition = avatarModel.ParseVertexData(19617) * bodyScale;
            Vector3 endPosition = avatarModel.ParseVertexData(1810) * bodyScale;

            SetLineRenderer(measurementPart, startPosition, endPosition);
        }

        void SetFirstLumbarHeight(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            Vector3 startPosition = avatarModel.ParseVertexData(904);
            startPosition.x = 0;
            startPosition.z = 0;

            float minY = avatarModel.ObjPoints.Select(n => n.y).Min();

            Vector3 endPosition = new Vector3(0, minY, 0) * bodyScale;

            startPosition = startPosition * bodyScale;

            SetLineRenderer(measurementPart, startPosition, endPosition, null, 0.15f);
        }

        void SetBackShoulderWidth(IAvatarModel avatarModel, MeasurementPart measurementPart, float bodyScale)
        {
            Vector3 startPosition = avatarModel.ParseVertexData(26093) * bodyScale;
            Vector3 rightPosition = avatarModel.ParseVertexData(11201) * bodyScale;
            Vector3 leftPosition = avatarModel.ParseVertexData(19617) * bodyScale;

            ArrowPart arrowPart = Instantiate(_arrowPartPrefab, _arrowPartTransform);
            arrowPart.Initialize(ArrowPartType.Line, measurementPart);

            _arrowPartList.Add(arrowPart);

            SetLineRenderer(measurementPart, startPosition, rightPosition, arrowPart);
            SetLineRenderer(measurementPart, startPosition, leftPosition, arrowPart);
        }

        ArrowPart SetLineRenderer(MeasurementPart measurementPart, Vector3 startPosition, Vector3 endPosition, ArrowPart defaltArrowPart = null, float offsetX = 0f)
        {
            startPosition.x += offsetX;
            endPosition.x += offsetX;

            Vector3[] points = new Vector3[] { startPosition, endPosition };

            ArrowPart arrowPart = null;

            if (defaltArrowPart == null)
            {
                arrowPart = Instantiate(_arrowPartPrefab, _arrowPartTransform);
                arrowPart.Initialize(ArrowPartType.Line, measurementPart);

                _arrowPartList.Add(arrowPart);
            }
            else
            {
                arrowPart = defaltArrowPart;
            }

            GameObject lineParent = new GameObject($"{measurementPart}");
            lineParent.transform.SetParent(arrowPart.transform);

            LineRenderer lineRenderer = lineParent.AddComponent<LineRenderer>();

            lineRenderer.startWidth = 0.050f;
            lineRenderer.endWidth = 0.050f;
            lineRenderer.positionCount = points.Length;
            lineRenderer.material = _defaultMaterial;
            lineRenderer.SetPositions(new Vector3[] { startPosition + MiddlePosition, endPosition + MiddlePosition });
            lineRenderer.gameObject.layer = LayerMask.NameToLayer("Line");

            arrowPart.LineRendererList.Add(lineRenderer);

            return arrowPart;

            //Vector3 center = (startPosition + endPosition) / 2;

            //foreach (Vector3 point in points)
            //{
                //ArrowHead arrowHead = Instantiate(_arrowHeadPrefab);
                //arrowHead.transform.SetParent(arrowPart.transform);
                //arrowHead.SpriteRenderer.color = _defaultColor;

                //Vector2 p1 = new Vector2(point.x, point.y);
                //Vector2 p2 = new Vector2(center.x, center.y);

                //float angle = GetAngle(p1, p2);

                //arrowHead.transform.Rotate(0, 0, angle);

                //float distance = Vector3.Distance(center, point);

                //float spriteSize = Mathf.Abs(arrowHead.transform.localScale.x * 0.32f / 4f);

                //Vector3 position = Vector3.Lerp(center, point, (distance - spriteSize) / distance);

                //arrowHead.transform.position = position;

                //arrowPart.ArrowHeadList.Add(arrowHead);
            //}
        }

        void SetHieghtSideLine(MeasurementPart measurementPart, ArrowPart arrowPart, Vector3 position, string name, float offsetX)
        {
            position.x += offsetX;
            position += MiddlePosition;

            GameObject lineParent = new GameObject($"{measurementPart}_{name}");
            lineParent.transform.SetParent(arrowPart.transform);

            LineRenderer lineRenderer = lineParent.AddComponent<LineRenderer>();

            Vector3 endPosition = position;
            endPosition.x -= 0.5f;

            lineRenderer.startWidth = 0.050f;
            lineRenderer.endWidth = 0.050f;
            lineRenderer.positionCount = 2;
            lineRenderer.material = _defaultMaterial;
            lineRenderer.SetPositions(new Vector3[] { position, endPosition });
            lineRenderer.gameObject.layer = LayerMask.NameToLayer("Line");

            arrowPart.LineRendererList.Add(lineRenderer);
        }

        //float GetAngle(Vector2 p1, Vector2 p2)
        //{
        //    float dx = p2.x - p1.x;
        //    float dy = p2.y - p1.y;
        //    return Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        //}
    }
}
