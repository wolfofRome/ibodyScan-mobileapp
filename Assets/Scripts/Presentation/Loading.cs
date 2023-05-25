using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Amatib.ObjViewer.Presentation
{
    public class Loading : MonoBehaviour
    {
        private static readonly float DURATION = 1f;

        private List<Tween> _tweenList = new List<Tween>();

        void Start()
        {
            Image[] circles = GetComponentsInChildren<Image>();

            for (var i = 0; i < circles.Length; i++)
            {
                var angle = -2 * Mathf.PI * i / circles.Length;
                circles[i].rectTransform.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 40f;
                _tweenList.Add(circles[i].DOFade(0f, DURATION).SetLoops(-1, LoopType.Yoyo).SetDelay(DURATION * i / circles.Length));
            }
        }

        private void OnDestroy()
        {
            foreach (var tween in _tweenList)
            {
                tween.Kill();
            }
        }
    }
}
