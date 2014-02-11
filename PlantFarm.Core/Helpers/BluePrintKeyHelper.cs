using System;

namespace PlantFarm.Core.Helpers
{
    internal class BluePrintKeyHelper
    {
        public string GetBluePrintKey<T>(string variation)
        {
            return GetBluePrintKey(variation, typeof(T));
        }

        public string GetBluePrintKey(string variation, Type type)
        {
            return string.Format("{0}-{1}", type, variation);
        }
    }
}