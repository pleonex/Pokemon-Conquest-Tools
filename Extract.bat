@echo off
cls

Decrypt -d MSG.DAT decrypted
AmbitionText -e %CD%\decrypted %CD%\text
RM -R decrypted