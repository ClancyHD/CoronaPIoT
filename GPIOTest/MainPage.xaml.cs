using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GPIOTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int _currentLight;
        private DispatcherTimer _timer;
        private int[] _pinNumbers = new[] { 22, 9, 19 };
        private GpioPin[] _pins = new GpioPin[3];
        public MainPage()
        {
            this.InitializeComponent();
            if (InitGPIO())
                InitTimer();
        }
        private void InitTimer()
        {
            var intervals = new[] { 10, 20, 30 };
            var lights = new[] { RedLed, YellowLed, GreenLed };
            _currentLight = 2;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                lights[_currentLight].Opacity = 0.5;
                _pins[_currentLight].Write(GpioPinValue.High);
                _currentLight = _currentLight == 2 ? 0 : _currentLight + 1;
                lights[_currentLight].Opacity = 1.0;
                _pins[_currentLight].Write(GpioPinValue.Low);
                _timer.Interval = TimeSpan.FromMilliseconds(intervals[_currentLight]);
                _timer.Start();
            };
            _timer.Start();
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
