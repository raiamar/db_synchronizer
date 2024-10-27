# DB Synchronizer
A desktop application developed in .NET (C#) that fetches data from a source database (MSSQL) and synchronizes it with a target database (SQLite using DB Browser) at user-defined intervals. 
The application will also log the details of each synchronization operation, maintaining records of changes to ensure traceability and integrity of data.

## Features
- Automatically syncs data to a local SQLite database (viewable with DB Browser) at intervals set by the user.
- Logs each sync operation and tracks any data changes since the last sync, enabling easy monitoring of updates.
- Supports manual syncing to update data on demand.
- Displays fetched data in a user-friendly format with built-in search functionality.


## Tables 
- **Customer(CustomerID, Name, Email, Phone):** customer credential
- **Location(LocationID, CustomerID, Address):** one-many relation with customer
- **SyncLog(SyncID, SyncTime, Description):** keeps sync log
- **ChangeLog(ChangeID, TableName, ColumnName, RecordID, OldValue, NewValue, ChangeTime, Description):** keeps detail log of changed value


## Workflow
- Automatically or manually fetch data from the MSSQL database.
- Synchronize the fetched data with the SQLite database to ensure consistency.
- Log each synchronization event in the SyncLog table.
- If any value is changed keeps track of it.
- Set up triggers on Customer and Location tables to detect and log changes automatically.