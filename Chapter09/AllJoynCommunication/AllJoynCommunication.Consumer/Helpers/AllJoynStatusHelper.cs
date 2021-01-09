using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Devices.AllJoyn;

namespace AllJoynCommunication.Consumer.Helpers
{
    public static class AllJoynStatusHelper
    {
        private static Dictionary<int, string> namedAllJoynStatusDictionary = GetNamedStatusCodes();

        private const string unknownStatus = "Unknown status";

        public static string GetStatusCodeName(int statusCode)
        {
            var statusName = unknownStatus;

            if (namedAllJoynStatusDictionary.ContainsKey(statusCode))
            {
                statusName = namedAllJoynStatusDictionary[statusCode];
            }

            return statusName;
        }

        private static Dictionary<int, string> GetNamedStatusCodes()
        {
            var namedStatusCodes = typeof(AllJoynStatus).GetRuntimeProperties().Select(
                r => new RequestStatus()
                {
                    Name = r.Name,
                    Value = (int)r.GetValue(null)
                });

            var result = new Dictionary<int, string>();

            foreach (var namedStatusCode in namedStatusCodes)
            {
                if (!result.ContainsKey(namedStatusCode.Value))
                {
                    result.Add(namedStatusCode.Value, namedStatusCode.Name);
                }
            }

            return result;
        }
    }
}
