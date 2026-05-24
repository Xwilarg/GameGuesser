import { useLocalize } from "localize-react";
import type { GameData } from "../model/GameData";
import { getGameName } from "./MainForm";

interface WinningFormProps {
    state: GameData
    close: () => void
}

function getCompletion(state: GameData) {
    let found = 0.0;
    for (let token of state.description.filter(x => x.needToBeGuessed)) {
        if (token.displayAsClose !== null) found += token.displayAsClose;
        else if (token.displayedWord !== null) found += 1;
    }

    return found / state.description.filter(x => x.needToBeGuessed).length * 100.0;
}

function getCompletionEmotes(state: GameData) {
    let str = "";
    for (let token of state.shortDescription.filter(x => x.needToBeGuessed)) {
        if (token.displayAsClose !== null) str += "🟧";
        else if (token.displayedWord !== null) str += "🟩";
        else str += "⬛";
    }
    return str;
}

export default function WinningForm({ state, close }: WinningFormProps) {
    if (!state) {
        return <></>
    }

    const { translate } = useLocalize();

    return (
        <div className='modal is-flex flex-center-hor flex-center-ver winning-modal'>
            <h1>{translate("win.title", { gamename: getGameName(), number: state.iteration })}</h1>
            <div className="is-flex">
                <button onClick={() => {
                    let shareText = `${getGameName()} #${state.iteration}\n${translate("win.completion", { completion: getCompletion(state).toFixed(1) })}\n${getCompletionEmotes(state)}\n\n${window.location}`;
                    navigator.clipboard.writeText(shareText)
                    .then(() => window.alert(translate("win.text_copied")))
                    .catch(() => window.prompt(translate("win.copy_share"), shareText));
                }}>Share</button>
                <button onClick={close}>{translate("generic.close")}</button>
            </div>
        </div>
    )
}