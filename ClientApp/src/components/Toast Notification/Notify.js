import React, { Component } from 'react';
import "./Notify.css";

export class Notify extends Component {
    displayName = Notify.name

    constructor(props) {
        super(props);
        this.state = {
            notificationIcon: <img src={require("../../ assets/info-notification.svg")} alt="notify-type" />,
            type: props.type,
            message: props.message,
            showNotification: true
        }
    }

    componentDidMount() {
        switch (this.state.type) {
            case "success":
                this.setState({ notificationIcon: <img src={require("../../ assets/success.svg")} alt="notify-type" /> });
                break;
            case "error":
                this.setState({ notificationIcon: <img src={require("../../ assets/error.svg")} alt="notify-type" /> });
                break;
            case "warning":
                this.setState({ notificationIcon: <img src={require("../../ assets/warning.svg")} alt="notify-type" /> });
                break;
            default: 
                this.setState({ notificationIcon: <img src={require("../../ assets/info-notification.svg")} alt="notify-type" /> });
                break;
        }


    }

    componentDidUpdate() {
        if (this.state.showNotification === true) {
            setTimeout(() => {
                this.setState({ showNotification: false })
            }, 4000)
        }
    }

    componentWillReceiveProps() {
        if (this.state.message !== "" && this.state.type !== "") {
            this.setState({ showNotification: true })
        } else {
            this.setState({ showNotification: false })
        }
    }

    render() {
        return (
            <div className={`notify-container ${this.state.showNotification === true ? "show-notify" : "hide-notify"}`}>
                <div className="icon">
                    {this.state.notificationIcon}
                </div>
                <h5 className="message">
                    {this.state.message}
                </h5>
                <button onClick={() => this.setState({ showNotification: false })}>
                    <img src={require("../../ assets/close-btn.svg")} alt="close button"/>
                </button>
            </div>
            )
    }
}
