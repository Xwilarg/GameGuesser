import { getGameName } from "./MainForm";

interface SettingsFormProps
{
    language: string
    setLanguage: (newLang: string) => void
    close: () => void
}

export default function SettingsForm({ close, language, setLanguage }: SettingsFormProps) {
    return (
        <div className='modal is-flex flex-center-hor flex-center-ver'>
            <div>
                <h2>Language</h2>
                <select onChange={(e) => setLanguage(e.target.value)}>
                    <option value="en" selected={language === "en"}>English</option>
                </select>
            </div>
            <button onClick={close}>Close</button>
        </div>
    )
    /* TODO: Add those once backend handle adjacent words for them
        <option value="fr" selected={language === "fr"}>Français</option>
        <option value="es" selected={language === "es"}>Español</option>
        <option value="nl" selected={language === "nl"}>Nederlands</option>
    */
}