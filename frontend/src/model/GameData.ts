// Class returned when first connecting to the server, sharing the game state
export interface GameData
{
    isReady: boolean // Is game loaded or still loading in the backend
    progression?: number // Is game is not loading, percentage of initialization
    iteration: number // Which "day" we are on the game
    name: GameWordData[] // Game name
    description: GameWordData[] // Game description
}

export interface GameWordData
{
    displayedWord: string | null // Word to display, null is we show a black square instead
    displayAsClose: number | null // Word was not found but we are close
    length: number // Length of the word
    needToBeGuessed: boolean // Does the word need to be guessed or was it given at the game start
}