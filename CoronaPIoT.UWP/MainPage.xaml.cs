using Sensors.Dht;
using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CoronaPIoT
{
    public sealed partial class MainPage : Page
    {
        private const int DHTPIN = 18;
        private IDht dht = null;
        private GpioPin dhtPin = null;
        private DispatcherTimer sensorTimer = new DispatcherTimer();
        private int[] _pinNumbers = new[] { 22, 9, 19 };
        private GpioPin[] _pins = new GpioPin[3];

        public MainPage()
        {
            this.InitializeComponent();

            InitGPIO();

            dhtPin = GpioController.GetDefault().OpenPin(DHTPIN, GpioSharingMode.Exclusive);
            dht = new Dht11(dhtPin, GpioPinDriveMode.Input);
            sensorTimer.Interval = TimeSpan.FromSeconds(1);
            sensorTimer.Tick += sensorTimer_Tick;
            sensorTimer.Start();

            temperatureMeter.Value = "OFF";
            humidityMeter.Value = "OFF";
            monoxidMeter.Value = "OFF";
        }

        private void sensorTimer_Tick(object sender, object e)
        {
            ReadSensorAsync();
        }

        private async void ReadSensorAsync()
        {
            double temp = 0;
            double humidity = 0;

            DhtReading reading = await dht.GetReadingAsync().AsTask();
            if (reading.IsValid)
            {
                temp = reading.Temperature;
                humidity = reading.Humidity;

                temperatureMeter.Value = $"{temp:0.0}";
                humidityMeter.Value = $"{humidity:0}";

                if (humidity >= 70)
                {
                    _pins[2].Write(GpioPinValue.High);
                    _pins[0].Write(GpioPinValue.Low);
                    humidityError.Text = "To high";
                } else if (humidity <= 20)
                {
                    _pins[2].Write(GpioPinValue.High);
                    _pins[0].Write(GpioPinValue.Low);
                    humidityError.Text = "To low";
                }
                else if (humidity < 60 && humidity > 40)
                {
                    _pins[2].Write(GpioPinValue.Low);
                    _pins[0].Write(GpioPinValue.High);
                    humidityError.Text = "OK";
                }
            }
        }

        private bool InitGPIO()
        {
            var gpio = GpioController.GetDefault();
            if (gpio == null)
                return false;
            for (int i = 0; i < 3; i++)
            {
                _pins[i] = gpio.OpenPin(_pinNumbers[i]);
                _pins[i].Write(GpioPinValue.High);
                _pins[i].SetDriveMode(GpioPinDriveMode.Output);
            }
            return true;
        }

    }
}
