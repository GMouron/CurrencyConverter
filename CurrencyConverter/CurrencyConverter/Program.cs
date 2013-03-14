using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurrencyConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string toConvert = Console.ReadLine();
            int numberOfRates = -1;
            if (int.TryParse(Console.ReadLine(), out numberOfRates) && numberOfRates > 0)
            {
                List<string> rates = new List<string>();
                for (int i = 0; i < numberOfRates; i++)
                {
                    rates.Add(Console.ReadLine());
                }
                Console.WriteLine(convert(toConvert, rates));
            }
            else
            {
                Console.WriteLine("Was expecting (positive) number of rates");
            }


        }
        private static string convert(string toConvert, List<string> rates)
        {
            return null;
        }
    }


}
