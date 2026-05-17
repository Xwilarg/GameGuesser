import { forwardRef, type ReactElement } from "react";
import type { GameWordData } from "../model/GameData";
import type { WordBlockData } from "../model/WordData";

interface GuessAreaFormProps {
    data: GameWordData[]
    lastInput: WordBlockData | null
    id: string
}

const GuessAreaForm = forwardRef((
    { data, id, lastInput }: GuessAreaFormProps,
    _
) => {
    const blocks = data.reduce<GameWordData[][]>((acc, item) => {
        if (item.displayedWord === "\n") {
            acc.push([]);
        } else {
            acc[acc.length - 1].push(item);
        }
        return acc;
    }, [[]]);

    let display: ReactElement[] = [];

    let cumulativeIndex = 0;
    for (let i1 = 0; i1 < blocks.length; i1++)
    {
        let block: ReactElement[] = [];
        const line = blocks[i1];
        for (let i2 = 0; i2 < line.length; i2++)
        {
            const elem = line[i2];
            const key = `text-${id}-${i1}-${i2}`;
            if (lastInput !== null && lastInput.foundIndexes.some(x => x.index === cumulativeIndex))
            {
               block.push(<span key={key} className="found-word">{elem.displayedWord}</span>); // We just found this word, we highlight it
            }
            else if (elem.displayedWord === null)
            {
                block.push(<span key={key} title={elem.length.toString()} className="hidden-word" style={{ // Not found, we just display a black square
                    width: (elem.length * 14.4025) + "px"
                }}></span>);
            }
            else if (elem.displayAsClose !== null) {
                block.push(<span key={key} title={elem.length.toString()} style={{ // We found a similar word, we display it in orange, transparency depending of how close it is
                    color: `rgba(255, 166, 0, ${elem.displayAsClose * 0.8 + 0.2})`
                }}>{elem.displayedWord}</span>);
            }
            else block.push(<span key={key}>{elem.displayedWord}</span>); // Word found

            cumulativeIndex++;
        }
        display.push(
            <div key={`block-${id}-${i1}`} className="is-flex">
                {
                    block
                }
            </div>
        )
        cumulativeIndex++; // To take in consideration the \n
    }

    return <div className="container box word-list is-flex" id={id}>
    {
        display
    }
    </div>
});

export default GuessAreaForm;