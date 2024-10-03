using System.Text.Json;

namespace E_Commerce_C__ASP.NET.Utility
{
    public static class SessionExtensions
    {
       
            // Método para armazenar um objeto na sessão
            public static void SetObjectAsJson(this ISession session, string key, object value)
            {
                session.SetString(key, JsonSerializer.Serialize(value));
            }

            // Método para recuperar um objeto da sessão
            public static T GetObjectFromJson<T>(this ISession session, string key)
            {
                var jsonString = session.GetString(key);
                return jsonString == null ? default(T) : JsonSerializer.Deserialize<T>(jsonString);
            }
       
    }
}
