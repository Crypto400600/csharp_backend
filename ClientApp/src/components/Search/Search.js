import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import "./Search.css";

export class Search extends Component {
    displayName = Search.name

    render() {
        return (
            <div className="search-component">
                <form className="search-form">
                    <input type="text" className="search-input" placeholder="Search for anything..." required />
                    <input type="image" name="submit" className="search-submit-btn" border="0" alt="Submit" src={require("../../ assets/search.svg")} />
                </form>
                <div className="search-suggestions">
                    <Link to="/results" className="search-suggestion">Why is Charles GOATED?</Link>
                    <Link to="/results" className="search-suggestion">Why is Nifemi the great dev alive?</Link>
                    <Link to="/results" className="search-suggestion">Lorem ipsum</Link>
                </div>
            </div>   
         )
    }

}