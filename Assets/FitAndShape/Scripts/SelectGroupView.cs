using UnityEngine;

namespace FitAndShape
{
    public class SelectGroupView : MonoBehaviour
    {
        [SerializeField] SelectView _selectViewAngle;
        [SerializeField] SelectView _selectViewColor;
        public SelectView SelectViewAngle => _selectViewAngle;
        public SelectView SelectViewColor => _selectViewColor;

        public void Initialize()
        {
            gameObject.SetActive(true);

            _selectViewAngle.Initialize();
            _selectViewColor.Initialize();
        }

        public void SetMeasurementPosition()
        {
            RectTransform rectTransform = _selectViewColor.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = new Vector2(250, -50);
        }
    }
}