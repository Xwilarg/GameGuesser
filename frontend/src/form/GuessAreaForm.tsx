import { forwardRef, useState, type ReactElement } from "react";
import type { GameWordData } from "../model/GameData";
import type { WordBlockData } from "../model/WordData";

interface GuessAreaFormProps {
    data: GameWordData[]
    lastInput: number[] | null
    id: string
}

const GuessAreaForm = forwardRef((
    { data, id, lastInput }: GuessAreaFormProps,
    _
) => {
    const [highlights, setHighlights] = useState<Array<number>>([]);

    const blocks = data.reduce<GameWordData[][]>((acc, item) => {
        if (item.displayedWord === "\n") {
            acc.push([]);
        } else {
            acc[acc.length - 1].push(item);
        }
        return acc;
    }, [[]]);

    let display: ReactElement[] = [];

    function toggleHighlight(index: number) {
        setHighlights(x => {
            if (x.includes(index)) {
                let arr = [...x];
                arr.splice(arr.indexOf(index), 1)
                return arr
            }
            return [...x, index];
        })
    }

    let cumulativeIndex = 0;
    for (let i1 = 0; i1 < blocks.length; i1++)
    {
        let block: ReactElement[] = [];
        const line = blocks[i1];
        for (let i2 = 0; i2 < line.length; i2++)
        {
            const currIndex = cumulativeIndex;
            const elem = line[i2];
            const key = `text-${id}-${i1}-${i2}`;
            if (lastInput !== null && lastInput.some(x => x === cumulativeIndex))
            {
                // We just found this word, we highlight it
                block.push(
                    <span key={key} className="word found-word">
                        {elem.displayedWord}
                    </span>
                );
            }
            else if (elem.displayedWord === null)
            {
                // Not found, we just display a black square
                block.push(
                    <span key={key} className="word hidden-word" style={{
                        width: (elem.length * 14.4025) + "px"
                    }} onClick={() => toggleHighlight(currIndex)}>
                        { highlights.includes(currIndex) ? <span className="hint-length">{elem.length}</span> : <></> }
                    </span>
                );
            }
            else if (elem.displayAsClose !== null) {
                // We found a similar word, we display it in orange, transparency depending of how close it is
                block.push(
                    <span key={key} className="word" style={{
                        color: `rgba(255, 166, 0, ${elem.displayAsClose * 0.8 + 0.2})`
                    }} onClick={() => toggleHighlight(currIndex)}>
                        {elem.displayedWord}
                        { highlights.includes(currIndex) ? <span className="hint-length">{elem.length}</span> : <></> }
                    </span>
                );
            }
            else {
                // Word found
                block.push(<span key={key} className="word">{elem.displayedWord}</span>);
            }

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