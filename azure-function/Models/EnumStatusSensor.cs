using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sensor_data_function.Models
{
    public enum EnumStatusSensor
    {
        Operating = 1,         // Máquina en operación
        Maintenance = 2,       // En mantenimiento
        Stopped = 3,           // Detenida
        Error = 4              // Error en la máquina
    }
}
