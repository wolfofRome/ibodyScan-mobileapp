using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public class Badge : MonoBehaviour
    {
        [SerializeField]
        private Text _text = default;

        private int _number = 0;
        public int number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
                _text.text = value.ToString();
            }
        }
    }
}
