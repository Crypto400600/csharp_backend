import React from 'react'
import "./loader.css"

const Loader = () => {
    return (
        <div className="loading-screen">
            <div className="loading-icon">
                <div className="lds-facebook"><div></div><div></div><div></div></div>
            </div>
            <h3 className="loading-text">Fetching data</h3>
        </div>
        )
}

export default Loader;