import React from 'react';
import { Breadcrumb, Layout } from 'antd';

const { Content } = Layout;



const PageLayout = props => (
    <Content className="site-layout" style={{ padding: '0 50px' }}>
        <Breadcrumb style={{ margin: '16px 0' }}>
            {props.breadcrumbs.map(breadcrumb =>
                <Breadcrumb.Item>{breadcrumb}</Breadcrumb.Item>
            )}
        </Breadcrumb>
        <div className="site-layout-background" style={{ padding: 24, minHeight: 380 }}>
            { props.children }
        </div>
    </Content>
)

export default PageLayout;
