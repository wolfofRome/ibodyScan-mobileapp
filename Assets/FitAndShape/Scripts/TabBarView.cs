using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

namespace FitAndShape
{
    public sealed class TabBarView : MonoBehaviour
    {
        [SerializeField] Button _myPageButton;
        [SerializeField] Button _scanButton;
        [SerializeField] Button _profileButton;
        [SerializeField] Button _optionButton;
        [SerializeField] SelectButtonGroupView _selectButtonGroupView;

        public IObservable<Unit> OnMyPageButtonClick => _onMyPageButtonClick;
        Subject<Unit> _onMyPageButtonClick = new Subject<Unit>();

        public IObservable<Unit> OnScanButtonClick => _onScanButtonClick;
        Subject<Unit> _onScanButtonClick = new Subject<Unit>();

        public IObservable<Unit> OnProfileButtonClick => _onProfileButtonClick;
        Subject<Unit> _onProfileButtonClick = new Subject<Unit>();

        public IObservable<Unit> OnOptionButtonClick => _onOptionButtonClick;
        Subject<Unit> _onOptionButtonClick = new Subject<Unit>();
                                                          
        public void Initialize()
        {
            _myPageButton.OnClickAsObservable().Subscribe(_ => _onMyPageButtonClick.OnNext(Unit.Default)).AddTo(this);
            _scanButton.OnClickAsObservable().Subscribe(_ => _onScanButtonClick.OnNext(Unit.Default)).AddTo(this);
            _profileButton.OnClickAsObservable().Subscribe(_ => _onProfileButtonClick.OnNext(Unit.Default)).AddTo(this);
            _optionButton.OnClickAsObservable().Subscribe(_ => _onOptionButtonClick.OnNext(Unit.Default)).AddTo(this);

            _selectButtonGroupView.Initialize();
        }

        public void ShowButton(SelectButtonType selectButtonType)
        {
            _selectButtonGroupView.ShowButton(selectButtonType);
        }

        public void EnabledButton(SelectButtonType selectButtonType, bool enabled)
        {
            _selectButtonGroupView.EnabledButton(selectButtonType, enabled);
        }

        public void Clear()
        {
            _selectButtonGroupView.Clear();
        }
    }
}