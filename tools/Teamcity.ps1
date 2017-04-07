Param ($checkoutdir, $nunitversion, $browsersToRun)

$StarCounterDir = "$checkoutdir\sc"
$StarCounterWorkDirPath = "$StarCounterDir\starcounter-workdir"
$StarCounterRepoPath = "$StarCounterWorkDirPath\personal"
$StarCounterConfigPath = "$StarCounterDir\Configuration"

$WebsiteWwwPath = "$checkoutdir\Website\src\Website\wwwroot"
$WebsiteExePath = "$checkoutdir\Website\src\Website\bin\Debug\Website.exe"
$WebsiteProviderWwwPath = "$checkoutdir\Website\src\WebsiteProvider\wwwroot"
$WebsiteProviderExePath = "$checkoutdir\Website\src\WebsiteProvider\bin\Debug\WebsiteProvider.exe"

$WebsiteTestsPath = "$checkoutdir\Website\test\WebsiteProvider.Tests\bin\Debug\WebsiteProvider.Tests.dll"

$WebsiteArg = "--resourcedir=$WebsiteWwwPath $WebsiteExePath"
$WebsiteProviderArg = "--resourcedir=$WebsiteProviderWwwPath $WebsiteProviderExePath"

$WebsiteProviderHelperOneWwwPath = "$checkoutdir\Website\test\WebsiteProvider_AcceptanceHelperOne\wwwroot"
$WebsiteProviderHelperOneExePath = "$checkoutdir\Website\test\WebsiteProvider_AcceptanceHelperOne\bin\Debug\WebsiteProvider_AcceptanceHelperOne.exe"
$HelperOneArg = "--resourcedir=$WebsiteProviderHelperOneWwwPath $WebsiteProviderHelperOneExePath"

$WebsiteProviderHelperTwoWwwPath = "$checkoutdir\Website\test\WebsiteProvider_AcceptanceHelperTwo\wwwroot"
$WebsiteProviderHelperTwoExePath = "$checkoutdir\Website\test\WebsiteProvider_AcceptanceHelperTwo\bin\Debug\WebsiteProvider_AcceptanceHelperTwo.exe"
$HelperTwoArg = "--resourcedir=$WebsiteProviderHelperTwoWwwPath $WebsiteProviderHelperTwoExePath"

$NunitConsoleRunnerExePath = "$checkoutdir\Website\packages\NUnit.ConsoleRunner.$nunitversion\tools\nunit3-console.exe"
$NunitArg = "$WebsiteTestsPath --noheader --teamcity --params Browsers=$browsersToRun"

$StarExePath = "$StarCounterDir\star.exe"
$StarAdminExePath = "$StarCounterDir\staradmin.exe"

Function createXML($repoPath, $configPath)
{
	$fileContent = "<?xml version=`"1.0`" encoding=`"UTF-8`"?>
<service><server-dir>$repoPath</server-dir></service>"
	
	New-Item -Path $configPath -Name personal.xml -ItemType "file" -force -Value $fileContent | Out-Null
	return Test-Path $configPath\personal.xml
}

try 
{
	$createRepo = Start-Process -FilePath $StarExePath -ArgumentList "`@`@createrepo $StarCounterWorkDirPath" -PassThru -NoNewWindow -Wait
	if ($createRepo.ExitCode -eq 0)
	{
		$createXMLExitCode = createXML -repoPath $StarCounterRepoPath -configPath $StarCounterConfigPath
		if ($createXMLExitCode)
		{ 
			$Website = Start-Process -FilePath $StarExePath -ArgumentList $WebsiteArg -PassThru -NoNewWindow
			wait-process -id $Website.Id
			$WebsiteProvider = Start-Process -FilePath $StarExePath -ArgumentList $WebsiteProviderArg -PassThru -NoNewWindow
			wait-process -id $WebsiteProvider.Id
			$HelperOne = Start-Process -FilePath $StarExePath -ArgumentList $HelperOneArg -PassThru -NoNewWindow	 		
			wait-process -id $HelperOne.Id
			$HelperTwo = Start-Process -FilePath $StarExePath -ArgumentList $HelperTwoArg -PassThru -NoNewWindow	 		
			wait-process -id $HelperTwo.Id
			$Tests = Start-Process -FilePath $NunitConsoleRunnerExePath -ArgumentList $NunitArg -PassThru -NoNewWindow -Wait
			if($Tests.ExitCode -ge 0)
			{
				$KillStarcounter = Start-Process -FilePath $StarAdminExePath -ArgumentList "kill all" -PassThru -NoNewWindow -Wait
				if($KillStarcounter.ExitCode -eq 0) { exit(0) }
				else { exit(1) }
			}
			else { exit(1) }
		}
		else { exit(1) }
	}
	else { exit(1) }
} 
Catch 
{
	$ErrorMessage = $_.Exception.Message
	Write-Output $ErrorMessage
	exit(1)
}