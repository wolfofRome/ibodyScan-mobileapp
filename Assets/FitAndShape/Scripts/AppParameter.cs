using UnityEngine;

namespace FitAndShape
{
    [CreateAssetMenu(fileName = "AppParameter", menuName = "ScriptableObjects/CreateAppParameter")]
    public class AppParameter : ScriptableObject
    {
        [SerializeField] float _fieldOfViewUpperRatio;
        [SerializeField] float _fieldOfViewLowerRatio;
        [SerializeField] float _upperHeight;
        [SerializeField] float _lowerHeight;

        public float GetFieldOfView(float height)
        {
            return (float)(_fieldOfViewUpperRatio - _fieldOfViewLowerRatio) * (float)(height - _lowerHeight) / (float)(_upperHeight - _lowerHeight) + _fieldOfViewLowerRatio;
        }
    }
}