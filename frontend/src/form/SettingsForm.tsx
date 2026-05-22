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
                <small>Language availability will depend of the daily game found</small><br/>
                <select onChange={(e) => setLanguage(e.target.value)}>
                    <option value="en" selected={language === "en"}>English</option>
                    <option value="es" selected={language === "es"}>Español</option>
                </select>
            </div>
            <button onClick={close}>Close</button>
        </div>
    )
}