import { getGameName } from "./MainForm";

interface RulesFormProps
{
    close: () => void
}

export default function RulesForm({ close }: RulesFormProps) {
    return (
        <div className='modal is-flex flex-center-hor flex-center-ver'>
            <h1>Welcome to {getGameName()}</h1>
            <p>
                Your goal is to find what PC game is hidden behind this description<br/>
                For that, use the input bar at the top to enter words<br/>
                <br/>
                If the word appear <span className="found-word">green</span>, good job, you just found a word!<br/>
                <small>Once you enter more, the word will then turn white</small><br/>
                <br/>
                If the word appear <span style={{ color: `rgb(255, 166, 0)` }}>orange</span>, eeh, you are close but that's not totally it. Watch the <span style={{ color: `rgba(255, 166, 0, 0.6)` }}>transparency</span> of the word tho, the more opaque, the closer it is to the word you're looking for!<br/>
                <br/>
                A last little tip before you go, click on a word you haven't found to see the amount of characters it contains<br/>
                <br/>
                Good luck!
            </p>
            <button onClick={close}>Got it!</button>
        </div>
    )
}