using UnityEngine;

namespace FitAndShape
{
    public class LoadingView : MonoBehaviour
    {
        public bool Visible { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }
    }
}