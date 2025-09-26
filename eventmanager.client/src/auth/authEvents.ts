type Listener = () => void;
const listeners: Listener[] = [];

export const authEvents = {
    on: (listener: Listener) => listeners.push(listener),
    off: (listener: Listener) => {
        const idx = listeners.indexOf(listener);
        if (idx >= 0) {
            listeners.splice(idx, 1);
        }
    },
    emit: () => listeners.forEach(l => l()),
};