# Grammophone.Storage.Azure
This .NET framework library adapts Azure blob storage to the abstract contract of a file system set by library
[Grammophone.Storage](https://github.com/grammophone/Grammophone.Storage/), which must reside in a sibling folder.

Use your preferred dependency injection framework to obtain a singleton `IStorageProvider` instance
implemented by the `AzureStorageProvider` class.
