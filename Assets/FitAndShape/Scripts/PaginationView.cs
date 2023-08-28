using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace FitAndShape
{
    public sealed class PaginationView : MonoBehaviour
    {
        [SerializeField] PageNumberView _pageNumberViewPrefab;
        [SerializeField] int _itemsPerPage = 5;

        public IObservable<int> OnClick => _onClick;
        Subject<int> _onClick = new Subject<int>();

        List<PageNumberView> _items = new List<PageNumberView>();
        int _currentPage = 0;
        int _totalPages = 0;

        public void Create(int totalPages)
        {
            _currentPage = 0;
            _totalPages = totalPages;
            
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }

            _items.Clear();

            for (int i = 0; i < totalPages; i++)
            {
                PageNumberView pageNumberView = Instantiate(_pageNumberViewPrefab, transform);
                pageNumberView.Page = i + 1;
                pageNumberView.Selected = false;

                pageNumberView.OnClick.Subscribe(n =>
                {
                    _onClick.OnNext(n);

                }).AddTo(this);

                _items.Add(pageNumberView);
            }

            PageNumberView firstPageNumberView = _items.Where(n => n.Page == 1).FirstOrDefault();

            if (firstPageNumberView != null)
            {
                firstPageNumberView.Selected = true;
            }

            UpdatePagination(1);
        }

        public void UpdatePagination(int page)
        {
            foreach (var item in _items)
            {
                item.Selected = item.Page == page;
            }

            _currentPage = page - 1;

            int halfCount = _itemsPerPage / 2;

            int min = _currentPage - halfCount;
            int max = _currentPage + halfCount;

            if (_currentPage - halfCount < 0)
            {
                max = Math.Min(max + Math.Abs(_currentPage - halfCount), _totalPages - 1);
            }

            if (_currentPage + halfCount > _totalPages - 1)
            {
                int add = _currentPage + halfCount - _totalPages - 1;
                min = Math.Max(min - add, 0);
            }

            min = Math.Clamp(min, 0, _totalPages - 1);
            max = Math.Clamp(max, 0, _totalPages - 1);

            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }

            for (int i = min; i < max + 1; i++)
            {
                _items[i].gameObject.SetActive(true);
            }
        }

        //void NextPage()
        //{
        //    if (_currentPage < _totalPages - 1)
        //    {
        //        _currentPage++;
        //        UpdatePagination();
        //    }
        //}

        //void PreviousPage()
        //{
        //    if (_currentPage > 0)
        //    {
        //        _currentPage--;
        //        UpdatePagination();
        //    }
        //}
    }
}