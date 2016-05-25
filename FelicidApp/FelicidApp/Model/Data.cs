using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicidApp.Model
{
    public abstract class Data
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string DeviceId { get; set; }

        public Data(string id, DateTime timestamp)
        {
            Id = id;
            Timestamp = timestamp;
        }
    }
}
