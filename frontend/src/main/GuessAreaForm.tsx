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
            if (x.displayedWord === null)
            {
                return <span className="hidden-word" style={{
                    width: (x.length * 14.4025) + "px"
                }}></span>
            }
            if (x.displayAsClose !== null) {
                return <span style={{
                    color: `rgba(255, 166, 0, ${x.displayAsClose * 0.8 + 0.2})`
                }}>{x.displayedWord}</span>
            }
            return <span>{x.displayedWord}</span>
        })
    }
    </div>
});

export default GuessAreaForm;