import { useEffect, useRef, useState } from "react"
import GuessAreaForm from "./GuessAreaForm"
import { Link } from "react-router"

interface GameData
{
    iteration: number
    name: GameWordData[]
    description: GameWordData[]
}

export interface GameWordData
{
    wasJustFound: boolean
    displayedWord: string
    displayAsClose: number | null // Word was not found but we are close
    length: number
}

interface WordData
{
    name: WordBlockData
    description: WordBlockData
}

interface WordBlockData
{
    foundIndexes: number[]
    closeIndexes: WordIndexScoreInfo[]
}

interface WordIndexScoreInfo
{
    index: number
    score: number
}

export default function MainForm() {
    let [data, setData] = useState<GameData | null>(null);
    let [input, setInput] = useState("");
    let [canType, setCanType] = useState(true);
    let [isLoadingData, setIsLoadingData] = useState(false);
    const inputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        let timeoutID: number | null = null;

        function getApiInfo() {
            fetch("/api/info")
            .then(x => {
                if (x.status === 204) {
                    setIsLoadingData(true);
                    timeoutID = setTimeout(getApiInfo, 1_000);
                    return;
                }
                return x.json().then((x: GameData) => {
                    setIsLoadingData(true);

                    if (parseInt(localStorage.getItem("iteration") ?? "0") === x.iteration) {
                        try
                        {
                            setData(JSON.parse(localStorage.getItem("state")!));
                        }
                        catch
                        {
                            console.warn("Failed to deserialize game state from local storation, resetting data");
                            localStorage.setItem("iteration", x.iteration.toString());
                            localStorage.setItem("state", "{}");
                            setData(x);
                        }
                    } else {
                        localStorage.setItem("iteration", x.iteration.toString());
                        localStorage.setItem("state", "{}");
                        
                        setData(x);
                    }
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
        for (let token of tokens) {
            token.wasJustFound = false;
        }
        for (let ci of x.closeIndexes) {
            if (tokens[ci.index].displayedWord === null || (tokens[ci.index].displayAsClose !== null && ci.score > tokens[ci.index].displayAsClose!)) { // Don't replace words we already found
                tokens[ci.index].displayedWord = input;
                tokens[ci.index].displayAsClose = ci.score;
            }
        }
        for (let i of x.foundIndexes) {
            tokens[i].wasJustFound = true;
            tokens[i].displayedWord = input;
            tokens[i].displayAsClose = null;
        }
    }

    if (!data) {
        if (isLoadingData) {
            return (
                <>
                    <div className="container box">
                        Data are being initialized, please wait...
                    </div>
                    <div className="container box">
                        <Link to="/privacy">Privacy & Contact</Link>
                    </div>
                </>
            )
        }

        return (
            <>
                <div className="container box">
                    Loading...
                </div>
                <div className="container box">
                    <Link to="/privacy">Privacy & Contact</Link>
                </div>
            </>
        )
    }

    return (
        <>
            <div className="container box">
                <input ref={inputRef} disabled={!canType} value={input} onChange={x => setInput((x.target as HTMLInputElement).value)} type="text" onKeyDown={e => {
                    if (e.key === "Enter")
                    {
                        setCanType(false)
                        fetch(`/api/validate/${input}`)
                        .then(x => {
                            if (x.ok) return x.json();
                            throw new Error();
                        })
                        .then((x: WordData) => {
                            setData(d => {
                                let nameTokens = [...d!.name];
                                let descriptionTokens = [...d!.description];
                                updateTokenList(nameTokens, x.name);
                                updateTokenList(descriptionTokens, x.description);
                                let newData = {
                                    iteration: d!.iteration,
                                    name: nameTokens,
                                    description: descriptionTokens
                                };
                                localStorage.setItem("state", JSON.stringify(newData));
                                return newData;
                            })
                            setInput("");
                            setCanType(true);
                        })
                        .catch(_ => setCanType(true));
                    }
                }} />
            </div>
            <GuessAreaForm data={data.name} />
            <GuessAreaForm data={data.description} />
            <div className="container box">
                <Link to="/privacy">Privacy & Contact</Link>
            </div>
        </>
    )
}