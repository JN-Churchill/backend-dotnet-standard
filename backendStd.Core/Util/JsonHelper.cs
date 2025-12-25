using System.Text.Json;
using System.Text.Json.Serialization;

namespace backendStd.Core.Util;

/// <summary>
/// JSON帮助类
/// </summary>
public static class JsonHelper
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false
    };

    /// <summary>
    /// 对象序列化为JSON字符串
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns>JSON字符串</returns>
    public static string Serialize(object obj)
    {
        if (obj == null) return string.Empty;
        return JsonSerializer.Serialize(obj, _options);
    }

    /// <summary>
    /// JSON字符串反序列化为对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">JSON字符串</param>
    /// <returns>对象实例</returns>
    public static T Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;
        return JsonSerializer.Deserialize<T>(json, _options);
    }

    /// <summary>
    /// JSON字符串反序列化为对象（非泛型）
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <param name="type">对象类型</param>
    /// <returns>对象实例</returns>
    public static object Deserialize(string json, Type type)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        return JsonSerializer.Deserialize(json, type, _options);
    }
}
