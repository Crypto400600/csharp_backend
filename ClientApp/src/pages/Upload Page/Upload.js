import React, { Component } from "react"
import { Link } from "react-router-dom"
import { Notify } from "../../components/Toast Notification/Notify";
import "./Upload.css"

export class UploadPage extends Component {
    displayName = UploadPage.name

    constructor(props) {
        super(props);
        this.state = {
            loadingState: false,
            title: "",
            description: "",
            url: "",
            document: "",
            notificationType: "",
            notificationMsg:""
        };
    }


    handleInputChange = (e) => {
        const { name, value } = e.target;
        this.setState({
            [name]: value
        })
    }

    handleFileUpload = () => {
        const uploadLink = "https://api.cloudinary.com/v1_1/dpgdjfckl/upload";
        const data = new FormData();
        data.append("file", this.state.document)
        data.append("upload_preset", "search-engine")


        fetch(uploadLink, {
            method: "POST",
            body: data
        })
            .then(response => {
                if (response.status === 200) {
                    response.json()
                } else {
                    throw new Error("Something went wrong");
                }
                
            })
            .then(data => this.setState({ url: data.url }))
            .catch(error => console.log(error.message))
    }

    handleSubmit = () => {
        this.handleFileUpload();

        if (this.state.url !== "") {
            const formData = { name: this.state.title, url: this.state.url, description: this.state.description };

        }
    }

    render() {
        
        return (
            <div className="upload-page">
                <div className="upload-page-header">
                    <Link to="/" className="logo">
                        <img src={require("../../ assets/logo.svg")} alt="team logo" />
                    </Link>
                    <div className="upload-page-info">
                        <h3 className="title">Upload content</h3>
                        <p className="subtitle">Fill the form with the appropriate information required</p>
                    </div>
                </div>
                <div className="upload-page-main">
                    <form onSubmit={this.handleSubmit}>
                        <div className="upload-form-input">
                            <label htmlFor="title">Title:</label>
                            <input type="text" name="title" id="document-title" required onChange={this.handleInputChange} />
                        </div>
                        <div className="upload-form-input">
                            <label htmlFor="description">Description:</label>
                            <textarea name="description" id="document-desc" onChange={this.handleInputChange}></textarea>
                        </div>
                        <div className="upload-form-input">
                            <label htmlFor="document">Choose a file to upload</label>
                            <input type="file" name="document" id="document" required onChange={this.handleInputChange} />
                        </div>
                        <input type="submit" value="Upload" className="submit-btn"/>
                    </form>
                    <p className="notice">
                        N.B: This information will be available on the search engine about 2 hours after upload
                    </p>
                </div>
                < Notify type={this.state.notificationType} message={this.state.notificationMsg} />
            </div>
            )
    }

}

