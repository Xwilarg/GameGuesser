interface AboutFormProps
{
    close: () => void
}

export default function AboutForm({ close }: AboutFormProps) {
    return (
        <div className='modal is-flex flex-center-hor flex-center-ver'>
            <div>
                <h2>About</h2>
                Game inspired by <a href="https://www.synoptix.fr/" target="_blank">Synoptix</a><br/>
                Daily reset happens at midnight UTC+0
                <h2>Privacy</h2>
                This website is using Shika for its analytics<br/>
                You can see all the data collected <a href="https://astylodon.org/docs/shika/data" target='_blank'>here</a><br/>
                Please note that Shika doesn't collect any data that allow to track individual users
                <h2>Contact & contribute</h2>
                The website source code is available at on <a href="https://github.com/Xwilarg/GameGuesser" target="_blank">GitHub</a><br/>
                For any inquiery, please open an issue or a conversation there
            </div>
            <button onClick={close}>Close</button>
        </div>
    )
}