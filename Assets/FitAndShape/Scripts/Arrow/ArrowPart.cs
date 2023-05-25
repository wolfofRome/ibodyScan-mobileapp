using System.Collections.Generic;
using UnityEngine;

namespace FitAndShape
{
    public enum ArrowPartType
    {
        Circumference,
        Line,
    }

    public sealed class ArrowPart : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;

        public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
        public ArrowPartType ArrowPartType { get; private set; }
        public MeasurementPart MeasurementPart { get; private set; }

        public List<LineRenderer> LineRendererList = new List<LineRenderer>();

        public List<ArrowHead> ArrowHeadList = new List<ArrowHead>();

        public void Initialize(ArrowPartType arrowPartType, MeasurementPart measurementPart)
        {
            ArrowPartType = arrowPartType;

            switch (ArrowPartType)
            {
                case ArrowPartType.Circumference:
                    _spriteRenderer.gameObject.SetActive(true);
                    break;
                default:
                    _spriteRenderer.gameObject.SetActive(false);
                    break;
            }

            MeasurementPart = measurementPart;

            gameObject.name = $"{gameObject.name}_{MeasurementPart}";

            SetCircumferenceScale();
        }

        public void SetCircumferenceScale()
        {
            Vector3 scale = Vector3.one;

            switch (MeasurementPart)
            {
                case MeasurementPart.NeckCircumference:
                case MeasurementPart.RightUpperArmCircumference:
                case MeasurementPart.LeftUpperArmCircumference:
                case MeasurementPart.RightThighCircumference:
                case MeasurementPart.LeftThighCircumference:
                case MeasurementPart.RightLowerLegMaxCircumference:
                case MeasurementPart.LeftLowerLegMaxCircumference:
                case MeasurementPart.RightLowerLegMinCircumference:
                case MeasurementPart.LeftLowerLegMinCircumference:
                case MeasurementPart.RightWristCircumference:
                case MeasurementPart.LeftWristCircumference:
                    scale = new Vector3(0.1f, 0.03f, 0.1f);
                    break;
                case MeasurementPart.ChestCircumference:
                case MeasurementPart.BustTopCircumference:
                case MeasurementPart.WaistMaxCircumference:
                case MeasurementPart.WaistMinCircumference:
                case MeasurementPart.Hip1Circumference:
                case MeasurementPart.Hip2Circumference:
                case MeasurementPart.Hip3Circumference:
                case MeasurementPart.Hip4Circumference:
                case MeasurementPart.Hip5Circumference:
                    scale = new Vector3(0.25f, 0.05f, 0.25f);
                    break;
                default:
                    break;
            }

            _spriteRenderer.transform.localScale = scale;
        }

        public void SetColor(Color color, Material material, string layerName)
        {
            SetSpriteColor(color, layerName);

            SetLineRendererMaterial(material, layerName);

            SetArrowHeadColor(color, layerName);
        }

        public void SetSpriteColor(Color color, string layerName)
        {
            _spriteRenderer.color = color;
            _spriteRenderer.gameObject.layer = LayerMask.NameToLayer(layerName);
        }

        public void SetLineRendererMaterial(Material material, string layerName)
        {
            foreach (var lneRenderer in LineRendererList)
            {
                lneRenderer.material = material;
                lneRenderer.gameObject.layer = LayerMask.NameToLayer(layerName);
            }
        }

        public void SetArrowHeadColor(Color color, string layerName)
        {
            foreach (var arrowHead in ArrowHeadList)
            {
                arrowHead.SpriteRenderer.color = color;
                arrowHead.SpriteRenderer.gameObject.layer = LayerMask.NameToLayer(layerName);
            }
        }
    }
}