using System.Collections.Generic;

namespace foodSchedule.Model {
    public class GetFoodScheduleResponse {

        public List<string> Days { get; set; } = new List<string>();

        public GetFoodScheduleResponse() {
            for (int i = 0; i < 7; i++) {
                Days.Add("");
            }
        }

        public GetFoodScheduleResponse(List<string> days) {
            Days = days;
        }
    }

    public class GetExpensesResponse {
        public List<Expense> Expenses { get; set; }

        public GetExpensesResponse(List<Expense> expenses) {
            Expenses = expenses;
        }
    }

    public class GetWishlistResponse
    {
        public List<WishListItem> Wishlist { get; set; }

        public GetWishlistResponse(List<WishListItem> wishlist)
        {
            Wishlist = wishlist;
        }
    }
}