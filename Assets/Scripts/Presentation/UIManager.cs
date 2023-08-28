using System;
using Amatib.ObjViewer.Domain;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Amatib.ObjViewer.Presentation
{
    public sealed class UIManager : MonoBehaviour
    {
        // 言語設定用
        [SerializeField] private TextMeshProUGUI showPointCloudLabel;
        [SerializeField] private TextMeshProUGUI hidePointCloudLabel;
        [SerializeField] private TextMeshProUGUI rotateLabel;
        [SerializeField] private TextMeshProUGUI moveLabel;
        [SerializeField] private TextMeshProUGUI zoomLabel;
        
        // アイコン設定用
        [SerializeField] private Image rotateImage;
        [SerializeField] private Image moveImage;
        [SerializeField] private Image zoomImage;
        
        // 操作有効無効用
        [SerializeField] private GameObject pointCloudPanel;
        [SerializeField] private GameObject zoomPanel;
        [SerializeField] private GameObject footerPanel;

        [SerializeField] private Color autoTailorTextColor;
        [SerializeField] private Color autoTailorFooterColor;
        [SerializeField] private Image footerImage;

        [SerializeField] private Button _zoomInButton;
        [SerializeField] private Button _zoomOutButton;
        [SerializeField] private Toggle _toggleShow;

        public Button ZoomInButton => _zoomInButton;
        public Button ZoomOutButton => _zoomOutButton;
        public Toggle ToggleShow => _toggleShow;

        private Language _language;

        public Language Language
        {
            get => _language;
            set
            {
                switch (value)
                {
                    case Language.English:
                        showPointCloudLabel.text = "color";
                        hidePointCloudLabel.text = "monochrome";

                        rotateLabel.text = "rotate";
                        moveLabel.text = "move";
                        zoomLabel.text = "zoom";
                        break;
                    case Language.Japanese:
                        showPointCloudLabel.text = "カラー";
                        hidePointCloudLabel.text = "モノクロ";

                        rotateLabel.text = "回転";
                        moveLabel.text = "視点移動";
                        zoomLabel.text = "拡大縮小";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                _language = value;
            }
        }

        public async UniTask SetIconImage(Service service, Platform platform)
        {
            var platformLower = platform.ToString().ToLower();
            rotateImage.sprite = await Addressables.LoadAssetAsync<Sprite>($"Assets/Icons/{service}/icon_rotate_{platformLower}.png");
            moveImage.sprite = await Addressables.LoadAssetAsync<Sprite>($"Assets/Icons/{service}/icon_move_{platformLower}.png");
            zoomImage.sprite = await Addressables.LoadAssetAsync<Sprite>($"Assets/Icons/{service}/icon_zoom_{platformLower}.png");

            if (service == Service.AutoTailor)
            {
                rotateLabel.color = autoTailorTextColor;
                moveLabel.color = autoTailorTextColor;
                zoomLabel.color = autoTailorTextColor;
                footerImage.color = autoTailorFooterColor;
            }
        }

        public void Active(bool isShowPointCloud, bool isShowPanel)
        {
            footerPanel.SetActive(isShowPanel);
            zoomPanel.SetActive(true);
            pointCloudPanel.SetActive(isShowPointCloud);
            _toggleShow.isOn = isShowPointCloud;
        }
    }
}