import { useLocalize } from "localize-react";
import { getGameName } from "./MainForm";

interface RulesFormProps
{
    close: () => void
}

export default function RulesForm({ close }: RulesFormProps) {
    const { translate } = useLocalize();
    return (
        <div className='modal is-flex flex-center-hor flex-center-ver'>
            <h1>{translate("rules.greetings", { gamename: getGameName() })}</h1>
            <p>
                {translate("rules.goal_1")}<br/>
                {translate("rules.goal_2")}
            </p>
            <p>
                <span dangerouslySetInnerHTML={{__html: translate("rules.green_1")}}></span>
                <br/>
                <small>{translate("rules.green_2")}</small>
            </p>
            <p>
                <span dangerouslySetInnerHTML={{__html: translate("rules.orange_1")}}></span>
                <br/>
                <span dangerouslySetInnerHTML={{__html: translate("rules.orange_2")}}></span>
            </p>
            <p>
                {translate("rules.length")}
            </p>
            <p>
                {translate("rules.good_luck")}
            </p>
            <button onClick={close}>{translate("rules.got_it")}!</button>
        </div>
    )
}