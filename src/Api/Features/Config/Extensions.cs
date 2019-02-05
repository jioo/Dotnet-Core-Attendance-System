using System;

namespace WebApi.Features.Config
{
    public static class Extensions
    {
        public static bool ValidateTimeStrings(ConfigViewModel viewModel, out string message)
        {
            TimeSpan outResult;
            message = "";

            var timeInResult = TimeSpan.TryParse(viewModel.TimeIn, out outResult);
            if (!timeInResult)
                message = "Invalid time value in TimeIn field.";

            var timeOutResult =  TimeSpan.TryParse(viewModel.TimeOut, out outResult);
            if (!timeOutResult)
                message = "Invalid time value in TimeOut field.";

            return !(timeInResult && timeOutResult);
        }
    }
}