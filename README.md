# TASK 1 Setup

Local Azure Functions Demo Guide for RandomApi

This guide provides a quick overview for running and testing Azure Functions locally, focusing on blob and table storage interactions using Azurite and Microsoft Azure Storage Explorer.

## Prerequisites

- Ensure [.NET 6.0 SDK](https://dotnet.microsoft.com/download) is installed. (Visual studio 2022)
- Install [Azurite](https://github.com/Azure/Azurite) for local storage emulation.
- Can use [Microsoft Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/) to inspect Blob Containers and Tables locally.
- Can use [Insomnia](https://insomnia.rest/download) for runing the GET commands

## Configuration

1. **Azurite Setup**: Before running the application, start Azurite to emulate Azure Storage services. Check its installation and running status as it's crucial for local testing.
2. **Firewall Configuration**: For local demo purposes, ensure your firewall settings allow traffic to and from Azurite and your local Azure Functions.
3. **Local Settings**: A `local.settings.json` file is included for demo purposes, configuring necessary connections to Azurite.

## Running the Application

The application targets `.NET 6.0` and uses a `TimerTrigger` function named `FetchDataFunction` 
configured to run every minute (`0 */1 * * * *`). 
This function fetches data from `https://api.publicapis.org/random?auth=null` and stores it in the local storage emulated by Azurite.

### Exploring Data

Run the project in Visual Sudio.
Use Microsoft Azure Storage Explorer to connect to the local emulator (under Local > Storage Accounts > (Emulator - Default Ports)) to view Blob Containers and Tables. 
Remember to refresh the explorer to see the latest data.

### Fetching Logs

- **List Logs**: Access logs by calling `http://localhost:7071/api/ListLogs` with parameters `from` and `to` specifying the date range 
(e.g., `from=2024-02-20T19:13:00&to=2024-02-21T19:13:00`).

### Retrieving Payload

- **Fetch Payload**: Use the `RowKey` value obtained from the log entries to fetch specific payloads by calling `http://localhost:7071/api/FetchPayload`. 
Include `rowKey` and a `guid` value in the parameters (e.g., `rowKey=0b0cdbc1-6a87-480a-aa02-1daa1f928220`).

## Notes

- This setup is intended for local development and demo purposes only.
- Make sure to replace placeholder example values with actual data when making requests.

## Localhost Access

When running Azure Functions locally, the URL is typically `http://localhost:7071`. However, this can vary:

- **Check Console**: Running the project in Visual Studio 2022 displays a console window with the actual ports used.
- **Port Variability**: If the default port is unavailable, the system will use the next available port, changing the URL.
- **VS 2022 Output**: You can refer to the Visual Studio 2022 console output for the exact address and port.
