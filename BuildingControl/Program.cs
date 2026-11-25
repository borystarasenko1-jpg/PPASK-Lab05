using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BuildingControl.Components.Controls;
using BuildingControl.Components.Sensors;
using MQTTnet;
using MQTTnet.Protocol;

namespace BuildingControl
{
    internal static class Program
    {
        private static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var temperature = new TemperatureSensor();
            var humidity = new HumiditySensor();
            var motion = new MotionSensor();
            var heating = new Device("Опалення");
            var ventilation = new Device("Вентиляція");
            var lighting = new Device("Освітлення");
            var controller = new BuildingController(
                temperature, humidity, motion, heating, ventilation, lighting);

            var factory = new MqttClientFactory();
            using var client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883)
                .Build();
            await client.ConnectAsync(options);

            var readings = new[]
            {
                new { Temperature = 19.5, Humidity = 67.0, Motion = true },
                new { Temperature = 23.0, Humidity = 50.0, Motion = false },
                new { Temperature = 21.0, Humidity = 60.0, Motion = true }
            };

            for (int i = 0; i < 10; i++)
            {
                var sample = readings[i % readings.Length];
                temperature.Value = sample.Temperature;
                humidity.Value = sample.Humidity;
                motion.Value = sample.Motion;
                controller.CheckBuilding();

                var measuredAtUtc = DateTimeOffset.UtcNow;
                await Publish(client, "temperature", temperature.ReadValue(), "°C", measuredAtUtc);
                await Publish(client, "humidity", humidity.ReadValue(), "%", measuredAtUtc);
                await Publish(client, "motion", motion.ReadValue(), "bool", measuredAtUtc);

                Console.WriteLine(string.Format(CultureInfo.GetCultureInfo("uk-UA"),
                    "Дослід {0}. Температура {1:0.0} °C, вологість {2:0} %, рух {3}.",
                    i + 1, temperature.Value, humidity.Value, motion.Value ? "є" : "відсутній"));
                heating.ShowState();
                ventilation.ShowState();
                lighting.ShowState();
                Console.WriteLine();
                if (i < 9)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
            await client.DisconnectAsync();
        }

        private static async Task Publish<T>(IMqttClient client, string sensor, T value,
            string unit, DateTimeOffset measuredAtUtc)
        {
            var payload = JsonSerializer.Serialize(new { value, unit, measuredAtUtc });
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("building/1/sensors/" + sensor)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            await client.PublishAsync(message);
        }
    }
}
