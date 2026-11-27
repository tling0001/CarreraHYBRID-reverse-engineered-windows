using System;
using UnityEngine;

namespace CarreraHybrid.Platform
{
    public interface IPlatformServices
    {
        string PlatformName { get; }
        bool SupportsBluetoothLowEnergy { get; }
        bool SupportsFirmwareUpdate { get; }
        ICarreraBluetooth CreateBluetooth();
    }

    /// <summary>
    /// Explicit Windows capability boundary. A real WinRT GATT + Nordic DFU implementation
    /// will replace this provider after the exact Android protocol and hardware behavior are
    /// recovered. Returning a fake device or successful connection would be unsafe and misleading.
    /// </summary>
    public sealed class WindowsPlatformServices : IPlatformServices
    {
        public string PlatformName => "Windows";
        public bool SupportsBluetoothLowEnergy => false;
        public bool SupportsFirmwareUpdate => false;

        public ICarreraBluetooth CreateBluetooth()
        {
            throw new PlatformNotSupportedException(
                "Carrera BLE transport has not yet been ported. No simulated device is provided.");
        }
    }

    public static class PlatformServiceLocator
    {
        private static IPlatformServices current;
        public static IPlatformServices Current => current ?? (current = Create());

        private static IPlatformServices Create()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return new WindowsPlatformServices();
#else
            Debug.LogWarning("[CarreraHybrid] No platform provider registered for this target.");
            return new WindowsPlatformServices();
#endif
        }
    }
}
