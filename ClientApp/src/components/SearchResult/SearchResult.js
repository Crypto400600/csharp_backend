import React, { Component } from "react"
import "./SearchResult.css"


export class SearchResult extends Component {
    displayName = SearchResult.name

    constructor(props) {
        super(props);
        this.state = { docIcon: <img src={require("../../ assets/document.svg")} alt="doc-type" />, type: props.type }
    }

    componentDidMount() {
        switch (this.state.type) {
            case "pdf":
                this.setState({ docIcon: <img src={require("../../ assets/pdf.svg")} alt="doc-type" /> });
                break;
            case "html":
                this.setState({ docIcon: <img src={require("../../ assets/html.svg")} alt="doc-type" /> });
                break;
            case "ppt":
                this.setState({ docIcon: <img src={require("../../ assets/powerpoint.svg")} alt="doc-type" /> });
                break;
            case "txt":
                this.setState({ docIcon: <img src={require("../../ assets/txt.svg")} alt="doc-type" /> });
                break;
            case "json":
                this.setState({ docIcon: <img src={require("../../ assets/json.svg")} alt="doc-type" /> });
                break;
            case "xls":
                this.setState({ docIcon: <img src={require("../../ assets/excel.svg")} alt="doc-type" /> });
                break;
            case "word":
                this.setState({ docIcon: <img src={require("../../ assets/word.svg")} alt="doc-type" /> });
                break;
            default:
                this.setState({ docIcon: <img src={require("../../ assets/document.svg")} alt="doc-type" /> });
        }
    }

    render() {
        return (
            <div className="search-result">
                <div className="search-result-img">
                    { this.state.docIcon}
                </div>
                <div className="search-result-info">
                    <div className="result-data">
                        <a className="title">Title</a>
                        <p className="description">Desc</p>
                    </div>
                    <p className="date-added">17/08/2021</p>
                </div>
            </div>
        )
    }
}