using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class LoginView : MonoBehaviour
    {
        [SerializeField] Button _loginButton;
        [SerializeField] Button _demoButton;
        [SerializeField] Button _passwordReregisterLinkButton;
        [SerializeField] Button _newIDRegisterLinkButton;
        [SerializeField] TMP_InputField _userIdInputField;
        [SerializeField] TMP_InputField _passwordInputField;
        [SerializeField] TextMeshProUGUI _errorMessageText;
        [SerializeField] Button _exchangePasswordButton;
        [SerializeField] Image _exchangePasswordImage;
        [SerializeField] Sprite _showSprite;
        [SerializeField] Sprite _hideSprite;

        public IObservable<Unit> OnLoginButtonClick => _onLoginButtonClick;
        Subject<Unit> _onLoginButtonClick = new Subject<Unit>();

        public IObservable<Unit> OnDemoButtonClick => _onDemoButtonClick;
        Subject<Unit> _onDemoButtonClick = new Subject<Unit>();

        public IObservable<Unit> OnPasswordReregisterLinkButtonClick => _onPasswordReregisterLinkButtonClick;
        Subject<Unit> _onPasswordReregisterLinkButtonClick = new Subject<Unit>();

        public IObservable<Unit> OnNewIDRegisterLinkButtonClick => _onNewIDRegisterLinkButtonClick;
        Subject<Unit> _onNewIDRegisterLinkButtonClick = new Subject<Unit>();

        public void Initialize()
        {
            _loginButton.OnClickAsObservable().Subscribe(_ => _onLoginButtonClick.OnNext(Unit.Default)).AddTo(this);
            _demoButton.OnClickAsObservable().Subscribe(_ => _onDemoButtonClick.OnNext(Unit.Default)).AddTo(this);
            _passwordReregisterLinkButton.OnClickAsObservable().Subscribe(_ => _onPasswordReregisterLinkButtonClick.OnNext(Unit.Default)).AddTo(this);
            _newIDRegisterLinkButton.OnClickAsObservable().Subscribe(_ => _onNewIDRegisterLinkButtonClick.OnNext(Unit.Default)).AddTo(this);

            _exchangePasswordButton.OnClickAsObservable().Subscribe(_ =>
            {
                switch (_passwordInputField.contentType)
                {
                    case TMP_InputField.ContentType.Password:
                        _passwordInputField.contentType = TMP_InputField.ContentType.Standard;
                        _passwordInputField.inputType = TMP_InputField.InputType.Standard;
                        _exchangePasswordImage.sprite = _hideSprite;
                        break;
                    case TMP_InputField.ContentType.Standard:
                        _passwordInputField.contentType = TMP_InputField.ContentType.Password;
                        _passwordInputField.inputType = TMP_InputField.InputType.Password;
                        _exchangePasswordImage.sprite = _showSprite;
                        break;
                }
                _passwordInputField.ForceLabelUpdate();

            }).AddTo(this);
        }

        public string ErrorMessage { set { _errorMessageText.text = value; } }

        public string UserID => _userIdInputField.text;

        public string Password => _passwordInputField.text;

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