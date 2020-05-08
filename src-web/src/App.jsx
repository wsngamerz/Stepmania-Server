import React from 'react';
import Loadable from 'react-loadable';
import { Layout, Menu } from 'antd';
import { Router, Link, Location } from '@reach/router';

import logo from './stepmaniaserver_logo.svg';

import Loading from './components/Loading';

const { Header } = Layout;
const { SubMenu } = Menu;
const loadingDelay = 500;

const LoadableHome = Loadable({
    loader: () => import("./pages/Home"),
    loading: Loading,
    delay: loadingDelay
});

const LoadableLeaderboardSongs = Loadable({
    loader: () => import("./pages/LeaderboardSongs"),
    loading: Loading,
    delay: loadingDelay
});

const LoadableLeaderboardUsers = Loadable({
    loader: () => import("./pages/LeaderboardUsers"),
    loading: Loading,
    delay: loadingDelay
});

const LoadableListRooms = Loadable({
    loader: () => import("./pages/ListRooms"),
    loading: Loading,
    delay: loadingDelay
});

const App = () => (
    <Layout className="app">
        <Header style={{ width: '100%' }}>
            <img src={logo} className="logo" alt="StepmaniaServer Logo" />
            <Location>
                {props => {
                    return (
                        <Menu theme="dark" mode="horizontal" selectedKeys={[props.location.pathname]}>
                            <Menu.Item key="/">
                                <Link to="/">Home</Link>
                            </Menu.Item>

                            <SubMenu key="leaderboards" title="Leaderboards">
                                <Menu.Item key="/leaderboards/users">
                                    <Link to="/leaderboards/users">Users</Link>
                                </Menu.Item>
                                <Menu.Item key="/leaderboards/songs">
                                    <Link to="/leaderboards/songs">Songs</Link>
                                </Menu.Item>
                            </SubMenu>

                            <Menu.Item key="/rooms">
                                <Link to="/rooms">Live Rooms</Link>
                            </Menu.Item>
                        </Menu>
                    )
                }}
            </Location>
        </Header>
        <Router>
            <LoadableHome path="/" />
            <LoadableLeaderboardSongs path="/leaderboards/songs" />
            <LoadableLeaderboardUsers path="/leaderboards/users" />
            <LoadableListRooms path="/rooms" />
        </Router>
    </Layout>
);

export default App;
