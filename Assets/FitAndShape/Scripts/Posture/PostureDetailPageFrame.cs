using System.Collections.Generic;
using UnityEngine;

namespace FitAndShape
{
    public sealed class PostureDetailPageFrame : MonoBehaviour
    {
        [SerializeField] LineRenderer _prefabLineRenderer = default;
        [SerializeField] Color _baseLineColor = new Color(0f, 0.796f, 1f);
        [SerializeField] Color _measurementLineColor = new Color(1f, 0.1607f, 0.1607f);
        [SerializeField] float _lineWidth;

        List<LineRenderer> _baseLineRendererList = new List<LineRenderer>();
        List<LineRenderer> _measurementLineRendererList = new List<LineRenderer>();

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void DrawLine(Result result)
        {
            DrawBaseLine(result);

            DrawMeasurementLine(result);
        }

        /// <summary>
        /// 基準となるラインを描画
        /// </summary>
        /// <param name="result"></param>
        void DrawBaseLine(Result result)
        {
            var basePoints = result.ScaledBaseLinePoints;

            if (basePoints.Count <= 0) return;

            foreach (var linPoints in basePoints)
            {
                var renderer = Instantiate(_prefabLineRenderer);
                renderer.transform.SetParent(transform);
                renderer.startColor = _baseLineColor;
                renderer.endColor = _baseLineColor;
                renderer.positionCount = linPoints.Length;
                renderer.startWidth = _lineWidth;
                renderer.endWidth = _lineWidth;
                renderer.SetPositions(linPoints);
                renderer.sortingOrder = 0;

                _baseLineRendererList.Add(renderer);
            }
        }

        /// <summary>
        /// 測定対象のラインを描画
        /// </summary>
        /// <param name="result"></param>
        public void DrawMeasurementLine(Result result)
        {
            var measurementPoints = result.ScaledMeasurementLinePoints;

            if (measurementPoints.Count <= 0) return;

            foreach (var linPoints in measurementPoints)
            {
                var renderer = Instantiate(_prefabLineRenderer);
                renderer.transform.SetParent(transform);
                renderer.startColor = _measurementLineColor;
                renderer.endColor = _measurementLineColor;
                renderer.positionCount = linPoints.Length;
                renderer.startWidth = _lineWidth;
                renderer.endWidth = _lineWidth;
                renderer.SetPositions(linPoints);
                renderer.sortingOrder = 1;

                _measurementLineRendererList.Add(renderer);
            }
        }

        public void ClearLine()
        {
            ClearLine(_baseLineRendererList);
            ClearLine(_measurementLineRendererList);
        }

        void ClearLine(List<LineRenderer> lineRendererList)
        {
            foreach (var lineRenderer in lineRendererList)
            {
                Destroy(lineRenderer.gameObject);
            }

            lineRendererList.Clear();
        }
    }
}