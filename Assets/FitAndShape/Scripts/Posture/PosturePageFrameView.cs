using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using VContainer;

namespace FitAndShape
{
    public sealed class PosturePageFrameView : MonoBehaviour
    {
        [Inject] readonly PostureWarningPivotAsset _postureWarningPivotAsset;

        [SerializeField] PostureWarningItem _postureWarningItem;

        List<PostureWarningItem> _postureWarningItemList = new List<PostureWarningItem>();
        CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public Subject<(int Number, Result Result)> OnClickAsWarningButton { get; } = new Subject<(int, Result)>();

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void CreateWarning(Angle angle, Result[] results, IRenderTextureController renderTextureController, float postureWarningOffset)
        {
            _compositeDisposable.Clear();

            foreach (var item in _postureWarningItemList)
            {
                Destroy(item.gameObject);
            }

            _postureWarningItemList.Clear();

            int number = 0;

            foreach (var result in results)
            {
                // 異常個所の「!」を追加する
                PostureWarningItem item = AddWarning(angle, ++number, result, renderTextureController);

                item.Button.OnClickAsObservable().Subscribe(_ =>
                {
                    OnClickAsWarningButton.OnNext((item.Number, result));

                }).AddTo(_compositeDisposable);

                _postureWarningItemList.Add(item);
            }

            foreach (PostureWarningItem item in _postureWarningItemList)
            {
                var overlapList = _postureWarningItemList.Where(n => n.Number != item.Number && IsOverlap(item, n, postureWarningOffset));

                foreach (var adjustItem in overlapList)
                {
                    adjustItem.Position -= item.Position - adjustItem.Position;
                }
            }
        }

        bool IsOverlap(PostureWarningItem r1, PostureWarningItem r2, float width)
        {
            if (Mathf.Abs(r1.Position.x - r2.Position.x) < width && Mathf.Abs(r1.Position.y - r2.Position.y) < width)
            {
                return true;
            }

            return false;
        }

        public void ShowWarning(bool value)
        {
            foreach (var item in _postureWarningItemList)
            {
                item.gameObject.SetActive(value);
            }
        }

        private PostureWarningItem AddWarning(Angle angle, int number, Result result, IRenderTextureController renderTextureController)
        {
            PostureWarningItem item = Instantiate(_postureWarningItem, transform);
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            rectTransform.pivot = _postureWarningPivotAsset.GetEntity(angle).Pivot;

            item.transform.position = renderTextureController.TargetCamera.WorldToScreenPoint(result.MidScaledFocusPoint);

            item.SetNumber(number);
            return item;
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
