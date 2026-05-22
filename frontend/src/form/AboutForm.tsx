import { useLocalize } from "localize-react"

interface AboutFormProps
{
    close: () => void
}

export default function AboutForm({ close }: AboutFormProps) {
    const { translate } = useLocalize();

    return (
        <div className='modal is-flex flex-center-hor flex-center-ver'>
            <div>
                <h2>{translate("about.about")}</h2>
                {translate("about.inspired_by")} <a href="https://www.synoptix.fr/" target="_blank">Synoptix</a><br/>
                {translate("about.daily_reset")}
                <h2>{translate("about.privacy")}</h2>
                {translate("about.shika_intro")}<br/>
                {translate("about.shika_privacy")} <a href="https://astylodon.org/docs/shika/data" target='_blank'>https://astylodon.org/docs/shika/data</a><br/>
                {translate("about.shika_tracking")}
                <h2>{translate("about.contact_and_contribute")}</h2>
                {translate("about.source_code")}<a href="https://github.com/Xwilarg/GameGuesser" target="_blank">GitHub</a><br/>
                {translate("about.issue")}
            </div>
            <button onClick={close}>{translate("generic.close")}</button>
        </div>
    )
}