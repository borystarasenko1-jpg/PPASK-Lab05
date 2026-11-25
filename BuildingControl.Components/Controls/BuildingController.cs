using BuildingControl.Components.Sensors;

namespace BuildingControl.Components.Controls
{
    public sealed class BuildingController
    {
        private const double TemperatureLimit = 21.0;
        private const double HumidityLimit = 60.0;
        private readonly TemperatureSensor temperature;
        private readonly HumiditySensor humidity;
        private readonly MotionSensor motion;
        private readonly Device heating;
        private readonly Device ventilation;
        private readonly Device lighting;

        public BuildingController(TemperatureSensor temperature, HumiditySensor humidity,
            MotionSensor motion, Device heating, Device ventilation, Device lighting)
        {
            this.temperature = temperature;
            this.humidity = humidity;
            this.motion = motion;
            this.heating = heating;
            this.ventilation = ventilation;
            this.lighting = lighting;
        }

        public void CheckBuilding()
        {
            heating.SetState(temperature.ReadValue() < TemperatureLimit);
            ventilation.SetState(humidity.ReadValue() > HumidityLimit);
            lighting.SetState(motion.ReadValue());
        }
    }
}
