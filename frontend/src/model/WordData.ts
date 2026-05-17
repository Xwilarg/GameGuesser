export interface WordData
{
    name: WordBlockData
    description: WordBlockData
    shortDescription: WordBlockData
}

export interface WordBlockData
{
    foundIndexes: WordFoundInfo[] // Words found
    closeIndexes: WordIndexScoreInfo[] // Words we are close of
}

interface WordFoundInfo
{
    index: number
    word: string // Corrected version of the word
}

interface WordIndexScoreInfo
{
    index: number
    score: number // Between 0 and 1, how close the word is from the one we need to find
}