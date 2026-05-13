import { useEffect, useRef, useState } from "react"

interface GameData
{
    tokens: GameWordData[]
}

interface GameWordData
{
    displayedWord: string
    displayAsClose: boolean // Word was not found but we are close
    length: number
}

interface WordData
{
    foundIndexes: number[]
    closeIndexes: number[]
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
                return x.json().then(x => {
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
                                for (let i of x.closeIndexes) {
                                    if (tokens[i].displayedWord === null || tokens[i].displayAsClose) { // Don't replace words we already found
                                        tokens[i].displayedWord = input;
                                        tokens[i].displayAsClose = true;
                                    }
                                }
                                for (let i of x.foundIndexes) {
                                    tokens[i].displayedWord = input;
                                    tokens[i].displayAsClose = false;
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
                        return <span className={x.displayAsClose ? "close-word" : ""}>{x.displayedWord}</span>
                    })
                }
            </div>
        </>
    )
}