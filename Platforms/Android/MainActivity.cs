using Android.App;
using Android.Content.PM;
using Android.OS;
using MauiApp1.DataGenerator;

namespace MauiApp1
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnStart()
        {
            base.OnStart();

            var _dataGenerator = new DeviceDataGenerator();
            _dataGenerator.Start();
        }
    }
}