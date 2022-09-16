import React, { Component } from 'react';
import { Col, Grid, Row } from 'react-bootstrap';
import "./Layout.css";

export class Layout extends Component {
  displayName = Layout.name

    render() {

        
      return (
          <div className="layout">
              <Grid fluid>
                  <Row>
                      <Col sm={12}>
                          {this.props.children}
                      </Col>
                  </Row>
                  <footer>
                      <div className="links">
                          <a href="" className="doc-link">Documentation</a>
                          <a href="https://github.com/charles-co/search-engine" className="github-link">Github</a>
                      </div>
                      <p>C# group project 2019/2020</p>
                  </footer>
              </Grid>
          </div>
    );
  }
}
