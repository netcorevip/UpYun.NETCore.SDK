

# ��ȡvs�Դ�MSBuild.exe·��

$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$path = &$vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | select-object -first 1
if ($path) {
    Write-Host $path
}


# ��ȡvs��װĿ¼��ʹ��vswhere.exe
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
&$vswhere -legacy -latest -property installationPath


# ͨ��ע����ȡVS2017��װĿ¼��ֻ������VS2017����ȡVS2019�Ͳ���
$VisualStudioVersion = "15.0";
$VSINSTALLDIR = $(Get-ItemProperty "Registry::HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7").$VisualStudioVersion;
Write-Host $VSINSTALLDIR
 



