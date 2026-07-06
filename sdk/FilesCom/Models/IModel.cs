using System.Collections.Generic;

namespace FilesCom.Models
{
    internal interface IModel
    {
        void SetOptions(Dictionary<string, object> options);
    }
}