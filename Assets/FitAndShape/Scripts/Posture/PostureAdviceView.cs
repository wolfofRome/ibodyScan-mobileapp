using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class PostureAdviceView : MonoBehaviour
    {
        [SerializeField] PostureAdviceType _postureAdviceType;
        [SerializeField] TextMeshProUGUI _titleText;
        [SerializeField] TextMeshProUGUI _advicePointText;
        [SerializeField] Image _image;

        public PostureAdviceType PostureAdviceType => _postureAdviceType;

        public void SetInfo(string advicePoint, Sprite sprite)
        {
            _titleText.text = _postureAdviceType.GetName();
            _advicePointText.text = advicePoint;
            _image.sprite = sprite;
            _image.SetNativeSize();
        }
    }
}