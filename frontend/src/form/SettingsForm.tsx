import { useLocalize } from "localize-react"

interface SettingsFormProps
{
    language: string
    setLanguage: (newLang: string) => void
    close: () => void
}

export default function SettingsForm({ close, language, setLanguage }: SettingsFormProps) {
    const { translate } = useLocalize();

    return (
        <div className='modal is-flex flex-center-hor flex-center-ver'>
            <div>
                <h2>{translate("settings.language")}</h2>
                <small>{translate("settings.language_warn")}</small><br/>
                <select onChange={(e) => setLanguage(e.target.value)} defaultValue={language}>
                    <option value="en">English</option>
                    <option value="es">Español</option>
                </select>
            </div>
            <button onClick={close}>Close</button>
        </div>
    )
}