using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NetworkService.TestRunner.Helper
{
    public static class ClientHelper
    {
        public static string ConvertToString(SecureString secure)
        {
            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secure);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static string SerializeObject(dynamic objectToSerialize)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, serializerSettings);
        }

        public static dynamic DeserializeObject(object objectToDeserialize)
        {
            return JsonConvert.DeserializeObject<dynamic>(objectToDeserialize.ToString());
        }
    }
}
