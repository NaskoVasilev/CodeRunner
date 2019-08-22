using System;
using System.Collections.Generic;
using ZipSubmissionTest.Models;

namespace ZipSubmissionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Car> cars = new List<Car>();
            string input;

            while ((input = Console.ReadLine()) != "end")
            {
                string[] data = input.Split(' ');
                cars.Add(new Car { Model = data[0], Year = int.Parse(data[1]) });
            }


            foreach (var car in cars)
            {
                Console.WriteLine(car);
            }
        }
    }
}
