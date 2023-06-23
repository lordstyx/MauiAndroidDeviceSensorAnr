using System.Numerics;

namespace MauiApp1.DataGenerator;

public class AccelerationInfo
{
    private DateTime _dataUpdatedAt;
    private float _ricoOne;
    private float _ricoTwo;
    private float _ricoFive;
    private float _ricoTen;
    private readonly CircularBuffer<float> _linearAccelerations;

    public AccelerationInfo()
    {
        _linearAccelerations = new CircularBuffer<float>(50);
    }

    public DateTime LastUpdatedAt => _dataUpdatedAt;

    public string GetAverage()
    {
        return _linearAccelerations.Count > 0
            ? $"{_linearAccelerations.Average()}"
            : string.Empty;
    }

    public event EventHandler<float>? Accelerating;

    public void Start()
    {
        ToggleAccelerometer(Accelerometer.Default, true);
    }

    public void Stop()
    {
        ToggleAccelerometer(Accelerometer.Default, false);
    }

    private float GetLinearAcceleration(Vector3? vector)
    {
        if (vector == null)
            return default;

        // Subtract 1 for the ever present gravitational pull (on earth)
        return vector.Value.X * vector.Value.X + vector.Value.Y * vector.Value.Y + vector.Value.Z * vector.Value.Z - 1;
    }

    private void ToggleAccelerometer(IAccelerometer accelerometer, bool enabled)
    {
        if (!accelerometer.IsSupported)
            return;

        if (enabled != accelerometer.IsMonitoring)
        {
            if (enabled)
            {
                // Turn on accelerometer
                accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                accelerometer.Start(SensorSpeed.UI);
            }
            else
            {
                // Turn off accelerometer
                accelerometer.Stop();
                accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            }
        }
    }

    private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        Console.WriteLine($"Accelerometer_ReadingChanged");
        var newValue = GetLinearAcceleration(e.Reading.Acceleration);

        if (_linearAccelerations.Count > 10)
        {
            _ricoOne = (newValue - _linearAccelerations.Peek()) / 1;
            _ricoTwo = (newValue - _linearAccelerations.Peek(1)) / 2;
            _ricoFive = (newValue - _linearAccelerations.Peek(5)) / 5;
            _ricoTen = (newValue - _linearAccelerations.Peek(10)) / 10;

            if (Math.Abs(_ricoOne) > 0.1 ||
                Math.Abs(_ricoTwo) > 0.1 ||
                Math.Abs(_ricoFive) > 0.1 ||
                Math.Abs(_ricoTen) > 0.1)
            {
                EventHandler<float> handler = Accelerating;
                handler?.Invoke(this, _ricoOne);
            }
        }

        _linearAccelerations.Enqueue(newValue);
        _dataUpdatedAt = DateTime.UtcNow;
    }
}
