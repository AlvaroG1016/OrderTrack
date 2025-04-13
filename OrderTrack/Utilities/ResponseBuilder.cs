using OrderTrack.Models.CustomResponses;

namespace OrderTrack.Utilities
{
    public class ResponseBuilder
    {
        public static GeneralResponse BuildSuccessResponse(object data, string message = "")
        {
            return new GeneralResponse { Result = true, Data = new List<object> { data }, Message = message };
        }

        public static GeneralResponse BuildErrorResponse(string message)
        {
            return new GeneralResponse { Result = false, Message = message };
        }
    }

}
