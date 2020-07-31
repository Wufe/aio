import * as React from 'react';
import loadable from '@loadable/component';
import { Route } from 'react-router-dom';
import './app.scss';
import { Build } from '../pages/build/build';
import { useDashboardPageLoad } from '../pages/dashboard/dashboard-hook';
import { useBuildAPI, usePolling } from '../pages/build/build-hook';
import { useSelector, useDispatch } from 'react-redux';
import { TGlobalState } from '~/state/reducer';
import { TIdentity, AppAction } from '~/state/app/app-state';
import { Identity } from '~/state/identity/identity';
import { GuardedRoute } from '../pages/build/guarded-route/guarded-route';
import Oidc from 'oidc-client';

export const App = () => {

    const { getAll } = useBuildAPI();
    const [isAuthenticated, setIsAuthenticated] = React.useState<boolean>(false);

    const { go } = useDashboardPageLoad();
    const DashboardPage = loadable(() => import('~/components/pages/dashboard/dashboard'));
    const LoginCallbackPage = loadable(() => import('~/components/pages/login-callback/login-callback'));
    const BuildPage = loadable(() => import('~/components/pages/build/build'));;

    const onLogoClick = () => go();

    const identity = useSelector<TGlobalState, TIdentity>(x => x.app.identity);
    const dispatch = useDispatch();

    usePolling(getAll, 3000, identity.logged && !!identity.token);

    const login = () =>
        Promise.resolve()
            .then(() => dispatch({ type: AppAction.SET_LOADING, payload: true }))
            .then(() => Identity.Instance.manager.signinPopup())
            .then(user => {
                dispatch({ type: AppAction.SET_LOGGED, payload: true });
                dispatch({ type: AppAction.SET_ACCESS_TOKEN, payload: user.access_token });
            })
            .catch(() => dispatch({ type: AppAction.SET_LOGGED, payload: false }))
            .finally(() => dispatch({ type: AppAction.SET_LOADING, payload: false }));

    const logout = () =>
        Promise.resolve()
            .then(() => dispatch({ type: AppAction.SET_LOADING, payload: true }))
            .then(() => Identity.Instance.manager.signoutRedirect())
            .then(() => location.href = '/')
            .finally(() => dispatch({ type: AppAction.SET_LOADING, payload: false }));

    React.useEffect(() => {

        const authenticated = !!identity.logged && !!identity.token;

        if (authenticated)
            getAll();

        setIsAuthenticated(authenticated);
        
    }, [identity.logged, identity.token])
    
    React.useEffect(() => {
        Identity.Instance.manager.events.addAccessTokenExpired(console.log);
        Identity.Instance.manager.events.addSilentRenewError(console.log);
        Oidc.Log.logger = console
        Identity.Instance.manager.getUser().then(function (user) {
            if (user) {
                console.log("User logged in", user);
                // Identity.Instance.manager.querySessionStatus()
                //     .then(console.log);
                dispatch({ type: AppAction.SET_LOGGED, payload: true });
                dispatch({ type: AppAction.SET_ACCESS_TOKEN, payload: user.access_token });
            } else {
                console.log("User not logged in");
                dispatch({ type: AppAction.SET_LOGGED, payload: false });
            }
        });
    }, []);

    return <div className="app__component">
        <div className="__header">
            <h1 onClick={onLogoClick} className="__logo">Aio</h1>
            <div className="__actions-container">
                {identity.logged !== undefined && <>
                    {!identity.logged && <button type="button" onClick={login} className="neui-element-flat neui-button">Login</button>}
                    {identity.logged && <button type="button" onClick={logout} className="neui-element-flat neui-button">Logout</button>}
                </>}
            </div>
        </div>
        <Route exact path="/">
            <DashboardPage />
        </Route>
        <Route path="/build/:id">
            <GuardedRoute component={BuildPage} authorized={isAuthenticated} />
        </Route>
        <Route path="/login-callback">
            <LoginCallbackPage />
        </Route>
    </div>;
};