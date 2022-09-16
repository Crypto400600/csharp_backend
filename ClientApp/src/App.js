import React, { Component } from 'react';

import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './pages/Home/Home';
import { ResultPage } from './pages/Results Page/Results';
import { UploadPage } from './pages/Upload Page/Upload';

export default class App extends Component {
  displayName = App.name

  render() {
      return (
          
              <Layout>
                  <Route exact path='/' component={Home} />
                  <Route path="/results" component={ResultPage} />
                  <Route path="/upload" component={UploadPage} />
              </Layout>
        
    );
  }
}
