# Trecco - Kanban Board Clone in .NET

**Trecco** is a Trello-inspired Kanban board application built with **.NET 9**, using **Vertical Slice Architecture**, **MongoDB**, **SignalR**, and modern .NET practices.  
This project demonstrates **clean architecture, feature-based modularization, and real-time collaboration**.

## Features

- **Boards & Lists**: Create boards and organize them with multiple lists.  
- **Cards**: Add, edit, move, and delete cards within lists.  
- **Card Movement**: Drag and drop cards between lists, with automatic position adjustments.  
- **Real-time Updates**: Uses **SignalR** to propagate board updates instantly to connected clients.  
- **Error Handling**: FluentResults pattern for consistent and clean error handling.  
- **Unit & Integration Tests**: Fully tested domain and features.  

## Architecture & Technologies

- **Vertical Slice Architecture**: Organizes code by feature (Board, List, Card), making the system modular and maintainable.  
- **.NET 9 & C#**: Modern, performant backend.  
- **MongoDB**: NoSQL database for flexible board and card data structures.  
- **SignalR**: Real-time updates for board collaboration.  
- **FluentResults**: Standardized result pattern for handling success and failure in features.  
- **xUnit + Moq**: Unit testing for domain and application layers.  

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)  
- [MongoDB](https://www.mongodb.com/) running locally or via Docker  

### Clone the repository

```bash
git clone https://github.com/L968/Trecco.git
cd Trecco
```

### Run the Application

```bash
dotnet run --project Trecco.Api
```

The API will start on `http://localhost:5000` (or another configured port).  

## Testing

Run all unit tests:

```bash
dotnet test
```

The project includes tests for:  

- Domain logic (Board, List, Card)  
- Features (CreateCard, MoveCard, etc.)  
- Error scenarios and FluentResults handling  

## Real-time Collaboration

Trecco uses **SignalR** for real-time updates. Events include:  

- `CardCreated`  
- `CardMoved`  
- `CardDeleted`  
- `ListCreated`  
- `ListRenamed`  
- `ListDeleted`  
- `BoardUpdated` (for bulk changes)

This allows multiple users to **see changes instantly** without refreshing the page.  

## Contributing

Contributions are welcome! Open issues or submit pull requests to improve the project.  

## License

This project is licensed under the [MIT License](LICENSE.txt).  
