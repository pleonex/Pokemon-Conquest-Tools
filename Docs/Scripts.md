# Scripts

## Format

| Offset | Size    | Description        |
| ------ | ------- | ------------------ |
| 0x00   | 0x04    | Signature `LSP\0`  |
| 0x04   | 0x04    | Unknown            |
| 0x08   | 0x04    | Unknown            |
| 0x0C   | 0x04    | Number of sections |
| 0x10   | 0x04    | Num items - 1 / 8  |
| 0x14   |         | Section? size      |
| 0x18   |         | Unknown            |
| 0x1C   | 0x04    | File size          |
| 0x20   | 4\*0x1B | unknown sizes      |
