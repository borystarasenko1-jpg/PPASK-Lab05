using System;

namespace BuildingControl.Components.Controls
{
    public sealed class Device
    {
        public string Name { get; private set; }
        public bool IsOn { get; private set; }

        public Device(string name)
        {
            Name = name;
        }

        public void SetState(bool isOn)
        {
            IsOn = isOn;
        }

        public void ShowState()
        {
            Console.WriteLine("{0} — {1}", Name, IsOn ? "увімкнено" : "вимкнено");
        }
    }
}
