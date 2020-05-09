import React, { Suspense, lazy } from 'react';
import { Layout, Menu } from 'antd';
import { Router, Link, Location } from '@reach/router';

import logo from './stepmaniaserver_logo.svg';

import Loading from './components/Loading';

const { Header } = Layout;
const { SubMenu } = Menu;

const Home = lazy(() => import('./pages/Home'));
const LeaderboardSongs = lazy(() => import('./pages/LeaderboardSongs'));
const LeaderboardUsers = lazy(() => import('./pages/LeaderboardUsers'));
const ListRooms = lazy(() => import('./pages/ListRooms'));

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
        <Suspense fallback={<Loading />}>
            <Router>
                <Home path="/" />
                <LeaderboardSongs path="/leaderboards/songs" />
                <LeaderboardUsers path="/leaderboards/users" />
                <ListRooms path="/rooms" />
            </Router>
        </Suspense>
    </Layout>
);

export default App;
