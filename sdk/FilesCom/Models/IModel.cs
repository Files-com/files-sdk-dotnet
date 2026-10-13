using System.Collections;
using System.Collections.Generic;

namespace FilesCom.Models
{
    internal interface IModel
    {
        // For a model the SDK has just made: it belongs to client (null leaves the choice to its first request), gets
        // its own copy of options, and sends its requests to origin, or, when that is null, to its client's BaseUrl.
        void SetContext(FilesClient client, Dictionary<string, object> options, string origin);

        // Where the model's operations send their requests when not to its client's BaseUrl, or null.
        string Origin { get; }

        // The models held in this model's properties.
        IEnumerable<object> NestedModels { get; }
    }

    internal static class ModelContext
    {
        // Sets the context of a model read from a response, or of each model in a collection read from one, and of
        // every model nested in them. They are all new objects, so none of them has a client yet.
        internal static void SetForResponse(object value, FilesClient client, Dictionary<string, object> options, string origin)
        {
            if (value is IModel model)
            {
                model.SetContext(client, options, origin);
                foreach (object nested in model.NestedModels)
                {
                    SetForResponse(nested, client, options, origin);
                }
            }
            else if (value is IEnumerable collection && !(value is string))
            {
                foreach (object item in collection)
                {
                    SetForResponse(item, client, options, origin);
                }
            }
        }
    }
}