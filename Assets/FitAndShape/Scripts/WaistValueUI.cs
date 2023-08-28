using TMPro;
using UnityEngine;

namespace FitAndShape
{
    public sealed class WaistValueUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _dateText;
        [SerializeField] TextMeshProUGUI _valueText;

        public string DateText { set { _dateText.text = value; } }
        public string ValueText { set { _valueText.text = value; } }
    }
}