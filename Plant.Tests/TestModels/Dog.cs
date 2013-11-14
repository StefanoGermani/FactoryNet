using System;

namespace Plant.Tests.TestModels
{
    public class Dog
    {
        public string Name { get; set; }

        public string ThrowException
        {
            get { throw new Exception(); }
        }

        public Dog ThisDog
        {
            get { return null; }
        }
    }
}