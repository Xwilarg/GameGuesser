Guess a game description one word at a time

Use the inputbar at the top to enter words, they will then appear in green or orange depending of their proximity

## Features
- A new word every day to guess, taken from a list of popular Steam games
- Automatically find adjacent words (cat -> cats, drink -> drinking)
- Available both in english and spanish

For list of upcoming features, please check https://github.com/users/Xwilarg/projects/18

## Contributing
Feel free to open a pull request or an issue for features you would like

To test the game locally, clone the repository then go in backend/GameGuesser.Backend/Database and follow the instructions [to create a database](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli#create-the-database), then move the generated Sqlite.db file to backend/GameGuesser.Backend

For the front, go in the frontend folder and run `npm i`, then `npm run dev`

To deploy, you can use the Dockerfile and docker-compose.yml.example (just rename it to docker-compose.yml.example and adjust what you might need to adjust) \
Move everything to your server then create a data/ folder at the root and move the Sqlite.db file inside

For the front, run `npm run build` instead

## Play

https://game.jinkou.org/

![Preview](/readme/preview.png)

## Credits

Game data taken from Steam \
English verbs from https://github.com/monolithpl/verb.forms.dictionary \
Spanish verbs from https://freedictionaryapi.com/ \
English and spanish word ajacency from https://www.datamuse.com/api/