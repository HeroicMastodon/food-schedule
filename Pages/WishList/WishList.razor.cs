using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodSchedule.Model;
using foodSchedule.Net;
using Microsoft.AspNetCore.Components;

namespace foodSchedule.Pages
{
    public class WishListBase : ComponentBase
    {
        protected WishListItem FormItem { get; set; } = new();
        [Inject]
        protected  ServerFacade ServerFacade { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected List<WishListItem> WishListItems = new();

        protected List<WishListItem> FilteredItems = new();

        protected bool IsNewItemFormOpen { get; set; } = false;
        protected bool IsItemNew { get; set; } = false;
        protected bool IsFilterFormOpen { get; set; } = false;
        protected bool IsSortFormOpen { get; set; } = false;
        protected WishListFilter Filter { get; set; } = new WishListFilter();
        protected int WishListTotal => WishListItems.Sum(item => item.Cost );
        protected int FilteredTotal => FilteredItems.Sum(item => item.Cost);

        protected override async Task OnInitializedAsync()
        {
            if (! await ServerFacade.Authenticate()) NavigationManager.NavigateTo("");
            WishListItems = (await ServerFacade.RetrieveWishlist()).Wishlist;
            FilterItems();
        }

        protected void OpenForm(WishListItem item = null)
        {
            FormItem = item ?? new WishListItem();
            IsItemNew = item == null;
            IsNewItemFormOpen = true;
            base.StateHasChanged();
        }

        protected async void DeleteItem(WishListItem item)
        {
            WishListItems.Remove(item);
            await ServerFacade.UpdateWishlist(WishListItems);
            FilterItems();
        }

        protected void OpenFilterForm()
        {
            IsFilterFormOpen = true;
        }

        protected void OpenSortForm()
        {
            IsSortFormOpen = true;
        }

        protected void CloseForms()
        {
            FilterItems();
            IsNewItemFormOpen = false;
            IsFilterFormOpen = false;
            IsSortFormOpen = false;
            base.StateHasChanged();
        }

        protected async void SaveItem()
        {
            if (IsItemNew)
            {
                WishListItems.Add(FormItem);
            }

            await ServerFacade.UpdateWishlist(WishListItems);
            CloseForms();
        }

        protected void FilterItems()
        {
            FilteredItems.Clear();

            foreach (var item in WishListItems)
            {
                
                if (! string.IsNullOrEmpty(Filter.Name) && ! item.Name.Contains(Filter.Name)) {
                    continue;
                }

                if (! string.IsNullOrEmpty(Filter.Category) && ! item.Category.Contains(Filter.Category)) {
                    continue;
                }

                if (item.Importance == 0 && ! Filter.NotImportant) {
                    continue;
                }

                if (item.Importance == 1 && ! Filter.Important) {
                    continue;
                }

                if (item.Importance == 2 && ! Filter.VeryImportant) {
                    continue;
                }

                if (item.Cost < Filter.LowerCostBound) {
                    continue;
                }

                if (Filter.UpperCostBound > 0 && (item.Cost > Filter.UpperCostBound)) {
                    continue;
                }
                
                FilteredItems.Add(item);
            }
        }

        protected void SortItems(string field = "name", bool reverse = false)
        {
            field = field.ToLower();
            switch(field) {
                case "importance":
                    WishListItems.Sort((expense, other) => expense.Importance.CompareTo(other.Importance));
                    break;
                case "category":
                    WishListItems.Sort((expense, other) => expense.Category.CompareTo(other.Category));
                    break;
                case "cost":
                    WishListItems.Sort((expense, other) => expense.Cost.CompareTo(other.Cost));
                    break;
                case "date wanted":
                    WishListItems.Sort((expense, other) => expense.DateWanted.Date.CompareTo(other.DateWanted.Date));
                    break;
                default:
                    WishListItems.Sort((expense, other) => expense.Name.CompareTo(other.Name));
                    break;
            }
            if (reverse) WishListItems.Reverse();
            base.StateHasChanged();
            CloseForms();
        }
    }

    public class WishListFilter
    {
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public int LowerCostBound { get; set; } = 0;
        public int UpperCostBound { get; set; } = 0;
        public bool NotImportant = true;
        public bool Important = true;
        public bool VeryImportant = true;
    }
}