using System;
using System.Reflection;
using System.Text.Json;

namespace Gates.DTOs
{
    public class GatePlacement
    {
        public GatePlacement(Guid passengerId, Guid flightId, bool hasBaggage, bool isVip, int gateNum)
        {
            PassengerId = passengerId;
            FlightId = flightId;
            HasBaggage = hasBaggage;
            IsVip = isVip;
            GateNum = gateNum;
        }

        public Guid PassengerId { get; init; }

        public Guid FlightId { get; init; }

        public bool HasBaggage { get; init; }

        public bool IsVip { get; init; }

        public int GateNum { get; init; }

        public static GatePlacement Deserialize(byte[] body)
        {
            return JsonSerializer.Deserialize<GatePlacement>(body);
        }
    }
}
