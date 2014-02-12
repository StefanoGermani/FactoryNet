using System.Collections.Generic;

namespace FactoryNet.Tests.TestModels
{
    public class House
    {
        public House(string color, int squareFoot)
        {
            Color = color;
            SquareFoot = squareFoot;
        }

        public House()
        {
            Persons = new HashSet<Person>();
        }

        public string Color { get; set; }
        public int SquareFoot { get; set; }
        public string Summary { get; set; }

        public ICollection<Person> Persons { get; set; }
    }
}