using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;




var factory = new ConnectionFactory()
{
    HostName = "142.93.173.18",
    UserName = "admin",
    Password = "devintwitter"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();


channel.QueueDeclare(queue: "GPTQueue",
                           durable: true,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    PostMedicine medicineModel = JsonConvert.DeserializeObject<PostMedicine>(message);
    Console.WriteLine($"Recebido {medicineModel.NameDrugs} de {medicineModel.Substance}.");

    try
    {

        using var cxt = new MedicineCoreApiContext();
        PostMedicine medicine = cxt.Medicine.First(e => e.Id == medicineModel.Id);

        if (medicineModel == null)
        {
            throw new Exception("Id invalido");
        }


        //await ValidaMensagemComGPT(medicineModel.Description);

        cxt.Entry(medicineModel).State = EntityState.Modified;

        await cxt.SaveChangesAsync();
        Console.WriteLine("Salvou");

    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

};

channel.BasicConsume(queue: "GPTQueue",
                        autoAck: true,
                        consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();



//async Task ValidaMensagemComGPT(string mensagem, object sdk)
//{
//    var openai = new OpenAI.API("Key ChatGPT");

//    var completionResult = sdk.Completions.CreateCompletionAsStream(new CompletionCreateRequest()
//    {
//        Prompt = "Once upon a time",
//        MaxTokens = 500, // optional
//        Model = Models.Davinci
//    });
//    await foreach (var completion in completionResult)
//    {
//        if (completion.Successful)
//        {
//            Console.Write(completion.Choices.FirstOrDefault()?.Text);
//        }
//        else
//        {
//            if (completion.Error == null)
//            {
//                throw new Exception("Unknown Error");
//            }
//            Console.WriteLine($"{completion.Error.Code}: {completion.Error.Message}");
//        }
//    }
//    return;
//    //logica para tratar api gpt
//}

//internal class openAIService
//{
//}