import * as React from 'react';
import { App } from '~/components/app/app';
import { useModal } from '~/components/modal/modal-hooks';
import './app-container.scss';

export const AppContainer = ({ children }: React.PropsWithChildren<{}>) => {

    const { visible } = useModal();

    return <div className={`app-container__component ${visible ? '--blurred' : ''}`}>
        <App />
        {children}
    </div>
}