using System;

namespace FelicidApp.Model
{
    public class BandData:Data
    {
        public int HeartRate { get; }

        public BandData(string id, DateTime timestamp, int heartRate):base(id,timestamp)
        {
            HeartRate = heartRate;
        }
    }
}
