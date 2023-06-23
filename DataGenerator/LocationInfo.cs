namespace MauiApp1.DataGenerator;

public static class LocationExtensions
{
    public static bool IsMoving(this Location? location)
    {
        if (location == null)
            return false;

        return location.Speed > 0.5;
    }

    public static bool IsFastMoving(this Location? location)
    {
        if (location == null)
            return false;

        var fastSpeed = 16.6666667; // 60km/h
        return location.Speed > fastSpeed;
    }

    public static bool IsDifferent(this Location? oldValue, Location? newValue)
    {
        if (oldValue == null && newValue == null) // Deem these not the same, so we retry when two null locations are following eachother up.
            return true;
        if (oldValue == null && newValue != null)
            return true;
        if (oldValue != null && newValue == null)
            return true;

        if (oldValue == newValue)
        {
            Console.WriteLine($"IsSame: reference");
            return false;
        }

        if (oldValue.Timestamp == newValue.Timestamp)
        {
            Console.WriteLine($"IsSame: timestamp");
            return false;
        }

        if (Math.Abs((oldValue.Speed ?? 0) - (newValue.Speed ?? 0)) > 0.5)
        {
            Console.WriteLine($"IsDifferent: speed ({oldValue.Speed} vs {newValue.Speed})");
            return true;
        }

        // See https://wiki.gis.com/wiki/index.php/Decimal_degrees#Accuracy
        // 0.00005 should correspong to ~5m.
        if (Math.Abs(oldValue.Longitude - newValue.Longitude) > 0.00005)
        {
            Console.WriteLine($"IsDifferent: long ({oldValue.Longitude} vs {newValue.Longitude})");
            return true;
        }

        if (Math.Abs(oldValue.Latitude - newValue.Latitude) > 0.00005)
        {
            Console.WriteLine($"IsDifferent: lat ({oldValue.Latitude} vs {newValue.Latitude})");
            return true;
        }

        var distance = oldValue.CalculateDistance(newValue, DistanceUnits.Kilometers);
        if (distance >= 0.002) // Require a distance of >= 2m.
        {
            Console.WriteLine($"IsDifferent: distance ({oldValue.Longitude} vs {newValue.Longitude})");
            return true;
        }

        Console.WriteLine($"IsSame: other");
        return false;
    }
}

public class NewLocationInfo
{
    private Location? _lastReturnedLocation;
    private DateTime? _lastReturnedLocationAt;
    private bool _started;

    public event EventHandler<Location>? LocationChanged;
    public void Start()
    {
        ToggleGeoLocator(Geolocation.Default, true);
    }

    public void Stop()
    {
        ToggleGeoLocator(Geolocation.Default, false);
    }
    private async void ToggleGeoLocator(IGeolocation geolocator, bool enabled)
    {
        if (enabled != geolocator.IsListeningForeground)
        {
            if (enabled)
            {
                if (_started) // geolocator.IsListeningForeground seems to always return false, to keep track of it ourselves? Ideally this should get fixed.
                    return;

                try
                {
                    geolocator.LocationChanged += Geolocation_LocationChanged;
                    geolocator.ListeningFailed += Geolocation_ListeningFailed;

                    var request = new GeolocationListeningRequest(GeolocationAccuracy.Best, TimeSpan.Zero);
                    var success = await Geolocation.StartListeningForegroundAsync(request);
                    _started = true;
                }
                catch (PermissionException)
                {
                    ToggleGeoLocator(geolocator, false);
                }
            }
            else
            {
                geolocator.StopListeningForeground();
                geolocator.LocationChanged -= Geolocation_LocationChanged;
                geolocator.ListeningFailed -= Geolocation_ListeningFailed;
                _started = false;
            }
        }
    }

    private void Geolocation_LocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
    {
        if (!e.Location.IsDifferent(_lastReturnedLocation))
            return;

        _lastReturnedLocation = e.Location;
        _lastReturnedLocationAt = DateTime.UtcNow;

        LocationChanged?.Invoke(this, e.Location);
    }

    private void Geolocation_ListeningFailed(object? sender, GeolocationListeningFailedEventArgs e)
    {
        Console.WriteLine($"Geolocation_ListeningFailed: {e.Error.ToString()}");
    }
}