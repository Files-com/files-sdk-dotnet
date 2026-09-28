using System.Collections.Generic;

namespace FilesCom.Util
{
    internal static class DictionaryUtil
    {
        // A shallow copy for the Async and client methods, which neither change the caller's dictionaries nor see
        // the caller's later changes to them.
        internal static Dictionary<string, object> Copy(Dictionary<string, object> dictionary)
        {
            return dictionary == null ? null : new Dictionary<string, object>(dictionary);
        }
    }
}