using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class PostureWarningItem : MonoBehaviour
    {
        [SerializeField] Button _button;
        [SerializeField] TextMeshProUGUI _numbetText;
        [SerializeField] RectTransform _rectTransform;

        int _number;

        public int Number => _number;

        public Button Button => _button;

        public void SetNumber(int number)
        {
            _number = number;
            _numbetText.text = $"{_number}";
        }

        public Vector2 Position { get { return _rectTransform.anchoredPosition; } set { _rectTransform.anchoredPosition = value; } }
    }
}
