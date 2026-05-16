import { createRoot } from 'react-dom/client'
import { BrowserRouter, Routes, Route } from "react-router";
import MainForm from './main/MainForm';
import { type ReactElement } from 'react';
import PrivacyForm from './main/PrivacyForm';

function AppRouter() : ReactElement {
    return (
        <Routes>
            <Route path="/" element={<MainForm/>} />
            <Route path="/privacy" element={<PrivacyForm/>} />
        </Routes>
    )
}

createRoot(document.getElementById('root')!).render(
    <BrowserRouter>
        <AppRouter />
    </BrowserRouter>
)