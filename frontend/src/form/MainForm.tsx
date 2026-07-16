import { useEffect, useRef, useState, type ReactElement } from "react"
import GuessAreaForm from "./GuessAreaForm"
import WinningForm from "./WinningForm"
import RulesForm from "./RulesForm"
import type { LastWordInfo } from "../model/LastWordInfo"
import type { GameData, GameWordData } from "../model/GameData"
import type { WordBlockData, WordData } from "../model/WordData"
import AboutForm from "./AboutForm"
import SettingsForm from "./SettingsForm"
import { useLocalize } from "localize-react"
import { type WordHistoryData } from "../model/WordHistoryData"

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

interface MainFormProps
{
    setLang: React.Dispatch<React.SetStateAction<string>>
    lang: string
}

export default function MainForm({ lang, setLang }: MainFormProps) {
    let [data, setData] = useState<GameData | null>(null);
    let [msg, setMsg] = useState<string | null>("Loading...");
    let [input, setInput] = useState("");
    let [haveWon, setHaveWon] = useState(false);
    let [lastInput, setLastInput] = useState<LastWordInfo | null>(null);
    let [history, setHistory] = useState<WordHistoryData[]>([]);
    let [hints, setHints] = useState<string[] | null>(null);
    const inputRef = useRef<HTMLInputElement>(null);

    // Modals
    let [showVictory, setShowVictory] = useState(false);
    let [showAbout, setShowAbout] = useState(false);
    let [showSettings, setShowSettings] = useState(false);
    let [showRules, setShowRules] = useState((localStorage.getItem("rules") ?? "0") !== "1");

    const { translate } = useLocalize();

    function clearGameState()
    {
        for (let lang of [ "en", "es" ])
        {
            localStorage.removeItem(`${lang}-name`)
            localStorage.removeItem(`${lang}-shortdesc`)
            localStorage.removeItem(`${lang}-desc`)
            localStorage.removeItem(`${lang}-history`)
            localStorage.removeItem(`${lang}-hint`)
        }
    }

    function saveGameState(x: GameData) {
        localStorage.setItem(`${x.language}-name`, JSON.stringify(x.name));
        localStorage.setItem(`${x.language}-shortdesc`, JSON.stringify(x.shortDescription));
        localStorage.setItem(`${x.language}-desc`, JSON.stringify(x.description));
    }

    function saveHistory(lang: string, history: WordHistoryData[]) {
        localStorage.setItem(`${lang}-history`, JSON.stringify(history));
    }

    useEffect(() => {
        let timeoutID: number | null = null;

        function getApiInfo() {
            fetch(`${getEndpoint()}/api/info/${lang}`)
            .then(x => {
                return x.json().then((x: GameData) => {
                    if (lang !== x.language) { // Language we got wasn't the one we expected
                        setLang(x.language);
                        return
                    }

                    if (!x.isReady) { // Backend is not ready yet...
                        setMsg(translate("main.initialize", { progression: x.progression!.toString() }));
                        if (timeoutID !== null) clearTimeout(timeoutID);
                        timeoutID = setTimeout(getApiInfo, 1_000);
                        return;
                    }

                    // Init field
                    // We note these fields as "to be guessed" depending of how they are given from the API
                    for (let t of x.name) {
                        t.needToBeGuessed = t.displayedWord === null;
                        t.displayAsClose = null;
                    }
                    for (let t of x.description) {
                        t.needToBeGuessed = t.displayedWord === null;
                        t.displayAsClose = null;
                    }
                    for (let t of x.shortDescription) {
                        t.needToBeGuessed = t.displayedWord === null;
                        t.displayAsClose = null;
                    }

                    if (parseInt(localStorage.getItem("iteration") ?? "0") === x.iteration) {
                        try
                        {
                            const name: GameWordData[] = JSON.parse(localStorage.getItem(`${x.language}-name`)!);
                            const savedHistory = JSON.parse(localStorage.getItem(`${x.language}-history`) ?? "[]").filter((x: WordHistoryData) => x.gotApiResponse);
                            setHistory(savedHistory);
                            setData({
                                isReady: true,
                                progression: x.progression,
                                language: x.language,
                                iteration: x.iteration,
                                name: name,
                                shortDescription: JSON.parse(localStorage.getItem(`${x.language}-shortdesc`)!),
                                description: JSON.parse(localStorage.getItem(`${x.language}-desc`)!)
                            });
                            const hints = localStorage.getItem(`${x.language}-hint`);
                            if (hints) setHints(JSON.parse(hints));
                            else setHints(null);

                            if (didWin(name)) {
                                setHaveWon(true);
                                setShowVictory(true);
                            }
                        }
                        catch
                        {
                            console.warn("Failed to deserialize game state from local storation, resetting data");
                            localStorage.setItem("iteration", x.iteration.toString());
                            clearGameState();
                            saveGameState(x);
                            setData(x);
                        }
                    } else {
                        localStorage.setItem("iteration", x.iteration.toString());
                        clearGameState();
                        saveGameState(x);
                        
                        setData(x);
                    }

                    setMsg(null)
                }).catch(err => {
                    setMsg(translate("error.generic", { error: err }))
                });
            });
        }

        getApiInfo();

        return () => {
            if (timeoutID !== null) clearTimeout(timeoutID);
        }
    }, [ lang ]);

    function updateTokenList(tokens: GameWordData[], x: WordBlockData) {
        for (let ci of x.closeIndexes) {
            if (tokens[ci.index].displayedWord === null || (tokens[ci.index].displayAsClose !== null && ci.score >= tokens[ci.index].displayAsClose!)) { // Don't replace words we already found
                tokens[ci.index].displayedWord = input;
                tokens[ci.index].displayAsClose = ci.score;
                ci.isBetter = true;
            } else {
                ci.isBetter = false;
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
                { showAbout ? <AboutForm close={() => { setShowAbout(false); }} /> : <></> }
                { showRules ? <RulesForm close={() => { setShowRules(false); localStorage.setItem("rules", "1"); }} /> : <></> }
                { showSettings ? <SettingsForm close={() => { setShowSettings(false) }} language={lang} setLanguage={(newLang: string) => {
                    localStorage.setItem("lang", newLang); // Store user prefered language
                    setLang(newLang);
                    }}/> : <></>
                }
                <div className="container box">
                    { msg }
                </div>
                <div className="container box is-flex">
                <a onClick={() => { closeModals(); setShowAbout(true); } }>{ translate("footer.privacy_and_contact") }</a>
                <a onClick={() => { closeModals(); setShowRules(true); } }>{ translate("footer.rules") }</a>
                <a onClick={() => { closeModals(); setShowSettings(true); } }>{ translate("footer.settings") }</a>
                </div>
            </>
        )
    }

    function closeModals() {
        setShowAbout(false);
        setShowRules(false);
        setShowVictory(false);
        setShowSettings(false);
    }

    let hintsHtml: ReactElement;
    if (history.length < 100) {
        hintsHtml = <p>{100 - history.length} tries before hints are available...</p>;
    } else if (hints !== null) {
        hintsHtml = <p>Game genres<br/>{ hints.join(", ") }</p>
    } else {
        hintsHtml = <p><button onClick={() => {
            fetch(`${getEndpoint()}/api/hint/${lang}`)
                .then(x => {
                    if (x.ok) return x.json();
                    throw new Error();
                })
                .then((x: string[]) => {
                    setHints(x);
                    localStorage.setItem(`${lang}-hint`, JSON.stringify(x));
                });
        }}>Click here to reveal hints</button></p>;
    }

    return (
        <>
            { showAbout ? <AboutForm close={() => { setShowAbout(false); }} /> : <></> }
            { showRules ? <RulesForm close={() => { setShowRules(false); localStorage.setItem("rules", "1"); }} /> : <></> }
            { showVictory ? <WinningForm close={() => { setShowVictory(false) }} state={data!} history={history} endpoint={getEndpoint()} lang={lang} /> : <></> }
            { showSettings ? <SettingsForm close={() => { setShowSettings(false) }} language={lang} setLanguage={(newLang: string) => {
                localStorage.setItem("lang", newLang); // Store user prefered language
                setLang(newLang);
                }}/> : <></>
            }

            <div className="is-flex columns">
                <div className="sub-column"></div>
                <div className="main-column">
                    <div className="container box is-flex flex-center-ver" id="input-area">
                        <input ref={inputRef} value={input} onChange={x => setInput((x.target as HTMLInputElement).value)} type="text" onKeyDown={e => {
                            if (e.key === "Enter")
                            {
                                // Clean input
                                input = input.trim();

                                const histInput = history.find(x => x.data.word.toLowerCase() === input.toLowerCase());
                                if (histInput && histInput.data.close === 0) {
                                    // Word was already typed
                                    setLastInput(histInput!.data)
                                } else {
                                    // Add word to history
                                    if (!histInput) {
                                        history.push({
                                            gotApiResponse: false,
                                            data: {
                                                word: input,
                                                close: 0,
                                                foundName: [],
                                                foundDesc: [],
                                                foundShortDesc: []
                                            }
                                        });
                                        setHistory([...history]);
                                        saveHistory(lang, history);
                                    }

                                    fetch(`${getEndpoint()}/api/validate/${lang}/${input}`)
                                    .then(x => {
                                        if (x.ok) return x.json();
                                        throw new Error();
                                    })
                                    .then((x: WordData) => {
                                        setData(d => {
                                            let nameTokens = [...d!.name];
                                            let descriptionTokens = [...d!.description];
                                            let shortDescriptionTokens = [...d!.shortDescription];
                                            updateTokenList(nameTokens, x.name);
                                            updateTokenList(descriptionTokens, x.description);
                                            updateTokenList(shortDescriptionTokens, x.shortDescription);
                                            let newData = {
                                                isReady: true,
                                                progression: undefined,
                                                language: lang,
                                                iteration: d!.iteration,
                                                name: nameTokens,
                                                description: descriptionTokens,
                                                shortDescription: shortDescriptionTokens
                                            };
                                            const lastInput = {
                                                word: input,
                                                close: 
                                                    x.name.closeIndexes.filter(x => x.isBetter).length +
                                                    x.shortDescription.closeIndexes.filter(x => x.isBetter).length +
                                                    x.description.closeIndexes.filter(x => x.isBetter).length,
                                                foundName: x.name.foundIndexes.map(x => x.index),
                                                foundDesc: x.description.foundIndexes.map(x => x.index),
                                                foundShortDesc: x.shortDescription.foundIndexes.map(x => x.index)
                                            };
                                            setLastInput(lastInput);

                                            // Update history
                                            if (histInput)
                                            {
                                                histInput.data = lastInput;
                                            }
                                            else
                                            {
                                                let h = history.find(x => x.data.word === input)!;
                                                h.gotApiResponse = true;
                                                h.data = lastInput;
                                            }

                                            setHistory([...history]);
                                            saveHistory(lang, history);
                                            saveGameState(newData);
                                            if (!haveWon && didWin(newData.name)) {
                                                setHaveWon(true);
                                                setShowVictory(true);
                                            }
                                            return newData;
                                        })
                                    })
                                    .catch(() => {
                                        if (!histInput) {
                                            let h = history.find(x => x.data.word === input)!;
                                            const index = history.indexOf(h);
                                            history.splice(index, 1);
                                            setHistory([...history]);
                                            saveHistory(lang, history);
                                        }
                                    });
                                }
                                setInput("");
                                inputRef?.current?.focus();
                            }
                        }} />
                        <p id="last-input" className="is-flex flex-center-ver">
                        {
                            lastInput ?
                            <>
                                <span className="last-word">{ lastInput.word }</span>
                                <span className="last-found">{ lastInput.foundName.length + lastInput.foundDesc.length + lastInput.foundShortDesc.length }</span>
                                <span className="last-close">{ lastInput.close }</span>
                            </>
                            : <></>
                        }
                        </p>
                    </div>
                    <GuessAreaForm data={data!.name} id="guess-name" lastInput={lastInput?.foundName ?? null} />
                </div>
                <div className="sub-column">
                </div>
            </div>
            <div className="is-flex columns">
                <div className="sub-column">
                    <div id="history" className="container box is-flex">
                        {
                            [...history].reverse().map(x =>
                                <div className="is-flex" key={x.data.word}>
                                    <span className="last-word">{ x.data.word }</span>
                                    <span className="last-found">{ x.data.foundName.length + x.data.foundDesc.length + x.data.foundShortDesc.length }</span>
                                    <span className="last-close">{ x.data.close }</span>
                                </div>
                            )
                        }
                    </div>
                </div>
                <div className="main-column">
                    <GuessAreaForm data={data!.shortDescription} id="guess-sdesc" lastInput={lastInput?.foundShortDesc ?? null} />
                    <GuessAreaForm data={data!.description} id="guess-desc" lastInput={lastInput?.foundDesc ?? null} />
                    <div className="container box is-flex">
                        <a onClick={() => { closeModals(); setShowAbout(true); } }>{ translate("footer.privacy_and_contact") }</a>
                        <a onClick={() => { closeModals(); setShowRules(true); } }>{ translate("footer.rules") }</a>
                        <a onClick={() => { closeModals(); setShowSettings(true); } }>{ translate("footer.settings") }</a>
                        {
                            haveWon
                            ? <a onClick={() => { closeModals(); setShowVictory(true); }}>{ translate("footer.victory") }</a>
                            : <></>
                        }
                    </div>
                </div>
                <div className="sub-column">
                    <div className="container box">
                        { hintsHtml }
                    </div>
                </div>
            </div>
        </>
    )
}