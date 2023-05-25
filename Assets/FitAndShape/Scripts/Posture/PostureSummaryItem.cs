using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class PostureSummaryItem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _textNumber;
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] Button _button;

        int _number;

        public int Number 
        { 
            get { return _number;  } 
            set
            {
                _number = value;
                _textNumber.text = $"{_number}"; 
            }
        }
        public string Text { set { _text.text = value; } }
        public Button Button => _button;
    }
}