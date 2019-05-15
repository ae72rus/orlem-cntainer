using System;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }


    }

    class Test1
    {
        public Test1(int param1, string param2, double param3, Test1 param4)
        {
            
        }
    }

    class Factory1
    {
        Test1 Create(int param1, string param2, double param3, Test1 param4)
        {
            return new Test1(param1, param2, param3, param4);
        }
    }
}
