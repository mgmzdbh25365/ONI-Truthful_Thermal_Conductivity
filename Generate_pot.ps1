$write='msgid ""
msgstr ""
"Application: Oxygen Not Included"
"POT Version: 2.0"

'

Write-Output $write
foreach($line in [System.IO.File]::ReadLines("./Strings.cs"))
{
	if (($line.indexof("LocString ")) -gt -1){
    $name=$line.SubString($line.indexof("LocString ")+10)
	$value=$name.SubString($name.indexof('"'))
	$name=$name.SubString(0,$name.indexof('=')-1)
	$value=$value.SubString(0,$value.lastindexof('"')+1)
	
	$result="#. ONI_Truthful_Thermal_Conductivity.STRINGS."+$name
    Write-Output $result
	$write+=$result+"`n"
	$result='msgctxt "ONI_Truthful_Thermal_Conductivity.STRINGS.'+$name+'"'
    Write-Output $result
	$write+=$result+"`n"
	$result="msgid "+$value
    Write-Output $result
	$write+=$result+"`n"
	$result='msgstr ""'+"`n"
    Write-Output $result
	$write+=$result+"`n"
	
	}
}
[Console]::ReadKey()
Set-Content -Path .\strings_template.pot -Value ($write)

Get-Content -Path .\strings_template.pot
[Console]::ReadKey()