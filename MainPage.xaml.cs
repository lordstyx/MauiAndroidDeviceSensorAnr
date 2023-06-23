using MauiApp1.DataGenerator;
using System.ComponentModel;
using System.Globalization;

namespace MauiApp1
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        #region Model binding

        private string _deviceInfoText = string.Empty;
        public string DeviceInfoText
        {
            get => _deviceInfoText;
            set { if (_deviceInfoText != value) { _deviceInfoText = value; OnPropertyChanged(); } }
        }

        #endregion

        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;

            var timer = Application.Current.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => UpdateData();
            timer.Start();
        }

        private void UpdateData()
        {
            var location = App.DeviceDataStorage?.Location;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var geoString = $"Latitude: {location?.Latitude}{System.Environment.NewLine}"
                        + $"Longitude: {location?.Longitude}{System.Environment.NewLine}"
                        + $"Altitude: {location?.Altitude}m{System.Environment.NewLine}"
                        + $"AccuracyH: {location?.Accuracy}m{System.Environment.NewLine}"
                        + $"AccuracyV: {location?.VerticalAccuracy}m{System.Environment.NewLine}"
                        + $"Compass: {location?.Course}deg{System.Environment.NewLine}"
                        + $"Speed: {(location?.Speed?.ToString() ?? "unk ")}m/s{System.Environment.NewLine}"
                        + $"Speed: {((location?.Speed * 3.6)?.ToString() ?? "unk ")}km/h{System.Environment.NewLine}"
                        + $"Is moving: {location?.IsMoving()}{System.Environment.NewLine}"
                        + $"At: {location?.Timestamp}";

                    var lastUpdatedAtString = $"Last updated at:{System.Environment.NewLine}{DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)}";

                    DeviceInfoText = $"Geo:{System.Environment.NewLine}{geoString}{System.Environment.NewLine}{System.Environment.NewLine}{System.Environment.NewLine}{lastUpdatedAtString}";
                }
                catch (Exception ex) { }
            });
        }
    }
}