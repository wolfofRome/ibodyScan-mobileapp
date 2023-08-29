using Amatib.ObjViewer.Domain;
using Amatib.ObjViewer.Infrastructure;
using Amatib.ObjViewer.Presentation.Loaders;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using VContainer;

namespace Amatib.ObjViewer.Presentation
{
    /// <summary>
    /// ModelView管理
    /// </summary>
    public sealed class ModelViewManager : MonoBehaviour
    {
        [Inject] readonly Parameter _parameter;

        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject modelRoot;
        [SerializeField] private GameObject meshModel;
        [SerializeField] private GameObject pointCloudModel;
        [SerializeField] private Vector3 modelScale;
        [SerializeField] private Texture2D meshTexture;
        [SerializeField] private CameraManager cameraManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private Material _clothingMaterial;
        [SerializeField] private ModelViewParameter _modelViewParameter;

        private PlayerInput _playerInput;
        private bool _isMove = false;
        private bool _isRotate = false;
        private WebSocketClient _webSocketClient = null;
        private MeshCollider _meshModelCollider;
        private CapsuleCollider _pointCloudModelCollider;
        private bool _isUnloadLoading = false;
        private TouchState _touchState0;
        private TouchState _touchState1;
        private Vector3 _center
        {
            get
            {
                Vector3 position = Vector3.zero;
                position.x = 0;
                position.y = uiManager.ToggleShow.isOn ? _pointCloudModelCollider.bounds.center.y : _meshModelCollider.bounds.center.y;
                position.z = 0;
                return position;
            }
        }

        private float GetZoom(float height)
        {
            return (float)(_modelViewParameter.UpperRatio - _modelViewParameter.LowerRatio) * (float)(height - _modelViewParameter.LowerHeight) / (float)(_modelViewParameter.UpperHeight - _modelViewParameter.LowerHeight) + _modelViewParameter.LowerRatio;
        }

        private void UpdateCameraHeight(float height)
        {
            Vector3 position = mainCamera.transform.position;
            position.y = height;
            mainCamera.transform.position = position;
        }

        private float GetCameraHeight(float height)
        {
            return (float)(_modelViewParameter.CameraUpperRatio - _modelViewParameter.CameraLowerRatio) * (float)(height - _modelViewParameter.LowerHeight) / (float)(_modelViewParameter.UpperHeight - _modelViewParameter.LowerHeight) + _modelViewParameter.CameraLowerRatio;
        }

        private async void Start()
        {
            try
            {
#if !UNITY_EDITOR
                Debug.unityLogger.logEnabled = false; // ログを無効化する          
#endif
                SceneManager.LoadScene("Loading", LoadSceneMode.Additive);

                uiManager.Language = _parameter.Language;
                await uiManager.SetIconImage(_parameter.Service, _parameter.Platform);

                var client = new ApiClient(_parameter.ApiHost, _parameter.MeasurementNumber, _parameter.Key, _parameter.Token);

                //MemoryStream memoryStream = await client.Download("scan_data_hires.obj");
                Debug.Log("[Start]: download scan_data.fbx start");
                MemoryStream memoryStream = await client.Download("scan_data.fbx");
                Debug.Log("[Start]: download scan_data.fbx end");

                string[] objLines = ObjLoader.LoadAsStream(memoryStream);

                Vector3 scale = Vector3.one;
                scale.x = scale.x * _modelViewParameter.ObjLoadScale * -1f;
                scale.y = scale.y * _modelViewParameter.ObjLoadScale;
                scale.z = scale.z * _modelViewParameter.ObjLoadScale;

                meshModel.GetComponent<MeshFilter>().mesh = ObjLoader.LoadAsMesh(objLines, scale);
                _meshModelCollider = meshModel.AddComponent<MeshCollider>();
                _meshModelCollider.convex = true;

                var heightParameter = new HeightParameter(meshModel.GetComponent<MeshFilter>().mesh);

                if (_parameter.IsShowPointCloud)
                {
                    pointCloudModel.GetComponent<MeshFilter>().mesh = PlyLoader.LoadAsMesh(await client.Download("scan_data.ply"), scale);
                    _pointCloudModelCollider = pointCloudModel.AddComponent<CapsuleCollider>();
                }

                if (_parameter.IsShowPreview)
                {
                    var position = new Vector3(_center.x * -1f, _center.y / -2f, 0f);
                    modelRoot.transform.position = position;
                    cameraManager.Zoom = _modelViewParameter.PreviewZoomValue;
                }

                pointCloudModel.SetActive(_parameter.IsShowPointCloud);
                meshModel.SetActive(!_parameter.IsShowPointCloud);

                if (_parameter.IsShowPreview)
                {
                    _webSocketClient = new WebSocketClient(_parameter.WebSocketUrl);

                    await _webSocketClient.DownloadAsync<WebSocketClient.TypedInitializedData>(CreateClothing);

                    _webSocketClient.DownloadAsync<WebSocketClient.TypedAdjustedData>(CreateClothing).Forget();
                }

                await SceneManager.UnloadSceneAsync("loading");

                _isUnloadLoading = true;

                float zoomValue = GetZoom(heightParameter.Height / _modelViewParameter.ObjLoadScale);

                float cameraHeight = GetCameraHeight(heightParameter.Height / _modelViewParameter.ObjLoadScale);

                UpdateCameraHeight(cameraHeight);

                await UniTask.WhenAll(OnMoveAsync(modelRoot.transform, Vector3.zero, _modelViewParameter.FrontPosition, _modelViewParameter.FrontZoomTime), OnZoomAsync(cameraManager.Zoom, zoomValue, _modelViewParameter.FrontZoomTime));

                uiManager.Active(_parameter.IsShowPointCloud, _parameter.IsShowPanel);

                // ローディング後に各種イベントを発火させる

                // 拡大ボタンクリック
                uiManager.ZoomInButton.OnClickAsObservable().Subscribe(_ =>
                {
                    Debug.Log("OnZoomIn");
                    cameraManager.OnZoom(_modelViewParameter.ZoomValue * -1f, _modelViewParameter.ZoomTime);

                }).AddTo(this);

                // 縮小ボタンクリック
                uiManager.ZoomOutButton.OnClickAsObservable().Subscribe(_ =>
                {
                    Debug.Log("OnZoomOut");
                    cameraManager.OnZoom(_modelViewParameter.ZoomValue, _modelViewParameter.ZoomTime);

                }).AddTo(this);

                // ToggleShowの値が変化した時発火
                uiManager.ToggleShow.OnValueChangedAsObservable().Where(_ => _parameter.IsShowPointCloud).Subscribe(n =>
                {
                    pointCloudModel.SetActive(n);
                    meshModel.SetActive(!n);

                }).AddTo(this);

                _playerInput.actions["Rotate"].performed += OnRotate;
                _playerInput.actions["Zoom"].performed += OnZoom;
                _playerInput.actions["MoveViewPoint"].performed += OnMoveViewPoint;
                _playerInput.actions["MoveViewPointTouch0"].performed += OnMoveViewPointTouch0;
                _playerInput.actions["MoveViewPointTouch1"].performed += OnMoveViewPointTouch1;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                SceneManager.LoadScene("LoadingError");
            }
            finally
            {
                if (!_isUnloadLoading) await SceneManager.UnloadSceneAsync("loading");
            }
        }

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            EnhancedTouchSupport.Enable();
        }

        private void OnDestroy()
        {
            _playerInput.actions["Rotate"].performed -= OnRotate;
            _playerInput.actions["Zoom"].performed -= OnZoom;
            _playerInput.actions["MoveViewPoint"].performed -= OnMoveViewPoint;
            _playerInput.actions["MoveViewPointTouch0"].performed -= OnMoveViewPointTouch0;
            _playerInput.actions["MoveViewPointTouch1"].performed -= OnMoveViewPointTouch1;
            _webSocketClient?.Close();
        }

        /// <summary>
        /// InputSystemのRotate
        /// </summary>
        /// <param name="context"></param>
        private void OnRotate(InputAction.CallbackContext context)
        {
            Vector3 currentPosition;

#if UNITY_IPHONE || UNITY_ANDROID
            // タッチ操作でピンチ操作中は処理しない
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count >= 2) return;
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 0) return;
            currentPosition = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
#else
            currentPosition = Input.mousePosition;
#endif
            if (IsPointerOverUIObject(currentPosition)) return;

            OnRotate(context.ReadValue<Vector2>());
        }

        /// <summary>
        /// InputSystemのMoveViewPoint
        /// </summary>
        /// <param name="context"></param>
        private void OnMoveViewPoint(InputAction.CallbackContext context)
        {
            var delta = context.ReadValue<Vector2>() * _modelViewParameter.MoveCoefficient;

            Debug.Log($"OnMoveViewPoint delta: {delta.ToString()}, delta3: {((Vector3)delta).ToString()}");

            var position = modelRoot.transform.position;
            var to = new Vector3(position.x - delta.x, position.y + delta.y, position.z);

            OnMove(modelRoot.transform, to);
        }

        /// <summary>
        /// InputSystemのMoveViewPointTouch0
        /// </summary>
        /// <param name="context"></param>
        private void OnMoveViewPointTouch0(InputAction.CallbackContext context)
        {
            _touchState0 = context.ReadValue<TouchState>();
            Debug.Log($"OnMoveViewPointTouch0 _touchState0.delta: {_touchState0.delta.ToString()}");

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
            _touchState1 = context.ReadValue<TouchState>();
            Debug.Log($"OnMoveViewPointTouch1 _touchState0.delta: {_touchState1.delta.ToString()}");

            OnMoveViewPointTouch();
        }

        private void OnMoveViewPointTouch()
        {
            // ２本指が移動していなかれば操作なしと判断
            if (!_touchState0.isInProgress || !_touchState1.isInProgress) return;

            if (Vector2.Angle(_touchState0.delta, _touchState1.delta) > 90)
            {
                // ピンチイン/アウト
                var previousDistance = Vector2.Distance(_touchState0.position - _touchState0.delta, _touchState1.position - _touchState1.delta);
                var distance = Vector2.Distance(_touchState0.position, _touchState1.position);

                Debug.Log($"pinch previousDistance:{previousDistance}, distance:{distance}");

                cameraManager.Zoom -= (distance - previousDistance) * _modelViewParameter.PinchOutInRate;
            }
            else
            {
                // マルチワイプ

                // 移動量（スクリーン座標）
                var delta0 = _touchState0.delta;
                var delta1 = _touchState1.delta;

                // 中点の変化量を求める
                var delta = (delta0 + delta1) / 2 * _modelViewParameter.MoveCoefficient;

                Debug.Log($"OnMoveViewPointTouch delta: {delta.ToString()}, delta3: {((Vector3)delta).ToString()}");

                var position = modelRoot.transform.position;
                var to = new Vector3(position.x - delta.x, position.y + delta.y, position.z);

                OnMove(modelRoot.transform, to);
            }
        }

        /// <summary>
        /// InputSystemのZoom
        /// </summary>
        /// <param name="context"></param>
        private void OnZoom(InputAction.CallbackContext context)
        {
            var scroll = context.ReadValue<float>();

            Debug.Log($"onZoom scroll:{scroll}");

            cameraManager.Zoom -= scroll / 120;
        }

        /// <summary>
        /// 即座に移動する
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="toPosition"></param>
        private void OnMove(Transform transform, Vector3 toPosition)
        {
            transform.position = toPosition;
        }

        /// <summary>
        /// 回転する
        /// </summary>
        /// <param name="delta"></param>
        private void OnRotate(Vector2 delta)
        {
            Debug.Log($"onRotate delta: {delta.ToString()}, delta3: {((Vector3)delta).ToString()}");

            modelRoot.transform.RotateAround(_center, modelRoot.transform.up, -delta.x);
            modelRoot.transform.RotateAround(_center, Vector3.right, -delta.y);
        }

        /// <summary>
        /// 時間をかけて移動する
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="fromPosition"></param>
        /// <param name="toPosition"></param>
        /// <param name="moveTime"></param>
        private async UniTask OnMoveAsync(Transform transform, Vector3 fromPosition, Vector3 toPosition, float moveTime)
        {
            if (_isMove) return;

            _isMove = true;

            float timer = 0f;

            while (timer < moveTime)
            {
                timer += Time.deltaTime;

                transform.position = Vector3.Lerp(fromPosition, toPosition, timer / moveTime);

                await UniTask.Yield();
            }

            transform.position = toPosition;

            _isMove = false;
        }

        /// <summary>
        /// 時間をかけて回転する
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="rotateTime"></param>
        private async UniTask OnRotateAsync(Transform transform, float angle, float rotateTime)
        {
            if (_isRotate) return;

            _isRotate = true;

            float timer = 0f;

            while (timer < rotateTime)
            {
                timer += Time.deltaTime;

                transform.RotateAround(_center, Vector3.up, angle / rotateTime * Time.deltaTime);

                await UniTask.Yield();
            }

            _isRotate = false;
        }

        /// <summary>
        /// 時間をかけて拡大する
        /// </summary>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        /// <param name="zoomTime"></param>
        /// <returns></returns>
        private async UniTask OnZoomAsync(float fromValue, float toValue, float zoomTime)
        {
            float timer = 0f;

            cameraManager.Zoom = fromValue;

            while (timer < zoomTime)
            {
                timer += Time.deltaTime;

                cameraManager.Zoom = Mathf.Lerp(fromValue, toValue, timer / zoomTime);

                await UniTask.Yield();
            }

            cameraManager.Zoom = toValue;
        }

        /// <summary>

        /// 角度を取得する
        /// </summary>
        /// <param name="hitInfo"></param>
        /// <returns></returns>
        private float GetAngle(RaycastHit hitInfo)
        {
            var fromNormal = new Vector3(hitInfo.normal.x * 100f, _center.y, hitInfo.normal.z * 100f);

            var toNormal = Camera.main.transform.position - _center;
            toNormal.y = _center.y;

            var axis = Vector3.Cross(fromNormal, toNormal);
            var angle = Vector3.Angle(fromNormal, toNormal) * (axis.y < 0 ? -1 : 1);

            return angle;
        }

        /// <summary>

        /// 衣類を作成する
        /// </summary>
        /// <param name="previewData"></param>
        private void CreateClothing(Dictionary<string, byte[]> previewData)
        {
            foreach (var item in previewData)
            {
                using Stream stream = new MemoryStream(item.Value);

                string[] objLines = ObjLoader.LoadAsStream(stream);

                var findGameObject = GameObject.Find(item.Key);

                if (findGameObject != null)
                {
                    Destroy(findGameObject);
                }

                var gameObject = new GameObject(item.Key);

                gameObject.transform.parent = modelRoot.transform;
                gameObject.transform.localScale = modelScale;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;

                var meshFilter = gameObject.AddComponent<MeshFilter>();
                var meshRenderer = gameObject.AddComponent<MeshRenderer>();

                meshFilter.mesh = ObjLoader.LoadAsMesh(objLines, Vector3.one);
                meshRenderer.sharedMaterial = _clothingMaterial;
            }
        }

        /// <summary>
        /// タッチorクリックした位置にUIがあるか判定する
        /// </summary>
        /// <param name="pointerPosition"></param>
        /// <returns></returns>
        private bool IsPointerOverUIObject(Vector2 pointerPosition)
        {
            List<RaycastResult> resultsBuffer = new List<RaycastResult>();

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = pointerPosition;

            EventSystem.current.RaycastAll(eventDataCurrentPosition, resultsBuffer);

            return resultsBuffer.Count > 0;
        }
    }
}
