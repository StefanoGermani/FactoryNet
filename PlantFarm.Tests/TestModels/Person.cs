namespace Plant.Tests.TestModels
{
    public class Person
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public long Age { get; set; }

        public House HouseWhereILive { get; set; }
    }
}