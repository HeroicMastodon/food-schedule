using System.Windows.Markup;

namespace foodSchedule.Model {
    public class Expense {
        public string Name {get; set;}
        public int Cost {get; set;}
        public int Importance {get; set;}
        public string Category {get; set;}

        public override string ToString()
        {
            return $"Expense: {Name} ${Cost} for {Category} | {ImportanceName}";
        }

        public string ImportanceName {get {
            switch(Importance) {
                case 1:
                    return "Important";
                case 2:
                    return "Very Important";
                default:
                    return "Not Important";
            }
        }}
    }
}