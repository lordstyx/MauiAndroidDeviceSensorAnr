namespace MauiApp1.DataGenerator;

public class BatteryInfo
{
    private bool _isBatteryWatched;
    private BatteryStatus? _lastKnownBatteryStatus;

    public event EventHandler<BatteryStatus>? StateChange;

    public void Start()
    {
        ToggleBatteryWatcher(Battery.Default, true);
    }

    public void Stop()
    {
        ToggleBatteryWatcher(Battery.Default, false);
    }

    private void ToggleBatteryWatcher(IBattery battery, bool enabled)
    {
        if (enabled != _isBatteryWatched)
        {
            if (enabled)
            {
                battery.BatteryInfoChanged += Battery_BatteryInfoChanged;
            }
            else
            {
                battery.BatteryInfoChanged -= Battery_BatteryInfoChanged;
            }
            _isBatteryWatched = enabled;
        }
    }

    private void Battery_BatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
    {
        var newStatus = new BatteryStatus
        {
            ChargeLevel = e.ChargeLevel,
            PowerSource = e.PowerSource,
            State = e.State,
            Timestamp = DateTimeOffset.UtcNow
        };
        if (!newStatus.IsSame(_lastKnownBatteryStatus))
        {
            _lastKnownBatteryStatus = newStatus;
            StateChange?.Invoke(sender, _lastKnownBatteryStatus);
        }
    }

    public BatteryStatus? GetLastKnownStatus()
    {
        return _lastKnownBatteryStatus;
    }
}

public class BatteryStatus
{
    public DateTimeOffset Timestamp { get; set; }
    /// <summary>
    /// The current charge level of the device from 0.0 to 1.0.
    /// </summary>
    public double ChargeLevel { get; set; }

    /// <summary>
    /// The charging state of the device if it can be determined.
    /// </summary>
    public BatteryState State { get; set; }

    /// <summary>
    /// The current power source for the device.
    /// </summary>
    public BatteryPowerSource PowerSource { get; set; }

    public bool IsSame(BatteryStatus? other)
    {
        if (other == null)
            return false;
        return ChargeLevel == other.ChargeLevel && State == other.State && PowerSource == other.PowerSource;
    }
}