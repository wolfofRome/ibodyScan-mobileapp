using TMPro;
using UnityEngine;

namespace FitAndShape
{
    public sealed class NumberView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _text;

        public string Text
        {
            set
            {
                _text.text = value;
                gameObject.SetActive(!(value.Length == 0));
            }
        }
    }
}