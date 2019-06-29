FSharp.MongoDB
==========================

Adapted from [NamelessInteractive.FSharp](https://github.com/NamelessInteractive/NamelessInteractive.FSharp/tree/master/NamelessInteractive.FSharp.MongoDB)


# Getting Started

## Installation of Library
```
git clone https://github.com/afshawnlotfi/FSharp.MongoDb.git
cd FSharp.MongoDb
dotnet restore
dotnet build
```

## Linking

```
dotnet add reference ./PATHTOLIBRARY/FSharp.MongoDb/FSharp.MongoDB.fsproj
```

## Using

```
// Startup.fs
open FSharp.MongoDB.SerializationProviderModule

member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =

    ....

    Register()

```

# License

Everything in this repository is licensed under the MIT License unless otherwise specified.


# Disclaimer

All actions with this software are strictly your responsibility. This project is not held accountable for any loss of data or damage to the user whatsoever.
