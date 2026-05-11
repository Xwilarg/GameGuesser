<?php

header('Content-Type: application/json; charset=utf-8');
$shouldUpdate = json_decode(file_get_contents("../data/info.json"), true)["last_update"] != date("Ymd");

if ($shouldUpdate) {
    $json = json_decode(file_get_contents("../data/info.json"), true);
    $api = reset(json_decode(file_get_contents("https://store.steampowered.com/api/appdetails?appids=440&l=english"), true))["data"];

    $updatedData = json_encode([
        "last_update" => date("Ymd"),
        "iteration" => $json["iteration"] + 1,
        "game" => [
            "name" => $api["name"],
            "description" => $api["detailed_description"]
        ]
    ]);
    file_put_contents("../data/info.json", $updatedData);
    echo($updatedData);
}
else
{
    echo(file_get_contents("../data/info.json"));
}