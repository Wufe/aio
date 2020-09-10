import * as React from 'react';
import './build-summary.scss';
import { TBuild, TRun } from '~/types';
import { FaTrashAlt, FaPencilAlt, FaDivide, FaPlay } from 'react-icons/fa';
import { Link } from 'react-router-dom';
import { useBuildPageLoad, useBuildAPI, usePolling, delay } from '../pages/build/build-hook';
import { diff } from 'deep-diff';
import { Modal } from '../modal/modal';
import { useModal } from '../modal/modal-hooks';
import { GenericModalLayout } from '../modal/modal-layout/textual-modal-layout/generic-modal-layout';
import { BuildTerminalLog } from './build-terminal-log/build-terminal-log';

type TProps = {
    build: TBuild;
    onHeaderClick?: () => void;
    active: boolean;
};

export const BuildSummary = React.memo((props: React.PropsWithChildren<TProps>) => {

    const [run, setRun] = React.useState<TRun>(null);
    
    const { go } = useBuildPageLoad();
    const { getAll, remove } = useBuildAPI();
    const { getLatestRun, enqueueNewRun } = useBuildAPI();
    const { hide, show } = useModal();

    const enqueueNewRunModal = `@@enqueueNewRun${props.build.id}Modal@@`;
    const deleteBuildModal = `@@deleteBuild${props.build.id}Modal@@`;
    
    usePolling(
        () => getLatestRun(props.build.id).then(run => setRun(run)),
        500, props.active);

    React.useEffect(() => {
        if (props.active) {
            getLatestRun(props.build.id)
                .then(run => setRun(run));
        }
    }, [props.active]);

    const onEnqueueClick = () => {
        enqueueNewRun(props.build.id)
            .then(() => show(enqueueNewRunModal))
            .then(() => delay(1000))
            .then(hide)
            .then(() => delay(1000))
            .then(getAll);
    }

    const onDeleteClick = () => {
        show(deleteBuildModal);
    }

    const onDeleteConfirm = () =>
        remove(props.build.id)
            .then(() => getAll())
            .then(hide);

    const onEditClick = () => {
        go(props.build.id);
    }

    return <div className="build__component">
        <Modal name={enqueueNewRunModal}>Build enqueued.</Modal>
        <Modal name={deleteBuildModal}>
            <GenericModalLayout title="Delete build" actionsRenderer={() => <>
                <button type="button" className="neui-button __action" onClick={hide}>Nevermind</button>
                <button type="button" className="neui-button __action --danger" onClick={onDeleteConfirm}>Yea, do it</button>
            </>}>
                You are going to delete build "{props.build.name}".<br />
                You sure? Delete? Really?
            </GenericModalLayout>
            
        </Modal> {/*Sure, go on*/}
        <div className="__header">
            <div className="__row">
                <div className="__column" onClick={() => props.onHeaderClick && props.onHeaderClick()}>
                    <div className="__column-header">Build</div>
                    <div className="__column-content">{props.build.name}</div>
                </div>
                <div className="__column" onClick={() => props.onHeaderClick && props.onHeaderClick()}>
                    <div className="__column-header">Status</div>
                    <div className={`__column-content ${props.build.status === 'Running' ? '--success' : ''}`}>{props.build.status}</div>
                </div>
                <div className="__column __actions-column">
                    <div className="__actions-container">
                        <button className="neui-element-flat __action" onClick={onEnqueueClick}>
                            <FaPlay color="#3b9c3a" />
                        </button>
                        <button className="neui-element-flat __action" onClick={onEditClick}>
                            <FaPencilAlt />
                        </button>
                        <button className="neui-element-flat __action" onClick={onDeleteClick}>
                            <FaTrashAlt />
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <div className="__content">
            <div className="__row">
                <div className="__column --baseline">
                    {props.build.steps.length > 0 && <>
                        <div className="__column-header">Steps</div>
                        <div className="__column-content">
                            <div className="__steps-container">
                                {props.build.steps.map((step, i) =>
                                    <div
                                        className={`__step ${step.status === 'Running' ? '--active' : ''}`}
                                        key={i}>{step.name}</div>)}
                            </div>
                        </div>
                    </>}
                </div>
                <div className="__column">
                    {run && run.logs.length > 0 && <>
                        <div className="__column-header">Log</div>
                        <div className="__column-content">
                            <div className="__column-overflow-container">
                                <BuildTerminalLog logs={run.logs} />
                            </div>
                        </div>
                    </>}
                </div>
            </div>
        </div>
    </div>;
}, (prev, next) => {
    return prev.active === next.active &&
        !diff(prev.build, next.build);
});