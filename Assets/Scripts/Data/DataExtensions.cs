using UnityEngine;

namespace Roguelike.Data
{
    public static class DataExtensions
    {
        public static string ToJson(this object obj) => 
            JsonUtility.ToJson(obj, true);

        public static T FromJson<T>(this string json) =>
            JsonUtility.FromJson<T>(json);
    }
}