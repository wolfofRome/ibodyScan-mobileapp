using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace FitAndShape
{
    public sealed class OptionView : MonoBehaviour
    {
        [SerializeField] OptionItemView _optionItemViewLogin;
        [SerializeField] OptionItemView _optionItemViewBrank1;
        [SerializeField] OptionItemView _optionItemViewInfo;
        [SerializeField] OptionItemView _optionItemViewBrank2;
        [SerializeField] OptionItemView _optionItemViewLogout;
        [SerializeField] TextMeshProUGUI _versionText;

        public IObservable<Unit> OnLoginClick => _onLoginClick;
        Subject<Unit> _onLoginClick = new Subject<Unit>();

        public IObservable<Unit> OnInfoClick => _onInfoClick;
        Subject<Unit> _onInfoClick = new Subject<Unit>();

        public IObservable<Unit> OnLogoutClick => _onLogoutClick;
        Subject<Unit> _onLogoutClick = new Subject<Unit>();

        public void Initialize()
        {
            _optionItemViewLogin.Initialize();
            _optionItemViewInfo.Initialize();
            _optionItemViewLogout.Initialize();

            _versionText.text = Application.version;

            _optionItemViewLogin.OnClick.Subscribe(_ => _onLoginClick.OnNext(Unit.Default)).AddTo(this);
            _optionItemViewInfo.OnClick.Subscribe(_ => _onInfoClick.OnNext(Unit.Default)).AddTo(this);
            _optionItemViewLogout.OnClick.Subscribe(_ => _onLogoutClick.OnNext(Unit.Default)).AddTo(this);
        }

        public void Show(bool isDemo)
        {
            _optionItemViewLogin.Hide();
            _optionItemViewBrank1.Hide();
            _optionItemViewInfo.Hide();
            _optionItemViewBrank2.Hide();
            _optionItemViewLogout.Hide();

            if (isDemo)
            {
                _optionItemViewLogin.Show();
                _optionItemViewBrank1.Show();
                _optionItemViewInfo.Show();
            }
            else
            {
                _optionItemViewInfo.Show();
                _optionItemViewBrank2.Show();
                _optionItemViewLogout.Show();
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}