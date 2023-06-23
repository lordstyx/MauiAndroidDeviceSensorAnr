using System.Collections.Concurrent;
using static MauiApp1.DataGenerator.DeviceDataGenerator;

namespace MauiApp1
{
    public partial class App : Application
    {
        public static DeviceDataMessage? DeviceDataStorage { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}