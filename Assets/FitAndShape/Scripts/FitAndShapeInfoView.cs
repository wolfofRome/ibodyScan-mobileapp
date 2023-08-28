using TMPro;
using UnityEngine;

namespace FitAndShape
{
    public sealed class FitAndShapeInfoView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _dateText;
        [SerializeField] TextMeshProUGUI _placeText;

        public string DateText { set { _dateText.text = value; } }
        public string PlaceText { set { _placeText.text = value; } }

        public void Initialize()
        {
            DateText = string.Empty;
            PlaceText = string.Empty;

            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}