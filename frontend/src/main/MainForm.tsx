import { useEffect, useState } from "react"

interface GameData
{
    name: string,
    description: string,
    iteration: number
}

export default function MainForm() {
    let [data, setData] = useState<GameData | null>(null);

    useEffect(() => {
        fetch("/php/getData.php")
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
        <div className="container box">
            Data loaded...
        </div>
    )
}