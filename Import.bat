@echo off
cls

AmbitionText -i %CD%\text %CD%\encrypted_new
Decrypt -e encrypted_new 1MSG.DAT
RM -R encrypted_new