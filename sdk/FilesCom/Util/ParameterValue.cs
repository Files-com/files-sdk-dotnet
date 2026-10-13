using System;
using System.Globalization;
using System.Linq;

namespace FilesCom.Util
{
    // Converts the value of a model property into the form the parameter of the same name takes, where the API gives
    // the two different types, so that SaveAsync can send what a property holds. Any other value, such as text the
    // caller supplied already in the parameter's form, is returned unchanged.
    internal static class ParameterValue
    {
        // A date-time parameter takes ISO 8601 text, written as the SDK's JSON serializer writes a DateTime: Z for UTC,
        // the offset for local time, none for an unspecified kind, and fractional seconds only when there are some.
        internal static object FromDateTime(object value)
        {
            return value is DateTime dateTime
                ? dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture)
                : value;
        }

        // An ID list parameter that the API documents as comma-delimited text. A list missing an ID is left as it is,
        // for the parameter's validation to reject.
        internal static object FromIds(object value)
        {
            return value is long?[] ids && ids.All(id => id.HasValue)
                ? string.Join(",", ids.Select(id => id.Value.ToString(CultureInfo.InvariantCulture)))
                : value;
        }
    }
}