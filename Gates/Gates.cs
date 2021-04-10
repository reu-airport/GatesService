using System;
using System.Collections.Generic;
using System.Linq;
using Gates.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Gates
{
    public class Gates
    {
        private IConnection connection;
        private IModel channel;

        private Uri uri = new Uri("amqps://avfepwdu:SS4fTAg36RK1hPQAUnyC6TH-4Mf3uyJo@fox.rmq.cloudamqp.com/avfepwdu"); //TODO

        private const string CheckInToGatesQueue = "GatePlacement";
        private const string VihiclesToGatesQueue = "PassengersRequest";
        private const string GatesToVihiclesQueue = "Passengers";

        private EventingBasicConsumer checkInConsumer;
        private EventingBasicConsumer vihicleConsumer;

        private Dictionary<PassengersInfo, int> passengers = new Dictionary<PassengersInfo, int>();

        public Gates()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "206.189.60.128",
                UserName = "guest",
                Password = "guest"
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(CheckInToGatesQueue, true, false, false, null);

            channel.QueueDeclare(VihiclesToGatesQueue, true, false, false, null);

            channel.QueueDeclare(GatesToVihiclesQueue, true, false, false);

            checkInConsumer = new EventingBasicConsumer(channel);
            checkInConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();

                GatePlacement placement = GatePlacement.Deserialize(body);

                var info = new PassengersInfo(placement.FlightId, placement.GateNum, placement.IsVip);

                if(passengers.ContainsKey(info))
                {
                    ++passengers[info];
                }
                else
                {
                    passengers.Add(info, 1);
                }

                Console.WriteLine($"[{DateTime.Now}] A new passenger has arrived:" +
                    $"\nPassenger ID:\t{placement.PassengerId}" +
                    $"\nIs VIP:\t{placement.IsVip}" +
                    $"\nFlight ID:\t{placement.FlightId}" +
                    $"\nGate number:\t{placement.GateNum}");
            };
            channel.BasicConsume(CheckInToGatesQueue, true, checkInConsumer);

            vihicleConsumer = new EventingBasicConsumer(channel);
            vihicleConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var request = PassengersRequest.Deserialize(body);

                Console.WriteLine($"[{DateTime.Now}] A new request has come");

                var info = new PassengersInfo(request.FlightId, request.GateNum, request.IsVip);
                int count = passengers[info];

                var response = new Passengers(request.VehicleId, request.GateNum, request.IsVip, count);

                channel.BasicPublish("", GatesToVihiclesQueue, null, response.Serialize());

                Console.WriteLine($"{DateTime.Now} The response has been sent");

                passengers.Remove(info);
            };
            channel.BasicConsume(VihiclesToGatesQueue, true, vihicleConsumer);
        }

        public void StartListening()
        {
            Console.WriteLine($"[{DateTime.Now}] Start listening");

            Console.ReadLine();

            channel.Close();
            connection.Close();
        }
    }

    public struct PassengersInfo
    {
        public PassengersInfo(Guid flightId, int gateNum, bool isVip)
        {
            FlightId = flightId;
            GateNum = gateNum;
            IsVip = isVip;
        }

        public Guid FlightId { get; init; }

        public int GateNum { get; init; }

        public bool IsVip { get; init; }
    }
}
