using System;

namespace CarreraHybrid.Bootstrap
{
    /// <summary>Facts recovered from the APK's Android bridge; not a complete drive protocol.</summary>
    public static class AndroidProtocolConstants
    {
        public static readonly Guid NordicUartWriteCharacteristic =
            new Guid("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
        public const int DrivePacketLength = 20;
        public const byte DrivePacketHeader = 0xAF;
        public const int DriveSendIntervalMilliseconds = 50;
        public const int StaleTargetTimeoutMilliseconds = 500;
    }
}
