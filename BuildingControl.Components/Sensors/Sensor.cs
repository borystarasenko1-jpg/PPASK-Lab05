namespace BuildingControl.Components.Sensors
{
    public abstract class Sensor<T>
    {
        public T Value { get; set; }

        protected Sensor(T initialValue)
        {
            Value = initialValue;
        }

        public T ReadValue()
        {
            return Value;
        }
    }
}
