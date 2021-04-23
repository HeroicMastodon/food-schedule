using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodSchedule.Model;
using Microsoft.AspNetCore.Components;

namespace foodSchedule.Pages
{
    public class WishListBase : ComponentBase
    {
        protected WishListItem FormItem { get; set; } = new WishListItem();

        protected List<WishListItem> WishListItems = new List<WishListItem>
        {
            new WishListItem()
            {
                Name = "one",
                Category = "Some Category",
                Cost = 100,
                Description =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Importance = 1,
            },
            new WishListItem()
            {
                Name = "two",
                Category = "Category",
                Cost = 50,
                Description = "Some Description",
                Importance = 0,
                ImageUrl = "https://cdn.pixabay.com/photo/2018/07/15/10/44/dna-3539309__340.jpg"
            },
            new WishListItem()
            {
                Name = "three",
                Category = "Some",
                Cost = 129,
                Description = "Some Description",
                Importance = 2,
                ImageUrl = "https://cdn.pixabay.com/photo/2018/07/15/10/44/dna-3539309__340.jpg"
            }
        };

        protected List<WishListItem> FilteredItems = new List<WishListItem>();

        protected bool IsNewItemFormOpen { get; set; } = false;
        protected bool IsItemNew { get; set; } = false;
        protected bool IsFilterFormOpen { get; set; } = false;
        protected bool IsSortFormOpen { get; set; } = false;
        protected WishListFilter Filter { get; set; } = new WishListFilter();
        protected int WishListTotal => WishListItems.Sum(item => item.Cost );
        protected int FilteredTotal => FilteredItems.Sum(item => item.Cost);

        protected override async Task OnInitializedAsync()
        {
            FilterItems();
        }

        protected void OpenForm(WishListItem item = null)
        {
            FormItem = item ?? new WishListItem();
            IsItemNew = item == null;
            IsNewItemFormOpen = true;
            base.StateHasChanged();
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

        protected void SaveItem()
        {
            if (IsItemNew)
            {
                WishListItems.Add(FormItem);
            }

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