using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace lab09
{
    internal class Program
    {
        static StreamWriter writer = new StreamWriter("result.txt");
        public static async Task average(string csv, string name)
        {
            double result = 0;
            double a = 0, b = 0;
            string[] lines = csv.Split('\n');

            if (lines.Length > 0)
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split(',');
                    if (fields.Length > 4)
                    {
                        if (fields[2] != "null" && fields[3] != "null")
                        {
                            a = Convert.ToDouble(fields[2].Replace('.', ','));
                            b = Convert.ToDouble(fields[3].Replace('.', ','));
                          
                            result = result + (a + b) / 2;
                        }
                    }
                }
            
            writer.WriteLine($"{name}::{result / lines.Length}");
        }
        static async Task Main(string[] args)
        {
            
            HttpClient client = new HttpClient();
            List<Task> results = new List<Task>();
            string response, name;

            using (StreamReader reader = new StreamReader("ticker.txt"))
            {
                while (!reader.EndOfStream)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        name = reader.ReadLine();
                        response = await client.GetStringAsync($"https://query1.finance.yahoo.com/v7/finance/download/{name}?period1=1669323600&period2=1660608000&interval=1d&events=history&includeAdjustedClose=true");
                        if (response.Length > 0)
                            results.Add(average(response, name));
                    }
                    catch (Exception e)
                    { Console.WriteLine(e); }
                }

                Task.WaitAll(results.ToArray());
            }
        }
    }
}
