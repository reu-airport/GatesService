using System;
using System.Text.Json;

namespace Gates.DTOs
{
    public class Passengers
    {
        public Passengers(Guid vehicleId, int gateNum, bool isVip, int count)
        {
            VehicleId = vehicleId;
            GateNum = gateNum;
            IsVip = isVip;
            Count = count;
        }

        public Guid VehicleId { get; init; }

        public int GateNum { get; init; }

        public bool IsVip { get; init; }

        public int Count { get; init; }

        public byte[] Serialize()
        {
            return JsonSerializer.SerializeToUtf8Bytes<Passengers>(this);
        }
    }
}
