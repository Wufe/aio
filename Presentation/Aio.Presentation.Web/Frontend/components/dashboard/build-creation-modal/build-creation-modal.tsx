import * as React from 'react';
import { Modal } from '~/components/modal/modal';
import './build-creation-modal.scss';
import { useBuildCreation } from './build-creation-hooks';
import { useModal } from '~/components/modal/modal-hooks';
import { useBuildAPI } from '~/components/pages/build/build-hook';

type TForm = {
    [field: string]: {
        enabled: boolean;
        value: string;
    };
}

const modalName = "@@BuildCreationModal@@";

const BuildCreationModal = () => {

    const [name, setName] = React.useState("");
    const [repositoryURL, setRepositoryURL] = React.useState("");
    const [formEnabled, setFormEnabled] = React.useState(true);
    const { saveBuild } = useBuildCreation();
    const { hide: hideModal } = useModal();
    const { getAll } = useBuildAPI();

    const save = async (e: React.FormEvent) => {
        e.preventDefault();
        setFormEnabled(false);
        try {
            await saveBuild({ name, repositoryURL });
            getAll();
            hideModal();
        } catch (e) {
            setFormEnabled(true);
        }
        
    };

    const cancel = (e: React.FormEvent) => {
        e.preventDefault();
        hideModal();
    }

    return <Modal name={modalName}>
        <div className="modal build-creation__modal">
            <header className="__header">
                <h3>Create build</h3>
            </header>
            <form onSubmit={e => save(e)}>
                <label htmlFor="name">
                    <span>Name *</span>
                    <input type="text" name="name" id="name" autoFocus placeholder="My awesome build"
                        disabled={!formEnabled}
                        onChange={e => setName(e.target.value)}
                        value={name} />
                </label>
                <div className="__actions">
                    <button type="button" className="neui-button" disabled={!formEnabled} onClick={e => cancel(e)}>Cancel</button>
                    <input type="submit" className="neui-button --success" disabled={!formEnabled} value="Go ahead" />
                </div>
            </form>
        </div>
    </Modal>
};

BuildCreationModal.MODAL_NAME = modalName;

export {
    BuildCreationModal
};