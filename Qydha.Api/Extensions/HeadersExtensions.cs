using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Qydha.API.Extensions;

public static class HeadersExtensions
{
    public static XInfoData GetXInfoHeaderData(this IHeaderDictionary headers)
    {
        if (headers.TryGetValue("X-INFO", out StringValues xInfoAsStringArr))
        {
            try
            {
                string xInfoAsString = xInfoAsStringArr.FirstOrDefault() ?? "";
                if (string.IsNullOrEmpty(xInfoAsString)) return new XInfoData();
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
