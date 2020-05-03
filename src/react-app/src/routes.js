import React from 'react'
import { Switch, Route } from 'react-router-dom';
import PrivateRoute from './components/PrivateRoute';
import { Layout } from './components/Layout';


import Login from './components/Login';
import Register from './components/Register';
import TwoFactAuth from './components/TwoFactAuth';



export default function Routes() {
    return (
        <Switch>
            <Route exact path='/login' component={Login} />
            <Route exact path='/register' component={Register} />
            <Route exact path='/twofactauth' component={TwoFactAuth} />
            <PrivateRoute path="/" component={Layout} />
        </Switch>
    )
}