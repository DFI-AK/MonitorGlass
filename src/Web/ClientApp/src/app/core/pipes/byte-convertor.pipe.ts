import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'byteConvertor',
  standalone: true,
})
export class ByteConvertorPipe implements PipeTransform {
  transform(
    value: number,
    from: 'b' | 'kb' | 'mb' | 'gb' = 'b',
    to: 'b' | 'kb' | 'mb' | 'gb' = 'kb'
  ): string {
    if (value == null || isNaN(value)) return '0 B';

    // Convert everything to bytes first
    let bytes = value;
    switch (from.toLowerCase()) {
      case 'kb':
        bytes = value * 1024;
        break;
      case 'mb':
        bytes = value * 1024 * 1024;
        break;
      case 'gb':
        bytes = value * 1024 * 1024 * 1024;
        break;
    }

    // Convert bytes to target unit
    let converted = bytes;
    let suffix = to.toUpperCase();

    switch (to.toLowerCase()) {
      case 'kb':
        converted = bytes / 1024;
        break;
      case 'mb':
        converted = bytes / (1024 * 1024);
        break;
      case 'gb':
        converted = bytes / (1024 * 1024 * 1024);
        break;
    }

    return `${converted.toFixed(2)} ${suffix}`;
  }
}
