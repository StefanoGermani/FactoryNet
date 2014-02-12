namespace FactoryNet.Tests.TestModels
{
    public class Book
    {
        public Book(string author, string publisher)
        {
            Author = author;
            Publisher = publisher;
        }

        public string Author { get; set; }
        public string Publisher { get; set; }
    }
}