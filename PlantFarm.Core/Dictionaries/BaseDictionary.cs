using System;

namespace PlantFarm.Core.Dictionaries
{
    internal class BaseDictionary
    {
        public string BluePrintKey<T>(string variation)
        {
            return BluePrintKey(variation, typeof(T));
        }

        public string BluePrintKey(string variation, Type type)
        {
            return string.Format("{0}-{1}", type, variation);
        }
    }
}