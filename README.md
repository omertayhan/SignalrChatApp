## Project Detail

Using .NET Core 8 Entity Framework and SignalR enables general, group-based, and private communication options.

# Project Setup Guide

To run this project on your system, ensure that you have the following prerequisites installed:

1. **Visual Studio 2022 or Visual Studio Code**
2. **.NET 8 SDK**
3. **SQL Server (Preferred)** - You can use any other database, but ensure that you have the correct connection string.

## Steps to Set Up the Project:

### 1. Install Required Tools

- Install [Visual Studio 2022](https://visualstudio.microsoft.com/downloads) or [Visual Studio Code](https://code.visualstudio.com/).
- Install [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

### 2. Configure Database Connection

- Make changes to the connection string in the `AppSettings.json` file.

### 3. Run Migration Command

#### Using Visual Studio:
1. Navigate to **Tools** => **NuGet Package Manager** => **Package Manager Console**.
2. Run the following command: `update-database`.

#### Using Visual Studio Code:
1. Open the terminal.
2. Run the following command: `dotnet ef database update`.

## Additional Notes:

- Ensure that your SQL Server is up and running before executing the migration command.
- Double-check the connection string in `AppSettings.json` to avoid any connection issues.

With these steps completed, your project should be ready to run. If you encounter any issues during setup, refer to the project contributors.

## Project Screenshots:
#### Login Page
![login](https://github.com/omertayhan/SignalrChatApp/assets/62504339/75ee00bc-f07d-484d-80ce-5acfd6dbeef6)

#### Register Page
![register](https://github.com/omertayhan/SignalrChatApp/assets/62504339/7f8f1b39-9541-40d0-872e-00841ca04a60)

#### Public Channel Page
Communicate with anyone connected to the chat room.
![public ](https://github.com/omertayhan/SignalrChatApp/assets/62504339/e0b500c6-a258-47fb-a9b1-9a337c46f879)

#### Group Channel Page
Communicate with users connected to a specific group.
![group](https://github.com/omertayhan/SignalrChatApp/assets/62504339/c84ecf47-9676-45ec-a4d2-08f25dcbe9d6)

#### Private Channel Page
Communicate directly with another user.
![private](https://github.com/omertayhan/SignalrChatApp/assets/62504339/00978db3-f6ea-4ffc-84bc-63ed1aae6d4b)



