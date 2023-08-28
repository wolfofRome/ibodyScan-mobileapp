using Amatib.ObjViewer.Domain;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class PostureDetailView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _summaryText;
        [SerializeField] TextMeshProUGUI _descriptionText;
        [SerializeField] TextMeshProUGUI _commentText;
        [SerializeField] TextMeshProUGUI _numberText;
        [SerializeField] Button _button;
        [SerializeField] FitAndShapeInfoView _fitAndShapeInfoView;
        [SerializeField] List<PostureAdviceView> _postureAdviceViewList;

        public Subject<Unit> OnPreveButtonClick { get; } = new Subject<Unit>();

        bool _isInitialize = false;

        public int Number { set { _numberText.text = $"{value}"; } }
        public string Summary { set { _summaryText.text = value; } }
        public string Description { set { _descriptionText.text = value; } }
        public string Comment { set { _commentText.text = value; } }

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

        public void Initialize()
        {
            if (_isInitialize) return;

            _isInitialize = true;

            if (_button != null)
            {
                _button.OnClickAsObservable().Subscribe(n => OnPreveButtonClick.OnNext(n)).AddTo(this);
            }

            gameObject.SetActive(false);
        }

        public void SetResult(int number, Result result, string comment, PostureAdviceEntity postureAdviceEntity)
        {
            if (_numberText != null)
            {
                Number = number;
            }

            if (_summaryText != null)
            {
                Summary = result.Summary;
            }
            
            Description = result.Description;

            Comment = comment;

            PostureAdviceView partPostureAdviceView = _postureAdviceViewList.Where(n => n.PostureAdviceType == PostureAdviceType.Part).First();
            partPostureAdviceView.SetInfo(postureAdviceEntity.PostureAdvicePoint.GetName(), postureAdviceEntity.PartSprite);

            PostureAdviceView treatmentPostureAdviceView = _postureAdviceViewList.Where(n => n.PostureAdviceType == PostureAdviceType.Treatment).First();
            treatmentPostureAdviceView.SetInfo(postureAdviceEntity.PostureAdvicePoint.GetName(), postureAdviceEntity.TreatmentSprite);
        }
    }
}