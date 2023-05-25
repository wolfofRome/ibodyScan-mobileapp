using Amatib;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using VContainer;

namespace FitAndShape
{

    public sealed class FitAndShapeView : MonoBehaviour
    {
        [Inject] readonly ModelView _modelView;
        [Inject] readonly ArrowView _arrowView;
        [Inject] readonly ModelViewParameter _modelViewParameter;

        [SerializeField] Angle _defaultAngle;
        [SerializeField] Camera _mainCamera;
        [SerializeField] PlayerInput _playerInput;

        public Angle DefaultAngle { get { return _defaultAngle; } set { _defaultAngle = value; } }
        public bool CameraVisible { get { return _mainCamera.gameObject.activeSelf; } set { _mainCamera.gameObject.SetActive(value); } }

        private TouchState _touchState0;
        private TouchState _touchState1;

        public IObservable<Unit> OnPlayerInput => _onPlayerInput;
        Subject<Unit> _onPlayerInput = new Subject<Unit>();

        bool _isPlayerInput = false;

        public void SetPlayerInput()
        {
            if (_isPlayerInput)
            {
                return;
            }

            _isPlayerInput = true;

            EnhancedTouchSupport.Enable();

            _playerInput.actions["Rotate"].performed += OnRotate;
            _playerInput.actions["Zoom"].performed += OnZoom;
            _playerInput.actions["MoveViewPoint"].performed += OnMoveViewPoint;
            _playerInput.actions["MoveViewPointTouch0"].performed += OnMoveViewPointTouch0;
            _playerInput.actions["MoveViewPointTouch1"].performed += OnMoveViewPointTouch1;
        }

        public void DisabledPlayerInput()
        {
            _isPlayerInput = false;

            _playerInput.actions["Rotate"].performed -= OnRotate;
            _playerInput.actions["Zoom"].performed -= OnZoom;
            _playerInput.actions["MoveViewPoint"].performed -= OnMoveViewPoint;
            _playerInput.actions["MoveViewPointTouch0"].performed -= OnMoveViewPointTouch0;
            _playerInput.actions["MoveViewPointTouch1"].performed -= OnMoveViewPointTouch1;
        }

        void OnRotate(InputAction.CallbackContext context)
        {
            Vector3 currentPosition;

#if UNITY_EDITOR || UNITY_WEBGL
            currentPosition = Input.mousePosition;
#elif UNITY_IPHONE || UNITY_ANDROID
            // タッチ操作でピンチ操作中は処理しない
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count >= 2) return;
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 0) return;
            currentPosition = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
#endif
            if (IsPointerOverUIObject(currentPosition)) return;

            _onPlayerInput.OnNext(Unit.Default);

            OnRotate(context.ReadValue<Vector2>());
        }

        /// <summary>
        /// InputSystemのZoom
        /// </summary>
        /// <param name="context"></param>
        void OnZoom(InputAction.CallbackContext context)
        {
#if UNITY_EDITOR || UNITY_WEBGL
            Vector3 currentPosition = Input.mousePosition;

            if (IsPointerOverUIObject(currentPosition)) return;
#endif

            _onPlayerInput.OnNext(Unit.Default);

            var scroll = context.ReadValue<float>();

            _arrowView.CameraManager.Zoom -= scroll / 120;
        }

        /// <summary>
        /// InputSystemのMoveViewPoint
        /// </summary>
        /// <param name="context"></param>
        void OnMoveViewPoint(InputAction.CallbackContext context)
        {
            Vector3 currentPosition;

#if UNITY_EDITOR || UNITY_WEBGL
            currentPosition = Input.mousePosition;
#elif UNITY_IPHONE || UNITY_ANDROID
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 0) return;
            currentPosition = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
#endif
            if (IsPointerOverUIObject(currentPosition)) return;

            _onPlayerInput.OnNext(Unit.Default);

            var delta = context.ReadValue<Vector2>() * _modelViewParameter.MoveCoefficient;

            var position = _modelView.Position;
            var to = new Vector3(position.x - delta.x, position.y + delta.y, position.z);

            OnMove(_modelView.transform, to);
        }

        /// <summary>
        /// InputSystemのMoveViewPointTouch0
        /// </summary>
        /// <param name="context"></param>
        void OnMoveViewPointTouch0(InputAction.CallbackContext context)
        {
            Vector3 currentPosition;

#if UNITY_EDITOR || UNITY_WEBGL
            currentPosition = Input.mousePosition;
#elif UNITY_IPHONE || UNITY_ANDROID
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 0) return;
            currentPosition = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
#endif
            if (IsPointerOverUIObject(currentPosition)) return;

            _onPlayerInput.OnNext(Unit.Default);

            _touchState0 = context.ReadValue<TouchState>();

            if (_touchState0.isInProgress && !_touchState1.isInProgress)
            {
                OnRotate(_touchState0.delta);
                return;
            }

            OnMoveViewPointTouch();
        }

        /// <summary>
        /// InputSystemのMoveViewPointTouch1
        /// </summary>
        /// <param name="context"></param>
        private void OnMoveViewPointTouch1(InputAction.CallbackContext context)
        {
            Vector3 currentPosition;

#if UNITY_EDITOR || UNITY_WEBGL
            currentPosition = Input.mousePosition;
#elif UNITY_IPHONE || UNITY_ANDROID
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 0) return;
            currentPosition = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
#endif
            if (IsPointerOverUIObject(currentPosition)) return;

            _onPlayerInput.OnNext(Unit.Default);

            _touchState1 = context.ReadValue<TouchState>();

            OnMoveViewPointTouch();
        }

        /// <summary>
        /// 回転する
        /// </summary>
        /// <param name="delta"></param>
        void OnRotate(Vector2 delta)
        {
            _modelView.transform.RotateAround(_modelView.Center, _modelView.transform.up, -delta.x);
            _modelView.transform.RotateAround(_modelView.Center, Vector3.right, -delta.y);
        }

        /// <summary>
        /// 即座に移動する
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="toPosition"></param>
        void OnMove(Transform transform, Vector3 toPosition)
        {
            transform.position = toPosition;
        }

        void OnMoveViewPointTouch()
        {
            // ２本指が移動していなかれば操作なしと判断
            if (!_touchState0.isInProgress || !_touchState1.isInProgress) return;

            if (Vector2.Angle(_touchState0.delta, _touchState1.delta) > 90)
            {
                // ピンチイン/アウト
                var previousDistance = Vector2.Distance(_touchState0.position - _touchState0.delta, _touchState1.position - _touchState1.delta);
                var distance = Vector2.Distance(_touchState0.position, _touchState1.position);

                _arrowView.CameraManager.Zoom -= (distance - previousDistance) * _modelViewParameter.PinchOutInRate;
            }
            else
            {
                // マルチワイプ

                // 移動量（スクリーン座標）
                var delta0 = _touchState0.delta;
                var delta1 = _touchState1.delta;

                // 中点の変化量を求める
                var delta = (delta0 + delta1) / 2 * _modelViewParameter.MoveCoefficient;

                var position = _modelView.Position;
                var to = new Vector3(position.x - delta.x, position.y + delta.y, position.z);

                OnMove(_modelView.transform, to);
            }
        }

        /// <summary>
        /// タッチorクリックした位置にUIがあるか判定する
        /// </summary>
        /// <param name="pointerPosition"></param>
        /// <returns></returns>
        bool IsPointerOverUIObject(Vector2 pointerPosition)
        {
            List<RaycastResult> resultsBuffer = new List<RaycastResult>();

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = pointerPosition;

            EventSystem.current.RaycastAll(eventDataCurrentPosition, resultsBuffer);

            return resultsBuffer.Count > 0;
        }

        void OnDestroy()
        {
            DisabledPlayerInput();
        }
    }
}