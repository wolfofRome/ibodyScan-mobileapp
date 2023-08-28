using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class ImageTextHolder : MonoBehaviour
    {
        [SerializeField] Image _image = default;
        [SerializeField] TextMeshProUGUI _text = default;

        public Image image => _image;
        public TextMeshProUGUI text => _text;
    }
}