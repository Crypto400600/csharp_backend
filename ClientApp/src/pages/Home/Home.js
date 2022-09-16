import React, { Component } from 'react';
import { Link } from "react-router-dom"
import './Home.css';
import { Search } from '../../components/Search/Search';

export class Home extends Component {
    displayName = Home.name
    
   render() {
       return (
           <div className="landing-page">
               <div className="home-navbar">
                   <Link className="navbar-option" to="/upload">
                       <img src={require("../../ assets/add.svg")} alt="Add document"/>
                   </Link>
                   <Link className="navbar-option" to="/upload">
                       <img src={require("../../ assets/info.svg")} alt="Project description" />
                   </Link>
               </div>
               <div className="page-header">
                   <div className="logo-homepage">
                       <img src={require("../../ assets/logo.svg")} className="logo-img" alt="Team logo" />
                   </div>
                   <p className="page-subtitle"> A custom search engine built with React & C# </p>
                   <Search/>
               </div>
               <div className="supported-content">
                   <h3>This search engine allows you search for </h3>
                   <div className="supported-items">
                       <div className="supported-item">
                           <img src={require("../../ assets/coloured/excel.svg")} className="supported-item-img" alt="excel" />
                       </div>
                       <div className="supported-item">
                           <img src={require("../../ assets/coloured/html.svg")} className="supported-item-img" alt="txt" />
                       </div>
                       <div className="supported-item">
                           <img src={require("../../ assets/coloured/pdf.svg")} className="supported-item-img" alt="powerpoint" />
                       </div>
                       <div className="supported-item">
                           <img src={require("../../ assets/coloured/word.svg")} className="supported-item-img" alt="word" />
                       </div>
                       <div className="supported-item">
                           <img src={require("../../ assets/coloured/json.svg")} className="supported-item-img" alt="json" />
                       </div>
                   </div>
                   <p>and more</p>
               </div>
           </div>
    );
  }
}
