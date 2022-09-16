# Search Engine Project

This repository contains a search engine project with the following parts

- Engine

This is the main engine class library that handles indexing the documents and querying the indexes

- Engine.Test

This contains the tests which are used to verify that the Engine is performing the actions that it should

- GUI

This is a GUI written in [WPF](https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf). 
It consumes the Engine class library directly and provides a UI for searching documents and uploading new documents to be indexed 

- API

This is a rest API written with .NET framework that consumes the Engine and provides endpoints for searching and indexing documents

## How to Run/Build
- Engine

You can set either

1. Set the mongodb uri via the Connector.SetMongoUri 
2. Install mongodb locally and run it. The connector defaults to using a local instance

- Engine.Test

Ensure the Engine project is built then open this project and run it in any IDE of your choice

- GUI

Ensure the Engine project is built.
Setup your AWS secrets and credentials using the instructions [here](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/quick-start-s3-1-cross.html).

- API

Ensure the engine project is built.
Install the required server for running the project eg [Windows](https://docs.microsoft.com/en-us/iis/extensions/introduction-to-iis-express/iis-express-overview)