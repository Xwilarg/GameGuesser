import { useEffect, useRef, useState } from "react"

interface GameData
{
    tokens: GameWordData[]
}

interface GameWordData
{
    displayedWord: string
    length: number
}

interface WordData
{
    foundIndexed: number[]
}

export default function MainForm() {
    let [data, setData] = useState<GameData | null>(null);
    let [input, setInput] = useState("");
    let [canType, setCanType] = useState(true);
    const inputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        fetch("/api/info")
        .then(x => x.json())
        .then(x => setData(x));
    }, []);

    useEffect(() => {
        if (canType) inputRef?.current?.focus();
    }, [ canType ])

    if (!data) {
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
                                for (let i of x.foundIndexed) {
                                    tokens[i].displayedWord = input;
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
                        return <span>{x.displayedWord}</span>
                    })
                }
            </div>
        </>
    )
}