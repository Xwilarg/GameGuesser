import { getGameName, type GameData } from "./MainForm";

interface WinningFormProps {
    state: GameData
    close: () => void
}

function getCompletion(state: GameData) {
    let found = 0.0;
    for (let token of state.description) {
        if (token.displayAsClose !== null) found += 0.5;
        else if (token.displayedWord !== null) found += 1;
    }
    return found / state.description.length * 100.0;
}

export default function WinningForm({ state, close }: WinningFormProps) {
    if (!state) {
        return <></>
    }

    return (
        <div className='modal is-flex flex-center-hor flex-center-ver winning-modal'>
            <h1>You finished the {getGameName()} #{state.iteration}</h1>
            <div className="is-flex">
                <button onClick={() => {
                    let shareText = `${getGameName()} #${state.iteration}\nCompletion: ${getCompletion(state).toFixed(1)}%\n\n${window.location}`;
                    window.prompt("Copy to share", shareText)
                }}>Share</button>
                <button onClick={close}>Close</button>
            </div>
        </div>
    )
}