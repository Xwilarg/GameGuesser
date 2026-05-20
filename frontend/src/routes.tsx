import { createRoot } from 'react-dom/client'
import { BrowserRouter, Routes, Route } from "react-router";
import { type ReactElement } from 'react';
import MainForm from './form/MainForm';

function AppRouter() : ReactElement {
    return (
        <Routes>
            <Route path="/" element={<MainForm/>} />
        </Routes>
    )
}

createRoot(document.getElementById('root')!).render(
    <BrowserRouter>
        <AppRouter />
    </BrowserRouter>
)