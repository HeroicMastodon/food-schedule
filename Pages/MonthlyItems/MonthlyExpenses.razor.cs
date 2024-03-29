using System.Collections.Generic;
using foodSchedule.Model;
using Microsoft.AspNetCore.Components;
using foodSchedule.Net;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace foodSchedule.Pages {
    public class ExpensesBase: ComponentBase {
        protected bool IsFormOpen {get; set;}
        protected bool IsFilterFormOpen {get; set;}
        protected bool IsEditFormOpen {get; set;}
        protected bool ShouldFilter {get; set;} = false;
        protected Expense NewExpense {get; set;} = new Expense();
        protected Expense EditExpense {get; set;}
        protected List<Expense> Expenses {get; set;} = new List<Expense>();
        protected List<Expense> FilteredExpenses {get; set;} = new List<Expense>();
        
        protected ExpenseFilter Filter {get; set;} = new ExpenseFilter();
        [Inject]
        protected ServerFacade ServerFacade {get; set;}
        [Inject]
        protected NavigationManager NavigationManager {get; set;}

        protected int GetFilteredCosts() {
            return FilteredExpenses.Sum(expense => expense.TotalCost);
        }

        protected int GetTotalCosts() {
            return Expenses.Sum(expense => expense.TotalCost);
        }

        protected override async Task OnInitializedAsync() {
            if (! await ServerFacade.Authenticate()) NavigationManager.NavigateTo("");
            Expenses = (await ServerFacade.RetrieveExpenses()).Expenses;
            FilterExpenses();
        }

        protected void OpenEditForm(Expense expense) {
            EditExpense = expense;
            IsEditFormOpen = true;
        }

        protected void CloseEditForm() {
            IsEditFormOpen = false;
        }

        protected async void AddNewExpense() {
            if (string.IsNullOrEmpty(NewExpense.Name)) return;
            Expenses.Add(NewExpense);
            await ServerFacade.UpdateExpenses(Expenses);
            FilterExpenses();
            NewExpense = new Expense();
            IsFormOpen = false;
            base.StateHasChanged();
        }

        protected async void UpdateExpenses() {
            await ServerFacade.UpdateExpenses(Expenses);
            FilterExpenses();
            CloseEditForm();
            base.StateHasChanged();
        }

        protected async void DeleteExpense(Expense expense) {
            Expenses.Remove(expense);
            await ServerFacade.UpdateExpenses(Expenses);
            FilterExpenses();
            base.StateHasChanged();
        }

        protected void FilterExpenses() {
            FilteredExpenses.Clear();

            foreach (var expense in Expenses) {
                if (! string.IsNullOrEmpty(Filter.Name) && ! expense.Name.Contains(Filter.Name)) {
                    continue;
                }

                if (! string.IsNullOrEmpty(Filter.Category) && ! expense.Category.Contains(Filter.Category)) {
                    continue;
                }

                if (expense.Importance == 0 && ! Filter.NotImportant) {
                    continue;
                }

                if (expense.Importance == 1 && ! Filter.Important) {
                    continue;
                }

                if (expense.Importance == 2 && ! Filter.VeryImportant) {
                    continue;
                }

                if (expense.Cost < Filter.CostLowerBound) {
                    continue;
                }

                if (Filter.CostUpperBound > 0 && (expense.Cost > Filter.CostUpperBound)) {
                    continue;
                }

                FilteredExpenses.Add(expense);
            } 

            IsFilterFormOpen = false;
        }

        protected void SortExpenses(string field = "name", bool reverse = false)
        {
            field = field.ToLower();
            switch(field) {
                case "importance":
                    FilteredExpenses.Sort((expense, other) => expense.Importance.CompareTo(other.Importance));
                    break;
                case "category":
                    FilteredExpenses.Sort((expense, other) => expense.Category.CompareTo(other.Category));
                    break;
                case "cost":
                    FilteredExpenses.Sort((expense, other) => expense.Cost.CompareTo(other.Cost));
                    break;
                case "count":
                    FilteredExpenses.Sort((expense, other)=> expense.Cost.CompareTo(other.Cost));
                    break;
                case "total cost":
                    FilteredExpenses.Sort(((expense, other) => expense.TotalCost.CompareTo(other.TotalCost)));
                    break;
                default:
                    FilteredExpenses.Sort((expense, other) => expense.Name.CompareTo(other.Name));
                    break;
            }
            if (reverse) FilteredExpenses.Reverse();
        }

        protected void ClearFilter() {
            Filter = new ExpenseFilter();
            FilteredExpenses.Clear();
            FilteredExpenses.AddRange(Expenses);
        }
    }

    public class ExpenseFilter {
        public string Name {get; set;} = "";
        public string Category {get; set;} = "";
        public bool NotImportant = true;
        public bool Important = true;
        public bool VeryImportant = true;
        public int CostLowerBound = 0;
        public int CostUpperBound = 0;
    }
}