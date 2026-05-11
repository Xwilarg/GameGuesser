import { useEffect, useState } from "react"

interface GameData
{
    tokens: GameWordData[]
}

interface GameWordData
{
    displayedWord: string
    length: number
}

export default function MainForm() {
    let [data, setData] = useState<GameData | null>(null);

    useEffect(() => {
        fetch("/api/info")
        .then(x => x.json())
        .then(x => setData(x));
    }, []);

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
                <input type="text" />
            </div>
            <div className="container box is-flex" id="word-list">
                {
                    data.tokens.map(x => {
                        if (x.displayedWord === null)
                        {
                            return <span className="hidden-word" style={{
                                width: (x.length * 20) + "px"
                            }}></span>
                        }
                        return <span>{x.displayedWord}</span>
                    })
                }
            </div>
        </>
    )
}