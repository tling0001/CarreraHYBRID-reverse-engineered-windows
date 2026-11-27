using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarreraHybrid.Platform
{
    /// <summary>Platform-neutral contract matching the responsibilities of the Android BLE bridge.</summary>
    public interface ICarreraBluetooth : IAsyncDisposable
    {
        event EventHandler<BluetoothConnectionChangedEventArgs> ConnectionChanged;
        event EventHandler<BluetoothNotificationEventArgs> NotificationReceived;
        Task<BluetoothDeviceInfo[]> ScanAsync(TimeSpan duration, CancellationToken cancellationToken);
        Task ConnectAsync(string deviceId, CancellationToken cancellationToken);
        Task DisconnectAsync(CancellationToken cancellationToken);
        Task WriteDrivePacketAsync(ReadOnlyMemory<byte> packet, CancellationToken cancellationToken);
        bool IsConnected { get; }
    }

    public sealed class BluetoothDeviceInfo
    {
        public string Id { get; }
        public string Name { get; }
        public BluetoothDeviceInfo(string id, string name) { Id = id; Name = name; }
    }

    public sealed class BluetoothConnectionChangedEventArgs : EventArgs
    {
        public bool Connected { get; }
        public string DeviceId { get; }
        public string Error { get; }
        public BluetoothConnectionChangedEventArgs(bool connected, string deviceId, string error = null)
        { Connected = connected; DeviceId = deviceId; Error = error; }
    }

    public sealed class BluetoothNotificationEventArgs : EventArgs
    {
        public Guid Characteristic { get; }
        public byte[] Payload { get; }
        public BluetoothNotificationEventArgs(Guid characteristic, byte[] payload)
        { Characteristic = characteristic; Payload = payload ?? Array.Empty<byte>(); }
    }
}
