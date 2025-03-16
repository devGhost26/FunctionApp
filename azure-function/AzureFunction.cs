using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using sensor_data_function.Models;
using System.Net.Http;

namespace azure_function
{
    public static class AzureFunction
    {
        [FunctionName("SensorDataFunction")]
        public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestMessage req,
          ILogger log)
        {
            log.LogInformation("Processing sensor data...");

            var requestBody = await req.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(requestBody))
            {
                return new BadRequestObjectResult("Request body is empty.");
            }

            try
            {
                var sensorData = JsonConvert.DeserializeObject<Sensor>(requestBody);

                // Validar datos del sensor
                var validationResult = ValidateSensorData(sensorData);
                if (!validationResult.IsValid)
                {
                    return new BadRequestObjectResult(validationResult.ErrorMessage);
                }

                // Asignar descripción al estado del sensor
                sensorData.Status = MapStatus(sensorData.Status.State);

                // Construir la respuesta
                var response = new
                {
                    MachineId = sensorData.MachineId,
                    Temperature = $"{sensorData.Temperature}°C",
                    Humidity = $"{sensorData.Humidity}%",
                    Vibration = $"{sensorData.Vibration} mm/s",
                    Pressure = $"{sensorData.Pressure} Pa",
                    EnergyConsumption = $"{sensorData.EnergyConsumption} kW",
                    Status = new
                    {
                        State = sensorData.Status.State.ToString(),
                        Description = sensorData.Status.Description
                    }
                };

                return new OkObjectResult(response);
            }
            catch (JsonException ex)
            {
                log.LogError($"Error deserializing request body: {ex.Message}");
                return new BadRequestObjectResult("Invalid JSON format.");
            }
            catch (Exception ex)
            {
                log.LogError($"Unexpected error: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Valida que los datos del sensor sean correctos.
        /// </summary>
        private static (bool IsValid, string ErrorMessage) ValidateSensorData(Sensor data)
        {
            if (data == null) return (false, "Sensor data is null.");

            if (string.IsNullOrEmpty(data.MachineId))
                return (false, "Machine ID is required.");

            if (!Enum.IsDefined(typeof(EnumStatusSensor), data.Status.State))
                return (false, "Invalid status ID.");

            return (true, null);
        }

        /// <summary>
        /// Mapea el estado del sensor a su respectiva descripción.
        /// </summary>
        private static StatusSensor MapStatus(EnumStatusSensor status)
        {
            return status switch
            {
                EnumStatusSensor.Operating => new StatusSensor { State = status, Description = "The machine is operating normally." },
                EnumStatusSensor.Maintenance => new StatusSensor { State = status, Description = "The machine is under maintenance." },
                EnumStatusSensor.Stopped => new StatusSensor { State = status, Description = "The machine is stopped." },
                EnumStatusSensor.Error => new StatusSensor { State = status, Description = "The machine has encountered an error!" },
                _ => new StatusSensor { State = status, Description = "Unknown status." }
            };
        }
    }
}
