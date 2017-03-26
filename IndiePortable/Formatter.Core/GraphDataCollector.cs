
namespace IndiePortable.Formatter.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    internal static class GraphDataCollector
    {

        internal static IEnumerable<ObjectDataStore> GetGraphData(object root, IEnumerable<ISurrogate> surrogates)
        {

        }


        private static void GetGraphData(object currentObject, IDictionary<object, ObjectDataStore> objectData, IEnumerable<ISurrogate> surrogates)
        {
            if (objectData.ContainsKey(currentObject))
            {
                return;
            }

            if (object.ReferenceEquals(currentObject, null))
            {
                objectData.Add(currentObject, new ObjectDataStore(currentObject));
            }
        }
    }
}
