using UnityEngine;

namespace FitAndShape
{
    public sealed class ArrowHead : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
    }
}