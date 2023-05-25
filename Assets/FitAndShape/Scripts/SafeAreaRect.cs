using UnityEngine;

namespace FitAndShape
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public sealed class SafeAreaRect : MonoBehaviour
    {
        [System.Flags]
        public enum Edge
        {
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8,
        }

        [SerializeField]
        private Edge controlEdges = (Edge)~0;

        public Edge ControlEdges => controlEdges;

        private Rect _lastSafeArea;
        private Vector2Int _lastResolution;
        private Edge _lastControlEgdes;

#if UNITY_EDITOR
        private DrivenRectTransformTracker _drivenRectTransformTracker = new DrivenRectTransformTracker();
#endif

        private void Update()
        {
            Apply();
        }

        private void OnEnable()
        {
            Apply(force: true);
        }

#if UNITY_EDITOR
        private void OnDisable()
        {
            _drivenRectTransformTracker.Clear();
        }
#endif

        private void Apply(bool force = false)
        {
            var rectTransform = (RectTransform)transform;
            var safeArea = Screen.safeArea;
            var resolution = new Vector2Int(Screen.width, Screen.height);
            if (resolution.x == 0 || resolution.y == 0)
            {
                return;
            }
            if (!force)
            {
                if (rectTransform.anchorMax == Vector2.zero)
                {
                    // Do apply.
                    // ※Undoすると0になるので再適用させる
                }
                else if (_lastSafeArea == safeArea && _lastResolution == resolution && _lastControlEgdes == controlEdges)
                {
                    return;
                }
            }
            this._lastSafeArea = safeArea;
            this._lastResolution = resolution;
            this._lastControlEgdes = controlEdges;

#if UNITY_EDITOR
            _drivenRectTransformTracker.Clear();
            _drivenRectTransformTracker.Add(
                this,
                rectTransform,
                DrivenTransformProperties.AnchoredPosition
                | DrivenTransformProperties.SizeDelta
                | DrivenTransformProperties.AnchorMin
                | DrivenTransformProperties.AnchorMax
            );
#endif

            var normalizedMin = new Vector2(safeArea.xMin / resolution.x, safeArea.yMin / resolution.y);
            var normalizedMax = new Vector2(safeArea.xMax / resolution.x, safeArea.yMax / resolution.y);
            if ((controlEdges & Edge.Left) == 0)
            {
                normalizedMin.x = 0;
            }
            if ((controlEdges & Edge.Right) == 0)
            {
                normalizedMax.x = 1;
            }
            if ((controlEdges & Edge.Top) == 0)
            {
                normalizedMax.y = 1;
            }
            if ((controlEdges & Edge.Bottom) == 0)
            {
                normalizedMin.y = 0;
            }

            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchorMin = normalizedMin;
            rectTransform.anchorMax = normalizedMax;
        }
    }
}