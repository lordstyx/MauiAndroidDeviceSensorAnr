namespace MauiApp1.DataGenerator;

public class DeviceDataGenerator
{
    private readonly BatteryInfo _batteryInfo;
    private readonly ConnectivityInfo _connectivityInfo;
    private readonly AccelerationInfo _accelerationInfo;
    private readonly NewLocationInfo _newLocationInfo;

    public DeviceDataGenerator()
    {
        _batteryInfo = new BatteryInfo();
        _connectivityInfo = new ConnectivityInfo();
        _accelerationInfo = new AccelerationInfo();
        _newLocationInfo = new NewLocationInfo();
    }

    public void Start()
    {
        _batteryInfo.StateChange += DeviceBatteryStateChanged;
        _batteryInfo.Start();
        _connectivityInfo.StateChange += DeviceConnectivityStateChanged;
        _connectivityInfo.Start();
        _accelerationInfo.Accelerating += DeviceStartedAccelerating;
        _accelerationInfo.Start();
        _newLocationInfo.LocationChanged += DeviceLocationChanged;
        _newLocationInfo.Start();
    }

    public void Stop()
    {
        _batteryInfo.Stop();
        _batteryInfo.StateChange -= DeviceBatteryStateChanged;
        _connectivityInfo.Stop();
        _connectivityInfo.StateChange -= DeviceConnectivityStateChanged;
        _accelerationInfo.Stop();
        _accelerationInfo.Accelerating -= DeviceStartedAccelerating;
        _newLocationInfo.Stop();
        _newLocationInfo.LocationChanged -= DeviceLocationChanged;
    }

    public DeviceDataMessage GetUpdateMessage(int msBetweenUpdates, Location? location = null, BatteryStatus? battery = null, ConnectivityStatus? connectivity = null, string? who = null)
    {
        return new DeviceDataMessage
        {
            Location = location,
            Battery = battery,
            ConnectivityStatus = connectivity,
            UpdatePeriod = msBetweenUpdates,
            Who = who
        };
    }

    private void DeviceStartedAccelerating(object? sender, float e)
    {
        Console.WriteLine($"DeviceBatteryStateChanged");
        if (Math.Abs(e) > 0.01)
        {
            Console.WriteLine($"DeviceBatteryStateChanged if");
            _newLocationInfo.Start();
            //_accelerationInfo.Stop();
        }
    }

    private void DeviceBatteryStateChanged(object? sender, BatteryStatus batteryStatus)
    {
        Console.WriteLine($"DeviceBatteryStateChanged");
        App.DeviceDataStorage = GetUpdateMessage(0, battery: batteryStatus, who: "battery");
    }

    private void DeviceConnectivityStateChanged(object? sender, ConnectivityStatus connectivityStatus)
    {
        Console.WriteLine($"DeviceConnectivityStateChanged");
        App.DeviceDataStorage = GetUpdateMessage(0, connectivity: connectivityStatus, who: "connectivity");
    }

    private void DeviceLocationChanged(object? sender, Location location)
    {
        Console.WriteLine($"DeviceLocationChanged");
        var updateMessage = GetUpdateMessage(0, location: location, who: "newLocation");

        App.DeviceDataStorage = updateMessage;
    }

    public class DeviceDataMessage
    {
        public Location? Location { get; set; }
        public BatteryStatus? Battery { get; set; }
        public ConnectivityStatus? ConnectivityStatus { get; set; }
        public int UpdatePeriod { get; set; }
        public string? Who { get; set; }
    }
}
