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


            //test1();
            //test2();
            //test3();
            //test4();
        }
        // TODO Make those into proper unit tests
        // This is testing the exemple given on the page
        // Expected result : 59033
        private static void test1()
        {
            string toConvert = "EUR;550;JPY";
            List<string> rates = new List<string>();
            rates.Add("AUD;CHF;0.9661");
            rates.Add("JPY;KRW;13.1151");
            rates.Add("EUR;CHF;1.2053");
            rates.Add("AUD;JPY;86.0305");
            rates.Add("EUR;USD;1.2989");
            rates.Add("JPY;INR;0.6571");
            string v = convert(toConvert, rates);
            System.Diagnostics.Debug.WriteLine(v);
            Console.WriteLine(v);
        }
        // This is testing the second test case announced ("direct conversion")
        // Expected result : 714
        private static void test2()
        {
            string toConvert = "EUR;550;USD";
            List<string> rates = new List<string>();
            rates.Add("AUD;CHF;0.9661");
            rates.Add("JPY;KRW;13.1151");
            rates.Add("EUR;CHF;1.2053");
            rates.Add("AUD;JPY;86.0305");
            rates.Add("EUR;USD;1.2989");
            rates.Add("JPY;INR;0.6571");
            string v = convert(toConvert, rates);
            System.Diagnostics.Debug.WriteLine(v);
            Console.WriteLine(v);
        }
        // This is testing the third test case announced ("multiple paths, needs to take the shortest")
        // Expected result : 250
        private static void test3()
        {
            string toConvert = "AAA;10;CCC";
            List<string> rates = new List<string>();
            rates.Add("AAA;BBB;5");
            rates.Add("BBB;CCC;5");
            rates.Add("AAA;DDD;5");
            rates.Add("DDD;EEE;5");
            rates.Add("EEE;FFF;5");
            rates.Add("FFF;CCC;5");
            string v = convert(toConvert, rates);
            System.Diagnostics.Debug.WriteLine(v);
            Console.WriteLine(v);
        }
        // This is testing the fourth test case announced ("inverted direct conversion")
        //Expected result : 423
        private static void test4()
        {
            string toConvert = "USD;550;EUR";
            List<string> rates = new List<string>();
            rates.Add("AUD;CHF;0.9661");
            rates.Add("JPY;KRW;13.1151");
            rates.Add("EUR;CHF;1.2053");
            rates.Add("AUD;JPY;86.0305");
            rates.Add("EUR;USD;1.2989");
            rates.Add("JPY;INR;0.6571");
            string v = convert(toConvert, rates);
            System.Diagnostics.Debug.WriteLine(v);
            Console.WriteLine(v);
        }

        private const char SEPARATOR = ';';
        private const int FROM_CURRENCY_POSITION = 0;
        private const int ORIGINAL_AMOUNT_POSITION = 1;
        private const int TO_CURRENCY_POSITION = 2;
        private const int RATE_FROM_CURRENCY_POSITION = 0;
        private const int RATE_TO_CURRENCY_POSITION = 1;
        private const int RATE_POSITION = 2;
        private const decimal PRECISION = 10000M;
 
        private static string convert(string toConvert, List<string> rates)
        {
            string[] splitConvertTo = toConvert.Split(SEPARATOR);
            if(splitConvertTo.Length != 3) {
                return "Invalid line describing the conversion to be done";
            }
            string fromCurrency = splitConvertTo[FROM_CURRENCY_POSITION];
            decimal originalAmount = 0;
            if (!decimal.TryParse(splitConvertTo[ORIGINAL_AMOUNT_POSITION], out originalAmount))
            {
                return "Invalid line describing the conversion to be done";
            }
            string toCurrency = splitConvertTo[TO_CURRENCY_POSITION];
            GraphSearcher helper = new GraphSearcher();
            foreach (string rateDescription in rates)
            {
                string[] splitRateDescription = rateDescription.Split(SEPARATOR);
                string fromRateCurrency = splitRateDescription[RATE_FROM_CURRENCY_POSITION];
                string toRateCurrency = splitRateDescription[RATE_TO_CURRENCY_POSITION];
                string rateAsString = splitRateDescription[RATE_POSITION];
                decimal rateAsDouble = decimal.Parse(rateAsString);
                decimal rate = decimal.Round(rateAsDouble * PRECISION);
                decimal inverseRate = decimal.Round((PRECISION / rateAsDouble));
                helper.addEdge(fromRateCurrency, toRateCurrency, rate);
                helper.addEdge(toRateCurrency, fromRateCurrency, inverseRate);
            }

            decimal convertedValue = originalAmount;
            foreach (decimal rate in helper.getShortestPath(fromCurrency, toCurrency))
            {
                convertedValue = convertedValue * (rate / PRECISION);
            }
            convertedValue = decimal.Round(convertedValue);
        //System.Diagnostics.Debug.WriteLine("CHF;JPY : " + helper.getShortestPath("CHF", "JPY"));
        //    System.Diagnostics.Debug.WriteLine("JPY;CHF : " + helper.getShortestPath("JPY", "CHF"));
        //    System.Diagnostics.Debug.WriteLine("USD;INR : " + helper.getShortestPath("USD", "INR"));
        //    System.Diagnostics.Debug.WriteLine("INR;USD : " + helper.getShortestPath("INR", "USD"));
        //    System.Diagnostics.Debug.WriteLine("EUR;JPY : " + helper.getShortestPath("EUR", "JPY"));
            return string.Format("{0:0}",convertedValue);
        }

        private class GraphSearcher
        {
            private HashSet<string> _Nodes = new HashSet<string>();
            private Dictionary<string, HashSet<string>> _Edges = new Dictionary<string, HashSet<string>>();
            private Dictionary<string, decimal> _EdgeValues = new Dictionary<string, decimal>();
            private const string EDGE_FORMAT = "{0};{1}";

            public GraphSearcher() { }

            public void addEdge(string startNode, string endNode, decimal value)
            {
                _Nodes.Add(startNode);
                _Nodes.Add(endNode);
                HashSet<string> nodeNeighbours = null;
                if(!_Edges.TryGetValue(startNode, out nodeNeighbours)) {
                    nodeNeighbours = new HashSet<string>();
                    _Edges.Add(startNode, nodeNeighbours);
                }
                nodeNeighbours.Add(endNode);
                if (!_Edges.TryGetValue(endNode, out nodeNeighbours))
                {
                    _Edges.Add(endNode, new HashSet<string>());
                }
                _EdgeValues[String.Format(EDGE_FORMAT, startNode, endNode)] = value;
            }
            //This is using a simple Dijkstra algorithm
            //There might be some other better choices, but this will do
            public List<decimal> getShortestPath(string sourceNode, string destinatioNode)
            {
                Dictionary<string, string> previous = new Dictionary<string, string>();
                Dictionary<string, int> distances = new Dictionary<string, int>();
                foreach (string node in _Nodes) {
                    distances[node] = int.MaxValue;
                }
                distances[sourceNode] = 0;
                List<string> queue = new List<string>(_Nodes);
                while(queue.Count > 0) {
                    int smallestDistance = int.MaxValue;
                    string selectedNode = "";
                    foreach (string node in queue)
                    {
                        int nodeDistance = distances[node];
                        if (nodeDistance < smallestDistance)
                        {
                            smallestDistance = nodeDistance;
                            selectedNode = node;
                        }
                    }
                    //We have found our path so we can exit OR there is a maxvalue, which means no path exists between the 2 nodes
                    if (selectedNode == destinatioNode || smallestDistance == int.MaxValue)
                    {
                        break;
                    }
                    queue.Remove(selectedNode);
                    foreach (string neighbour in _Edges[selectedNode])
                    {
                        int distance = smallestDistance + 1;
                        if(distance < distances[neighbour]) {
                            distances[neighbour] = distance;
                            previous[neighbour] = selectedNode;
                        }
                    }
                }
                List<decimal> result = new List<decimal>();
                if (previous.ContainsKey(destinatioNode))
                {
                    string current = destinatioNode;
                    while (previous.ContainsKey(current))
                    {
                        string previousNode = previous[current];
                        decimal rate = _EdgeValues[string.Format(EDGE_FORMAT, previousNode, current)];
                        result.Add(rate);
                        current = previousNode;
                    }
                }
                return result;
            }

        }

    }


}
