namespace Raygun.Messages
{
    using System.Collections.Generic;

    public class RaygunEnvironmentMessage
    {
        public int ProcessorCount { get; set; }

        public string OSVersion { get; set; }

        public string PackageVersion { get; set; }

        public string Cpu { get; set; }

        public string Architecture { get; set; }

        //public ulong TotalVirtualMemory { get; set; }
        //public ulong AvailableVirtualMemory { get; set; }

        public List<double> DiskSpaceFree { get; set; }

        //public ulong TotalPhysicalMemory { get; set; }
        //public ulong AvailablePhysicalMemory { get; set; }

        public double UtcOffset { get; set; }

        public string Locale { get; set; }
    }
}