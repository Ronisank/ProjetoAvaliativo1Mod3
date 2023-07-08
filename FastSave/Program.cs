using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using FastSave.Models;
using Microsoft.AspNetCore.Connections;
using static System.Net.Mime.MediaTypeNames;

var factory = new ConnectionFactory()
{
    HostName = "142.93.173.18",
    UserName = "admin",
    Password = "devintwitter"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();


channel.QueueDeclare(queue: "FastSaveQueue",
                           durable: true,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    PostMedicineViewModel medicineModel = JsonConvert.DeserializeObject<PostMedicineViewModel>(message);
    Console.WriteLine($"Recebido {medicineModel.NameDrugs} de {medicineModel.Substance}.");
    //SaveMedicine(medicine);

    try
    {

       await SaveMedicine(medicineModel);
       await PublishGPTQueue(channel, medicineModel);

    }
    catch (Exception e)
    {
        Console.WriteLine("falha ao salvar");
    }

};

channel.BasicConsume(queue: "FastSaveQueue",
                        autoAck: true,
                        consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

async Task PublishGPTQueue(IModel channel, PostMedicineViewModel medicineModel)
{


    channel.QueueDeclare(queue: "GPTQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

    var stringJson = JsonConvert.SerializeObject(new
    {
        Id = 0,
        //NameDrugs = medicineModel.NameDrugs,
        //Substance = medicineModel.Substance,
        //PharmaceuticalIndustry = medicineModel.PharmaceuticalIndustry,
        //Description = medicineModel.Description,
        
    });

    var body = Encoding.UTF8.GetBytes(stringJson);

    channel.BasicPublish(exchange: string.Empty,
                            routingKey: "GPTQueue",
                            basicProperties: null,
                            body: body);

}


async Task SaveMedicine(PostMedicineViewModel medicine)
{
    using var ctx = new MedicineCoreApiContext();
    ctx.Medicine.Add(new PostMedicine
    {
        Id = 0,
        NameDrugs = medicine.NameDrugs,
        Substance = medicine.Substance,
        Description = medicine.Description,
        PharmaceuticalIndustry = medicine.PharmaceuticalIndustry

    });
    await ctx.SaveChangesAsync();
    


}