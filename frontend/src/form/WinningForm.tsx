import { useLocalize } from "localize-react";
import type { GameData } from "../model/GameData";
import { getGameName } from "./MainForm";
import { useEffect, useRef, useState } from "react";
import { type RevealData } from "../model/RevealData";
import { Link } from "react-router";
import Hls from "hls.js";
import type { WordHistoryData } from "../model/WordHistoryData";

interface WinningFormProps {
    state: GameData
    history: WordHistoryData[]
    close: () => void

    endpoint: string
    lang: string
}

export default function WinningForm({ state, history, close, endpoint, lang }: WinningFormProps) {
    if (!state) {
        return <></>
    }

    const { translate } = useLocalize();
    const [ revealData, setRevealData ] = useState<RevealData | null>(null);
    const videoRef = useRef<HTMLVideoElement>(null);

    useEffect(() => {
        fetch(`${endpoint}/api/reveal/${lang}`)
        .then(x => x.json())
        .then(x => setRevealData(x));
    }, []);

    useEffect(() => {
        if (!revealData) return;

        const video = videoRef.current;

        const hls = new Hls();
        hls.loadSource(revealData.videoLink);
        hls.attachMedia(video!);
    }, [ revealData ])

    return (
        <div className='modal is-flex flex-center-hor flex-center-ver winning-modal'>
            <div className="background-image" style={{
                backgroundImage: revealData ? `url('${revealData.backgroundImage}')` : ""
            }}></div>
            <div>
                <h1>{translate("win.title", { gamename: getGameName(), number: state.iteration })}</h1>
                <div className="is-flex flex-center-hor ">
                    <button onClick={() => {
                        let shareText = `${getGameName()} #${state.iteration}\n${translate("win.completion_count", { tries: history.length })}\n\n${window.location}`;
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
                <div className="is-flex flex-center-hor">
                {
                    revealData
                        ? <video ref={videoRef} controls></video>
                        : <></>
                }
                </div>
            </div>
        </div>
    )
}