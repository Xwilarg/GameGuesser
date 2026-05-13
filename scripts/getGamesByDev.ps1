$ErrorActionPreference = "Stop"

$name = Read-Host 'Enter publisher name'
$name = $name.Replace(" ", "+")

$info = Invoke-RestMethod -Uri "https://store.steampowered.com/search/results/?publisher=${name}&json=1"

if ($info.items.length -eq 0) {
    Write-Error "No developer found with the corresponding name"
}

foreach ($game in $info.items)
{
    $match = $game.logo -match 'steam\/apps\/([0-9]+)'
    if ($match) {
        $id = $Matches[1]
        $gameInfo = Invoke-RestMethod -Uri "https://store.steampowered.com/api/appdetails?appids=${id}"
        if ($gameInfo.$id.success)
        {
            if ($gameInfo.$id.data.type -eq "game")
            {
                Write-Output "$id ($($game.name))"
            }
        }
        else
        {
            Write-Warning "Failed to get information for ${id}"
        }
    } else {
        Write-Warning "Failed to find ID in URL for $($game.name)} $($game.logo)"
    }
}
