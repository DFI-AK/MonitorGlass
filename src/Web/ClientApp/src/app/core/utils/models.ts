export interface WindowsMetricDto {
  serverId: string;
  cpu: CPU;
  memory: Memory;
  diskDetails: DiskDetail[];
  networkDetails: NetworkDetail[];
  created: Date;
}

export interface CPU {
  cores: number;
  coreUsage: number;
  processCount: number;
  threadCount: number;
}

export interface DiskDetail {
  driveLetter: string;
  diskReadSpeedMBps: number;
  diskWriteSpeedMBps: number;
  diskIOPS: number;
  diskFreeSpaceGB: number;
  diskTotalSpaceGB: number;
  created: Date;
}

export interface Memory {
  totalMemoryMB: number;
  usedMemoryMB: number;
  availableMemoryMB: number;
}

export interface NetworkDetail {
  interfaceName: string;
  description: string;
  macAddress: string;
  iPv4Address: string;
  iPv6Address: null | string;
  isUp: boolean;
  speedMbps: number;
  bytesSent: number;
  bytesReceived: number;
  created: Date;
}
