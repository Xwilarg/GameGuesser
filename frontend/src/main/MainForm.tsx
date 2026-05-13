import { useEffect, useRef, useState } from "react"

interface GameData
{
    tokens: GameWordData[]
}

interface GameWordData
{
    displayedWord: string
    displayAsClose: number | null // Word was not found but we are close
    length: number
}

interface WordData
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
                    setData(x);
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

    if (!data) {
        if (isLoadingData) {
            return (
                <div className="container box">
                    Data are being initialized, please wait...
                </div>
            )
        }

        return (
            <div className="container box">
                Loading...
            </div>
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
                                let tokens = [...d!.tokens];
                                for (let ci of x.closeIndexes) {
                                    if (tokens[ci.index].displayedWord === null || (tokens[ci.index].displayAsClose !== null && ci.score > tokens[ci.index].displayAsClose!)) { // Don't replace words we already found
                                        tokens[ci.index].displayedWord = input;
                                        tokens[ci.index].displayAsClose = ci.score;
                                    }
                                }
                                for (let i of x.foundIndexes) {
                                    tokens[i].displayedWord = input;
                                    tokens[i].displayAsClose = null;
                                    tokens[i].displayAsClose = null;
                                }
                                return { tokens: tokens };
                            })
                            setInput("");
                            setCanType(true);
                        })
                        .catch(_ => setCanType(true));
                    }
                }} />
            </div>
            <div className="container box is-flex" id="word-list">
                {
                    data.tokens.map(x => {
                        if (x.displayedWord === null)
                        {
                            return <span className="hidden-word" style={{
                                width: (x.length * 14.4025) + "px"
                            }}></span>
                        }
                        if (x.displayAsClose !== null) {
                            return <span style={{
                                color: `rgba(255, 166, 0, ${x.displayAsClose})`
                            }}>{x.displayedWord}</span>
                        }
                        return <span>{x.displayedWord}</span>
                    })
                }
            </div>
        </>
    )
}