import { useEffect, useRef, useState } from "react"
import GuessAreaForm from "./GuessAreaForm"
import { Link } from "react-router"
import WinningForm from "./WinningForm"
import RulesForm from "./RulesForm"
import type { LastWordInfo } from "../model/LastWordInfo"
import type { GameData, GameWordData } from "../model/GameData"
import type { WordBlockData, WordData } from "../model/WordData"

function getEndpoint(): string
{
    if (window.location.host.startsWith("localhost")) return "http://localhost:5174";
    return "";
}

export function getGameName(): string {
    return "Game Guesser";
}

function didWin(data: GameWordData[]): boolean
{
    return data.every(x => x.displayedWord !== null && !x.displayAsClose);
}

export default function MainForm() {
    let [data, setData] = useState<GameData | null>(null);
    let [msg, setMsg] = useState<string | null>("Loading...");
    let [input, setInput] = useState("");
    let [canType, setCanType] = useState(true);
    let [showVictory, setShowVictory] = useState(false);
    let [showRules, setShowRules] = useState((localStorage.getItem("rules") ?? "0") !== "1");
    let [lastInput, setLastInput] = useState<LastWordInfo | null>(null);
    const inputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        let timeoutID: number | null = null;

        function getApiInfo() {
            fetch(`${getEndpoint()}/api/info`)
            .then(x => {
                return x.json().then((x: GameData) => {
                    if (!x.isReady) { // Backend is not ready yet...
                        setMsg(`First connection of the day, data are being initialized, please wait... ${x.progression}%`);
                        timeoutID = setTimeout(getApiInfo, 1_000);
                        return;
                    }

                    if (parseInt(localStorage.getItem("iteration") ?? "0") === x.iteration) {
                        try
                        {
                            const state: GameData = JSON.parse(localStorage.getItem("state")!);
                            setData(state);
                            if (didWin(state.name))
                            {
                                setShowVictory(true);
                            }
                        }
                        catch
                        {
                            console.warn("Failed to deserialize game state from local storation, resetting data");
                            localStorage.setItem("iteration", x.iteration.toString());
                            localStorage.setItem("state", JSON.stringify(x));
                            setData(x);
                        }
                    } else {
                        localStorage.setItem("iteration", x.iteration.toString());
                        localStorage.setItem("state", JSON.stringify(x));
                        
                        setData(x);
                    }

                    setMsg(null)
                }).catch(err => {
                    setMsg(`An error occured while initializing game data: ${err}`)
                });
            });
        }
        getApiInfo();

        return () => {
            if (timeoutID !== null) clearTimeout(timeoutID);
        }
    }, []);

    useEffect(() => {
        if (canType) inputRef?.current?.focus();
    }, [ canType ])

    function updateTokenList(tokens: GameWordData[], x: WordBlockData) {
        for (let ci of x.closeIndexes) {
            if (tokens[ci.index].displayedWord === null || (tokens[ci.index].displayAsClose !== null && ci.score > tokens[ci.index].displayAsClose!)) { // Don't replace words we already found
                tokens[ci.index].displayedWord = input;
                tokens[ci.index].displayAsClose = ci.score;
            }
        }
        for (let fi of x.foundIndexes) {
            tokens[fi.index].displayedWord = fi.word;
            tokens[fi.index].displayAsClose = null;
        }
    }

    if (msg !== null) {
        return (
            <>
                {
                    showRules
                    ? <RulesForm close={() => { setShowRules(false); localStorage.setItem("rules", "1"); }} />
                    : <></>
                }
                <div className="container box">
                    { msg }
                </div>
                <div className="container box">
                    <Link to="/privacy">Privacy & Contact</Link>
                </div>
            </>
        )
    }

    return (
        <>
            {
                showRules
                ? <RulesForm close={() => { setShowRules(false); localStorage.setItem("rules", "1"); }} />
                : <></>
            }
            {
                showVictory
                ? <WinningForm close={() => { setShowVictory(false) }} state={data!} />
                : <></>
            }
            <div className="container box is-flex flex-center-ver" id="input-area">
                <input ref={inputRef} disabled={!canType} value={input} onChange={x => setInput((x.target as HTMLInputElement).value)} type="text" onKeyDown={e => {
                    if (e.key === "Enter")
                    {
                        setCanType(false)
                        fetch(`${getEndpoint()}/api/validate/${input}`)
                        .then(x => {
                            if (x.ok) return x.json();
                            throw new Error();
                        })
                        .then((x: WordData) => {
                            setLastInput({
                                word: input,
                                data: x
                            });
                            setData(d => {
                                let nameTokens = [...d!.name];
                                let descriptionTokens = [...d!.description];
                                updateTokenList(nameTokens, x.name);
                                updateTokenList(descriptionTokens, x.description);
                                let newData = {
                                    isReady: true,
                                    progression: undefined,
                                    iteration: d!.iteration,
                                    name: nameTokens,
                                    description: descriptionTokens
                                };
                                localStorage.setItem("state", JSON.stringify(newData));
                                if (didWin(newData.name))
                                {
                                    setShowVictory(true);
                                }
                                return newData;
                            })
                            setInput("");
                            setCanType(true);
                        })
                        .catch(_ => setCanType(true));
                    }
                }} />
                <p id="last-input" className="is-flex flex-center-ver">
                {
                    lastInput ?
                    <>
                        <span id="last-word">{ lastInput.word }</span>
                        <span id="last-found">{lastInput.data.name.foundIndexes.length + lastInput.data.description.foundIndexes.length}</span>
                        <span id="last-close">{lastInput.data.name.closeIndexes.length + lastInput.data.description.closeIndexes.length}</span>
                    </>
                    : <></>
                }
                </p>
            </div>
            <GuessAreaForm data={data!.name} id="guess-name" lastInput={lastInput?.data?.name ?? null} />
            <GuessAreaForm data={data!.description} id="guess-desc" lastInput={lastInput?.data?.description ?? null} />
            <div className="container box is-flex">
                <Link to="/privacy">Privacy & Contact</Link>
                <a onClick={() => setShowRules(true) }>Show rules</a>
            </div>
        </>
    )
}