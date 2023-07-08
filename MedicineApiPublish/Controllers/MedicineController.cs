using MedicineApiPublish.Models;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MedicineApiPublish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {

        private readonly ConnectionFactory _factory;
        public MedicineController()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "142.93.173.18",
                UserName = "admin",
                Password = "devintwitter"
            };
        }
        
        // POST api/<MedicineController>
        [HttpPost]
        public ActionResult Post([FromBody] MedicineViewsModel model)
        {
            if (model == null)
            {
                return BadRequest(ModelState);
            }
            var medicine = new PostMedicine()
            {
                NameDrugs = model.NameDrugs,
                Substance = model.Substance,
                Description = model.Description,
                PharmaceuticalIndustry = model.PharmaceuticalIndustry,
            };
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            try
            {
                PublishMedicine(medicine, channel);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
        private void PublishMedicine(PostMedicine model, IModel channel)
        {
            channel.QueueDeclare(queue: "Medicine",
                       durable: true,
                       exclusive: false,
                       autoDelete: false,
               arguments: null);

            var body = JsonConvert.SerializeObject(model);

            var modelBytes = Encoding.UTF8.GetBytes(body);
            
            channel.BasicPublish(exchange: string.Empty,
                    routingKey: "Medicine",
                    basicProperties: null,
                    body: modelBytes);
            //Console.WriteLine(" [x] Sent {0}", message);
        }
    }
}

