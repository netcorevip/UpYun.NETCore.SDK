

# 获取vs自带MSBuild.exe路径

$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$path = &$vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | select-object -first 1
if ($path) {
    Write-Host $path
}


# 获取vs安装目录，使用vswhere.exe
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
&$vswhere -legacy -latest -property installationPath


# 通过注册表获取VS2017安装目录，只能用于VS2017，获取VS2019就不行
$VisualStudioVersion = "15.0";
$VSINSTALLDIR = $(Get-ItemProperty "Registry::HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7").$VisualStudioVersion;
Write-Host $VSINSTALLDIR
 



