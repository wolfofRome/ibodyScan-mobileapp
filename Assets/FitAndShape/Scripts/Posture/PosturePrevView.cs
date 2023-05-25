using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class PosturePrevView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _summaryText;
        [SerializeField] TextMeshProUGUI _numberText;
        [SerializeField] Button _button;

        public IObservable<Unit> OnClick => _onClick;
        Subject<Unit> _onClick = new Subject<Unit>();

        public void Initialize()
        {
            _button.OnClickAsObservable().Subscribe(_ => _onClick.OnNext(Unit.Default)).AddTo(this);
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
