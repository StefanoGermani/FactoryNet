namespace Plant.Core.Impl
{
    internal class BlueprintKeyGenerator
    {
        public static string BluePrintKey<T>()
        {
            return BluePrintKey<T>(string.Empty);
        }

        public static string BluePrintKey<T>(string variation)
        {
            return string.Format("{0}-{1}", typeof(T), variation);
        }
    }
}