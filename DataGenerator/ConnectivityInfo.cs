namespace MauiApp1.DataGenerator;

public class ConnectivityInfo
{
    private bool _isConnectivityWatched;
    private ConnectivityStatus? _lastKnownNetworkStatus;

    public event EventHandler<ConnectivityStatus>? StateChange;

    public void Start()
    {
        ToggleConnectivityWatcher(Connectivity.Current, true);
        StateChange?.Invoke(this, GetConnectivityStatus(Connectivity.Current.NetworkAccess, Connectivity.Current.ConnectionProfiles));
    }

    public void Stop()
    {
        ToggleConnectivityWatcher(Connectivity.Current, false);
    }

    private void ToggleConnectivityWatcher(IConnectivity connectivity, bool enabled)
    {
        if (enabled != _isConnectivityWatched)
        {
            if (enabled)
            {
                try
                {
                    connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
                }
                catch (PermissionException)
                {
                    ToggleConnectivityWatcher(connectivity, false);
                }
            }
            else
            {

                connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            }
            _isConnectivityWatched = enabled;
        }
    }

    private void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        var newStatus = GetConnectivityStatus(e.NetworkAccess, e.ConnectionProfiles);
        if (!newStatus.IsSame(_lastKnownNetworkStatus))
        {
            _lastKnownNetworkStatus = newStatus;
            StateChange?.Invoke(sender, _lastKnownNetworkStatus);
        }
    }

    private ConnectivityStatus GetConnectivityStatus(NetworkAccess networkAccess, IEnumerable<ConnectionProfile> connectionProfiles)
    {
        return new ConnectivityStatus
        {
            IsConnected = networkAccess == NetworkAccess.Internet,
            NetworkType = string.Join(',', connectionProfiles.OrderBy(x => x).Select(x => x.ToString()))
        };
    }
}

public class ConnectivityStatus
{
    public bool IsConnected { get; set; }
    public string NetworkType { get; set; }

    public bool IsSame(ConnectivityStatus? other)
    {
        if (other == null)
            return false;

        return IsConnected == other.IsConnected && string.Equals(NetworkType, other.NetworkType);
    }
}
