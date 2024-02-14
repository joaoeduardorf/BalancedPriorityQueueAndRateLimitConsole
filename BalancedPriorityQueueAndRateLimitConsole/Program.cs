// See https://aka.ms/new-console-template for more information
using BalancedPriorityQueueAndRateLimitConsole;

Console.WriteLine("Fila balanceada e limite de envios");

// Obtendo caminho do arquivo de banco de dados
string absolutPath = Path.GetFullPath((@"..\..\..\Database1.mdf"));

// Montagem da string de conexão
string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={absolutPath};";

// Criando uma fila de mensagens prioritárias balanceadas
BalancedPriorityQueue<string> priorityQueue = new BalancedPriorityQueue<string>(connectionString);

// Atribuindo quantidade maxima de envios
var rateLimiter = new RateLimiter(5);

// Adicionando algumas mensagens com prioridades diferentes
priorityQueue.Enqueue("Mensagem 1", 2);
priorityQueue.Enqueue("Mensagem 2", 1);
priorityQueue.Enqueue("Mensagem 3", 3);
priorityQueue.Enqueue("Mensagem 4", 1);
priorityQueue.Enqueue("Mensagem 5", 5);
priorityQueue.Enqueue("Mensagem 6", 1);
priorityQueue.Enqueue("Mensagem 7", 2);
priorityQueue.Enqueue("Mensagem 8", 3);
priorityQueue.Enqueue("Mensagem 9", 6);
priorityQueue.Enqueue("Mensagem 10", 1);
priorityQueue.Enqueue("Mensagem 11", 1);
priorityQueue.Enqueue("Mensagem 12", 2);
priorityQueue.Enqueue("Mensagem 13", 2);

// Processando as mensagens da fila
while (priorityQueue.Count > 0)
{
    var message = priorityQueue.Dequeue();
    
    rateLimiter.MakeRequest(() =>
    {
        Console.WriteLine($"Fazendo requisição {message} - {DateTime.Now}");
        // Coloque aqui a lógica para fazer a requisição à API
    });
}