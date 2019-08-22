using System;
using System.Threading;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			string name = Console.ReadLine();
			int age = int.Parse(Console.ReadLine());
			string town = Console.ReadLine();

			Console.WriteLine($"I am {name} - {age} years old. I am from {town}.");
		}
	}
}
