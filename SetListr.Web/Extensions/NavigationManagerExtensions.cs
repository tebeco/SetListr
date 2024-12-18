using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace SetListr.Web.Extensions;

public static class NavigationManagerExtensions
{
    public static bool TryGetValue<T>(this NavigationManager navigationManager, string name, [NotNullWhen(true)] out T? value)
    {
        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
        if (!QueryHelpers.ParseQuery(uri.Query).TryGetValue(name, out var valueString))
        {
            value = default;
            return false;
        }

        if (typeof(T) == typeof(int))
        {
            if (!int.TryParse(valueString, out var i))
            {
                value = default;
                return false;
            }

            value = (T)(object)i;
            return true;
        }

        if (typeof(T) == typeof(string))
        {
            value = (T)(object)valueString.ToString();
            return true;
        }

        if (typeof(T) == typeof(Guid))
        {
            if(!Guid.TryParse(valueString, out var guid))
            {
                value = default;
                return false;
            }

            value = (T)(object)guid;
            return true;
        }

        throw new InvalidOperationException($"Unknown type T = {typeof(T)}");
    }
}