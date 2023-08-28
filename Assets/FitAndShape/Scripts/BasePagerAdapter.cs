using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FitAndShape
{
    public abstract class BasePagerAdapter : MonoBehaviour
    {
        private UnityEvent _onNotifyDataSetChanged = new UnityEvent();
        public UnityEvent OnNotifyDataSetChanged
        {
            get
            {
                return _onNotifyDataSetChanged;
            }
        }

        protected virtual void Awake() { }

        protected virtual void Start() { }

        protected virtual void Update() { }

        public abstract int GetCount();

        public abstract List<BasePageFrame> GetItemList();

        public abstract void ClearAllItems();

        public abstract int AddItem(BasePageFrame item);

        public abstract BasePageFrame GetItem(int pagePosition);

        public abstract GameObject InstantiateItem(int pagePosition, Vector2 anchorMin, Vector2 anchorMax);

        public abstract void BindItem(int pagePosition, Vector2 anchorMin, Vector2 anchorMax);

        public abstract void UnBindItem(int pagePosition);

        public abstract void OnGotFocusItem(int pagePosition);

        public abstract void OnLostFocusItem(int pagePosition);

        public virtual void NotifyDataSetChanged()
        {
            OnNotifyDataSetChanged.Invoke();
        }
    }
}
