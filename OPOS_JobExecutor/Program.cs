using System;

namespace OPOS_project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Consumer consumer = new Consumer();
            consumer.ConsumeMessagesFromUnscheduled();

            Console.ReadLine();

            
            consumer.Dispose();
            
        }
    }
}