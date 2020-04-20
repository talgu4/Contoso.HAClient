
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.HAClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of HTTP Client:");
            var cancellationTokenSource = new CancellationTokenSource();
            int input;

            while (!int.TryParse(Console.ReadLine(), out input))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter a valid number...");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Task[] tasks = CreateHttpClients(input, cancellationTokenSource.Token);
            if (tasks != null)
            {
                Console.WriteLine("Press any key to exit....");
                Console.ReadLine();
                Console.WriteLine("Start exit");

                cancellationTokenSource.Cancel();

                Task.WaitAll(tasks);
                Console.WriteLine("Bye bye");
            }
        }

        private static Task[] CreateHttpClients(int numOfClients, CancellationToken cancellationToken)
        {
            var tasks = new Task[numOfClients];
            var clientIdRandom = new Random();
            for (int i = 0; i < numOfClients; i++)
            {
                tasks[i] = Task.Run(async () => await SimulateClient(clientIdRandom.Next(1, numOfClients), cancellationToken));
            }
            return tasks;
        }

        private static async Task SimulateClient(int clientId, CancellationToken cancellationToken)
        {
            try
            {
                var client = new HttpClient();
                Console.WriteLine($"start SimulateClient {clientId}");
                var random = new Random();
                do
                {
                    var response = await client.GetAsync($"http://localhost:8080/?clientId= {clientId}");
                    Console.WriteLine($"clientId: {clientId} statusCode: {response.StatusCode} {DateTime.Now}");
                    await Task.Delay(random.Next(50, 1000));
                }
                while (!cancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
