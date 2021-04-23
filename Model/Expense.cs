using System;
using System.Linq;

namespace foodSchedule.Model {
    public class Expense {
        public string Name {get; set;} = "";
        public int Cost {get; set;} = 0;
        public int Importance {get; set;} = 0;
        public string Category {get; set;} = "None";
        public int Count { get; set; } = 1;
        public int TotalCost => Cost * Count;

        public override string ToString()
        {
            return $"Expense: {Name} ${Cost} for {Category} | {ImportanceName}";
        }

        public string ImportanceName {get
        {
            return Importance switch
            {
                1 => "Important",
                2 => "Very Important",
                _ => "Not Important"
            };
        }}
    }

    public class WishListItem : Expense {
        public string ImageUrl {get; set;}
        public AFDate DateWanted { get; set; } = new AFDate();
        public AFDate DateCreated { get; set; } = new AFDate();
        public string Description { get; set; }

        public string Link
        {
            get
            {
                if (_link == null)
                {
                    return null;
                }
                if (_link.StartsWith("http"))
                {
                    return _link;
                }

                return "https://" + _link;
            }
            set => _link = value;
        }

        private string _link;
    }

    public class AFDate
    {
        public string DateString => string.Join(",", DateTimeOffset.DateTime.ToLongDateString().Split(",").Skip(1));
        public DateTime Date {get; set;}
        public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.Now;
        public override string ToString()
        {
            return DateString;
        }
    }
}