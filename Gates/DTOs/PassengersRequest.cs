using System;
using System.Text.Json;

namespace Gates.DTOs
{
    public class PassengersRequest
    {
        public PassengersRequest(Guid vehicleId, int gateNum, Guid flightId, bool isVip)
        {
            VehicleId = vehicleId;
            GateNum = gateNum;
            FlightId = flightId;
            IsVip = isVip;
        }

        public Guid VehicleId { get; init; }

        public int GateNum { get; init; }

        public Guid FlightId { get; init; }

        public bool IsVip { get; init; }

        public static PassengersRequest Deserialize(byte[] body)
        {
            return JsonSerializer.Deserialize<PassengersRequest>(body);
        }
    }
}
