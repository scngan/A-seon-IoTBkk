using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Emmellsoft.IoT.Rpi.SenseHat;
using Emmellsoft.IoT.Rpi.SenseHat.Fonts.SingleColor;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AEMBKK
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ISenseHat senseHat;
        DispatcherTimer timer = new DispatcherTimer();
        
        public MainPage()
        {
            
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            //initialize the timer
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds (3); //Put in count down
            timer.Start();
        }
        private async void Timer_Tick(object sender, object e)
        {
            // Trigger the timer
            timer.Stop();
            getTemperature();
            getHumidity();
            timer.Start();
        }
        public async void getTemperature()
        {
            try
            {
                //Get the value from the sensor          
                senseHat = await SenseHatFactory.GetSenseHat();
                senseHat.Sensors.HumiditySensor.Update();
                double temperature = double.Parse(senseHat.Sensors.Temperature.ToString());
                //rounding temperature
                int rtemp = (int)Math.Round(temperature);
                lblTemperature.Text = temperature.ToString();
                writeScreen(rtemp.ToString());
            }
            catch (Exception ex)
            {
                lblTemperature.Text = ex.ToString();
            }

        }
        public void writeScreen(string value)
        {
            //Write the text to the sensehat LED
            var tinyFont = new TinyFont();
            ISenseHatDisplay display = senseHat.Display;
            display.Clear();
            tinyFont.Write(display, value, Colors.Purple); // Change to the color you like ^^
            display.Update();
        }
        public async void passData()
        {
            AEMClass t = new AEMClass()
            {
                PartitionKey = "SCPi3v1key",
                RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString(),
                DeviceID = "SCPi2v1",
                Temperature = string.Format("{0:0.0}", lblTemperature.Text),
                Humidity = string.Format("{0:0.0}", lblHumidity.Text)
            };
            var serializedmsg = JsonConvert.SerializeObject(t);
            await communicator.SendDataToAzure(serializedmsg);
        }
        public async void getHumidity()
        {
            try
            {
                senseHat = await SenseHatFactory.GetSenseHat();
                senseHat.Sensors.HumiditySensor.Update();
                var humidity = senseHat.Sensors.Humidity.ToString();

                lblHumidity.Text = humidity.ToString();
            }
            catch (Exception ex)
            {
                lblHumidity.Text = ex.ToString();
            }
        }
    }
}
