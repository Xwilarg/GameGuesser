import { forwardRef } from "react";
import type { GameWordData } from "./MainForm";

interface GuessAreaFormProps {
    data: GameWordData[];
}

const GuessAreaForm = forwardRef((
    { data }: GuessAreaFormProps,
    _
) => {
    return <div className="container box is-flex" id="word-list">
    {
        data.map(x => {
            if (x.wasJustFound)
            {
                return <span className="found-word">{x.displayedWord}</span> // We just found this word, we highlight it
            }
            if (x.displayedWord === null)
            {
                return <span className="hidden-word" style={{ // Not found, we just display a black square
                    width: (x.length * 14.4025) + "px"
                }}></span>
            }
            if (x.displayAsClose !== null) {
                return <span style={{ // We found a similar word, we display it in orange, transparency depending of how close it is
                    color: `rgba(255, 166, 0, ${x.displayAsClose * 0.8 + 0.2})`
                }}>{x.displayedWord}</span>
            }
            return <span>{x.displayedWord}</span> // Word found
        })
    }
    </div>
});

export default GuessAreaForm;