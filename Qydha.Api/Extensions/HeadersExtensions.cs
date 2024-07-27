using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Qydha.API.Extensions;

public static class HeadersExtensions
{
    // " => X-INFO : "{environment: production, appVersion: 2.5.3, platform: Android, deviceName: Infinix X6823C, deviceId: 7b5f44f0-4c3a-11ef-8d98-818bdf77a1a7}" " 


    public static XInfoData GetXInfoHeaderData(this IHeaderDictionary headers)
    {
        if (headers.TryGetValue("X-INFO", out StringValues xInfoAsStringArr))
        {
            try
            {
                string xInfoAsString = xInfoAsStringArr.FirstOrDefault() ?? "";
                if (string.IsNullOrEmpty(xInfoAsString)) return new XInfoData();
                xInfoAsString = xInfoAsString.Replace("{", "{\"")
                      .Replace("}", "\"}")
                      .Replace(": ", "\": \"")
                      .Replace(", ", "\", \"");
                XInfoData data = JsonConvert.DeserializeObject<XInfoData>(xInfoAsString) ?? new XInfoData();
                return data;

            }
            catch (Exception)
            {
                return new XInfoData();
            }
        }
        return new XInfoData();
    }
}
