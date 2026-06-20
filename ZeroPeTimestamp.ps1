param([string]$Path)
$bytes = [System.IO.File]::ReadAllBytes($Path)
$peOffset = [System.BitConverter]::ToUInt32($bytes, 0x3C)
$bytes[$peOffset+8] = 0
$bytes[$peOffset+9] = 0
$bytes[$peOffset+10] = 0
$bytes[$peOffset+11] = 0
[System.IO.File]::WriteAllBytes($Path, $bytes)
