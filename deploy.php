<?php `git pull`;

<?php

error_reporting(-1);
ini_set('display_errors', 'On');

if(isset($_GET["pokeName"]) && !empty($_GET["pokeName"])) {
    $content = file_get_contents("http://bulbapedia.bulbagarden.net/wiki/List_of_German_Pok%C3%A9mon_names");
    preg_match_all("/<a href=\"\/wiki\/.*_\(Pok%C3%A9mon\)\" title=\".* \(PokÃ©mon\)\">(.*)<\/a>/", $content, $english);
    preg_match_all("/<a href=\"http:\/\/www.pokewiki.de\/.*\" class=\"extiw\" title=\"de:.*\">(.*)<\/a>/", $content, $deutsch);

    $index = array_search(rtrim($_GET["pokeName"]), $english[1]);
    die($deutsch[1][$index]);
} else {
    die("api error");
}
