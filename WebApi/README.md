```
# Movie Playlist API

## Overview
The Movie Playlist API is a web application built with ASP.NET Core and Microsoft Azure services. It allows users to manage their movie playlists, track their progress, and store interaction history with the movies in the playlist.

## Features
- **User Management**: Users can create and manage their playlists (attach, start and stop)
- **Progress Tracking**: Can track the users progress.
- **Interaction History**: The API records the interaction history of users with their playlists.

## Tech Stack
- **Backend**: ASP.NET Core
- **Database**: Microsoft Azure Cosmos DB
- **Blob Storage**: Azure Blob Storage for file handling
- **Queue Storage**: Azure Queue Storage for asynchronous processing
- **Testing**: MSTest for unit and component testing (**Needs Improvement**)

## Getting Started

### Prerequisites
- [Azure Account](https://azure.microsoft.com/free/) (for Cosmos DB and Blob Storage)

### Installation
1. Clone the repository:
   ```bash
   git clone [https://github.com/cohenyaniv/MoviePlaylistAPI.git](https://github.com/cohenyaniv/MoviePlaylistAPI.git)
   cd MoviePlaylistAPI

### Configuration
 - Need to update the appsettings.json file in case you want to use your own azure account

### Movie Playlist Data
- The available movie lists are currently stored in a JSON file under the `ResourceMock` folder. This is a temporary solution and in a production environment, the data source would likely be a database or another more robust storage solution.

### Starting Playlist
- Users need to attach themselves to a playlist before starting it. This behavior should be clearly documented in the API documentation.

## Open Issues

* **Swagger Documentation:** even that it was requested without swagger, I kept it for debugging and to avoid using postman.
* **Testing:**
    * There are currently limited unit tests. Expanding unit test coverage is highly recommended.
    * Component tests are entirely missing. Implementing component tests is crucial for ensuring proper functionality of individual components.
* **Blob Naming:** Blob names currently lack meaningfulness. Implementing a clear naming convention for blobs would improve code maintainability.
* **Logging:** Logging functionality is currently missing. Implementing logging would be beneficial for debugging and monitoring purposes.

```
