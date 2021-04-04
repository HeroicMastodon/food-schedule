using System.Collections.Generic;
using foodSchedule.Model;
using Microsoft.AspNetCore.Components;
using foodSchedule.Net;
using System.Threading.Tasks;
using System;

namespace foodSchedule.Pages {
    public class ExpensesBase: ComponentBase {
        protected bool IsFormOpen {get; set;}
        protected bool IsFilterFormOpen {get; set;}
        protected bool ShouldFilter {get; set;} = false;
        protected Expense NewExpense {get; set;} = new Expense();
        protected List<Expense> Expenses {get; set;} = new List<Expense>();
        protected List<Expense> FilteredExpenses {get; set;} = new List<Expense>();
        protected ExpenseFilter Filter {get; set;} = new ExpenseFilter();
        [Inject]
        protected ServerFacade ServerFacade {get; set;}
        [Inject]
        protected NavigationManager NavigationManager {get; set;}

        protected override async Task OnInitializedAsync() {
            if (! await ServerFacade.Authenticate()) NavigationManager.NavigateTo("");
            Expenses = (await ServerFacade.RetrieveExpenses()).Expenses;
            FilterExpenses();
        }


        protected async void Save() {
            if (string.IsNullOrEmpty(NewExpense.Name)) return;
            Expenses.Add(NewExpense);
            await ServerFacade.UpdateExpenses(Expenses);
            FilterExpenses();
            NewExpense = new Expense();
            IsFormOpen = false;
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

        protected void SortExpenses(string field = "name", bool reverse = false) {
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