using System.Collections.Generic;

namespace foodSchedule.Model.Request {
    public class LoginRequest {
        public LoginRequest(string username, string password) {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }

    }

    public class RegisterRequest {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RealName { get; set; }
        public string Email { get; set; }

        public bool Validate() {
            if (
                string.IsNullOrEmpty(Username) || 
                string.IsNullOrEmpty(Password) || 
                string.IsNullOrEmpty(RealName) || 
                string.IsNullOrEmpty(Email)) {
                return false;
            }

            return true;
        }
    }

    public class UpdateScheduleRequest {
        public string UserId {get; init; }

        public List<string> Days {get; init;}

        public UpdateScheduleRequest(string userId, List<string> days) {
            UserId = userId;
            Days = days;
        }
    }

    public class UpdateExpensesRequest {
        public UpdateExpensesRequest(string userId, List<Expense> expenses) {
            UserId = userId;
            Expenses = expenses;
        }

        public string UserId {get; set;}
        public List<Expense> Expenses {get; set;}
    }
}