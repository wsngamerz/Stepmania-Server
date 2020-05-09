import React from 'react';
import { Spin } from 'antd';



const Loading = () => (
    <div className="loading">
        <Spin tip="Loading ..." size="large" />
    </div>
);

export default Loading;
