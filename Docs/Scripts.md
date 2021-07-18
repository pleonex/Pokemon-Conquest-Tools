# Scripts

## Container format

| Offset | Size | Description                     |
| ------ | ---- | ------------------------------- |
| 0x00   | 0x04 | Signature `LSP\0`               |
| 0x04   | 0x04 | Version?                        |
| 0x08   | 0x04 | Unknown - Used in blocks2 setup |
| 0x0C   | 0x04 | Number of groups                |
| 0x10   | 0x04 | Num of functions - 1            |
| 0x14   | 0x04 | Relative data offset to block 2 |
| 0x18   | 0x04 | Unknown                         |
| 0x1C   | 0x04 | File size                       |
| 0x20   | ...  | Num of functions per group      |
| 0x100  | ...  | Function pointers in block1     |
| ....   | ...  | Function pointers in block2     |
| ...    | ...  | Block 1                         |
| ...    | ...  | Block 2                         |

## Pre-header format

| Offset | Size      | Description                     |
| ------ | --------- | ------------------------------- |
| 0x00   | 0x04      |                                 |
| 0x04   | 0x04      |                                 |
| 0x08   | 0x04      |                                 |
| 0x0C   | 0x04      | Switch only special for 4 and 5 |
| 0x10   | 0x04      |                                 |
| 0x14   | 0x04      | Skip things if not bit0         |
| 0x18   | 0x04      |                                 |
| 0x1C   | 0x04 \* 3 | Some 3x uint to loop            |

## Script format

| Offset | Size | Description                    |
| ------ | ---- | ------------------------------ |
| 0x00   | 0x04 | Signature `LSP\0`              |
| 0x04   | 0x04 | ?                              |
| 0x08   | 0x04 | If bit0, fields 20 and 24      |
| 0x0C   | 0x04 | Offset to instruction sections |
| 0x10   | 0x04 | Number of instructions         |
| 0x14   | 0x04 | ?                              |
| 0x18   | 0x04 | Offset to ?                    |
| 0x1C   | 0x04 | Offset to ?                    |
| 0x20   | 0x04 | Offset to ?                    |
| 0x24   | 0x04 | Offset to ?                    |

### Instructions

The script contains low-level instructions, similar to x86 assembly language. In
summary, you can jump, push, pop and move values from two stacks, do basic math
operations, comparison and call external functions.

| Opcode  | Mnemonic        | Description                                                          |
| ------- | --------------- | -------------------------------------------------------------------- |
| `0 x`   | `ENTER x`       | Enter a new stack frame by skipping `x` elements                     |
| `1 i t` | `SET.TYPE i t`  | Set the type of the stack value `r6 + i` to `t`                      |
| `2 i t` | `SET.TYPES i t` | Set the type of the stack values `(STACK[-1], STACK[-1] + i)` to `t` |
| `3 _ t` | `CAST t`        | Cast the last stack value to type `t`                                |
| `4 v t` | `PUSH v`        | Push the value `v` of type `t` into the stack                        |
| `5 v _` | `PUSH (6)v`     | Push the value `r6 + v` of type `6` into the stack                   |
| `C x y` | `SYSCALL x(y)`  | Call `x` with `y` stack values (pop) and write return value in stack |
| `E x y` |                 |

Valid stack value types:

- 0: integer
- 2: ?
- 1: boolean
- 5: void - no value.
- 6: ? (opcode 5)
- 1E: ?
