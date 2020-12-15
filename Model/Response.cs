using System.Collections.Generic;

namespace foodSchedule.Model {
    public class GetFoodScheduleResponse {

        public List<string> Days {get; set;} = new List<string>();

        public GetFoodScheduleResponse() {
            for (int i = 0; i < 7; i++) {
                Days.Add("");
            }
        }

        public GetFoodScheduleResponse(List<string> days) {
            Days = days;
        }
    }
}