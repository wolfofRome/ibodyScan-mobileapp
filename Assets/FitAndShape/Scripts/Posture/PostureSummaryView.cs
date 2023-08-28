using Amatib.ObjViewer.Domain;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

namespace FitAndShape
{
    public sealed class PostureSummaryView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] RectTransform _postureSummaryListRectTransform;
        [SerializeField] PostureSummaryItem _postureSummaryItemPrefab;
        [SerializeField] FitAndShapeInfoView _fitAndShapeInfoView;

        public int Number { set { _text.text = $"{value}箇所の「ゆがみ」症状が検出されました"; } }
        public Subject<(int Number, Result Result)> OnClick { get; } = new Subject<(int, Result)>();

        List<PostureSummaryItem> _postureSummaryItemList = new List<PostureSummaryItem>();

        CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetParameter(Parameter parameter)
        {
            _fitAndShapeInfoView.DateText = parameter.CreatedAt;
            _fitAndShapeInfoView.PlaceText = parameter.Place;
        }

        public void SetResult(Result[] results)
        {
            _compositeDisposable.Clear();

            Number = results.Length;

            foreach (var item in _postureSummaryItemList)
            {
                Destroy(item.gameObject);
            }

            _postureSummaryItemList.Clear();

            int number = 0;

            foreach (var result in results)
            {
                PostureSummaryItem postureSummaryItem = Instantiate(_postureSummaryItemPrefab, _postureSummaryListRectTransform);

                number++;

                postureSummaryItem.Number = number;
                postureSummaryItem.Text = result.Summary;
                postureSummaryItem.Button.OnClickAsObservable().Subscribe(_ => OnClick.OnNext((postureSummaryItem.Number, result))).AddTo(_compositeDisposable);

                _postureSummaryItemList.Add(postureSummaryItem);
            }
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}