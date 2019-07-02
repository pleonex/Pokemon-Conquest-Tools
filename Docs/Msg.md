# MSG file format

## Binary format

## Encryption

## Text format
Maximum text size: 0x270F (9999)

1. Read byte
2. If less than `0x20` --> it's a control code
    1. Valid control codes are: `01, 02, 05, 08, 09, 0A, 1B`. If invalid does nothing.

### Control code list
0x0122: Text start
0x0505: Text end

### Control 1
1. Reads a byte `b`
2. If b > 0x4A
3. If b == 0x4A
4. If b > 0x43 && b <= 0x46
5. Else quit

### Control 1B
1. Reads a byte `b`