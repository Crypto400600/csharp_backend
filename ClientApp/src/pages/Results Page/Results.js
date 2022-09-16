import React, { Component } from "react"
import { Link } from "react-router-dom";
import Loader from "../../components/Loader/loader"
import './Results.css'
import { SearchResult } from "../../components/SearchResult/SearchResult";
import { Search } from "../../components/Search/Search";

export class ResultPage extends Component {
    displayName = ResultPage.name

    constructor(props) {
        super(props);
        this.state = { loadingState: false };
    }

    render() {
        return (
            < div className="results-page" >
                {
                    this.state.loadingState === true ? <Loader /> :
                        <main>
                            <div className="results-page-header">
                                <div className="top-nav">
                                    <Link to="/" className="logo">
                                        <img src={require("../../ assets/logo.svg")} alt="Team logo" />
                                    </Link>
                                    <Search/>
                                </div>
                                
                                <div className="result-info">
                                    <h2>Search results for ""</h2>
                                    <h5 className="matches-found">20 match(es) found</h5>
                                    <p className="response-time">Response time: </p>
                                </div>
                            </div>
                            <div className="results-body">
                                <SearchResult type="html" />
                                <SearchResult type="json" />
                                <SearchResult type="word" />
                                <SearchResult type="xls" />
                                <SearchResult type="json" />
                            </div>
                        </main>
                }
            </div >
        )
    }
}

export default ResultPage;