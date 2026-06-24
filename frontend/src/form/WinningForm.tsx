import { useLocalize } from "localize-react";
import type { GameData } from "../model/GameData";
import { getGameName } from "./MainForm";
import { useEffect, useState } from "react";
import { type RevealData } from "../model/RevealData";
import { Link } from "react-router";

interface WinningFormProps {
    state: GameData
    close: () => void

    endpoint: string
    lang: string
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

export default function WinningForm({ state, close, endpoint, lang }: WinningFormProps) {
    if (!state) {
        return <></>
    }

    const { translate } = useLocalize();
    const [ revealData, setRevealData ] = useState<RevealData | null>(null);

    useEffect(() => {
        fetch(`${endpoint}/api/reveal/${lang}`)
        .then(x => x.json())
        .then(x => setRevealData(x));
    }, []);

    return (
        <div className='modal is-flex flex-center-hor flex-center-ver winning-modal'>
            <div className="background-image" style={{
                backgroundImage: revealData ? `url('${revealData.backgroundImage}')` : ""
            }}></div>
            <div>
                <h1>{translate("win.title", { gamename: getGameName(), number: state.iteration })}</h1>
                <div className="is-flex flex-center-hor ">
                    <button onClick={() => {
                        let shareText = `${getGameName()} #${state.iteration}\n${translate("win.completion", { completion: getCompletion(state).toFixed(1) })}\n${getCompletionEmotes(state)}\n\n${window.location}`;
                        navigator.clipboard.writeText(shareText)
                        .then(() => window.alert(translate("win.text_copied")))
                        .catch(() => window.prompt(translate("win.copy_share"), shareText));
                    }}>Share</button>
                    <button onClick={close}>{translate("generic.close")}</button>
                    {
                        revealData
                            ? <Link to={revealData.steamLink} target="_blank" className="button">Store page</Link>
                            : <></>
                    }
                </div>
                {
                    revealData
                        ? <video src={revealData.videoLink} controls></video>
                        : <></>
                }
            </div>
        </div>
    )
}