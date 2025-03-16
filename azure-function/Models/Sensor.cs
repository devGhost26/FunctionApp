using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sensor_data_function.Models
{
    class Sensor
    {
        public string MachineId { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Vibration { get; set; }
        public string Pressure { get; set; }
        public string EnergyConsumption { get; set; }
        public StatusSensor Status { get; set; }

        public Sensor()
        {
            Status = new StatusSensor();
        }
    }
}
