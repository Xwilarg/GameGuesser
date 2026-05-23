import { createRoot } from 'react-dom/client'
import { BrowserRouter, Routes, Route } from "react-router";
import { useState, type ReactElement } from 'react';
import MainForm from './form/MainForm';
import { LocalizationProvider } from 'localize-react';
import translationsEn from "../translation/en.json";
import translationsEs from "../translation/es.json";

interface AppRouterProps {
    lang: string;
    setLang: React.Dispatch<React.SetStateAction<string>>;
}

function AppRouter({ lang, setLang}: AppRouterProps) : ReactElement {
    return (
        <Routes>
            <Route path="/" element={<MainForm lang={lang} setLang={setLang} />} />
        </Routes>
    )
}

function App() {
    const [lang, setLang] = useState<string>(() => {
        return localStorage.getItem("lang") ?? navigator.language?.split('-')[0];
    });

    const translations = {
        "en": translationsEn,
        "es": translationsEs
    }

    return (
        <LocalizationProvider locale={lang} translations={translations}>
            <BrowserRouter>
                <AppRouter lang={lang} setLang={setLang} />
            </BrowserRouter>
        </LocalizationProvider>
    );
}

createRoot(document.getElementById('root')!).render(<App />)