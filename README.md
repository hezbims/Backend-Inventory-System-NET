## Run The App
Make sure you are running the app from linux environement with `docker compose` and `.NET SDK` installed.


If you are using Windows OS, download the WSL and move this project to linux file system to avoid performance drawback. 
You can also use remote development tools such as Rider WSL Remote Development tools or Jetbrains Gateway to develop this app in WSL environement.

Here is the steps to run the app :
1. Run this command in the terminal on the root of project :
    ```
    docker compose up -d   
    ```

2. Wait for the SQL Server container to successfully running. You can check it using this command :
    ```
    docker exec -it inventory-system-local-sql-server /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Strongpwd123_"
    ```
    The container is successfully running if the resulting output is like this :
    ```
    1>
    ```

3. After that, you can run the app using `dotnet run` in the `Inv_Backend_NET` folder, or you can also use your favorite IDE to run the app.

4. If you are using Windows, you can check the URL of the linux subsystem that is used to run this app by using this command in the linux terminal :
    ```
    hostname -I
    ```

### Development Data Seeding
you can run this command in the `Inv_Backend_NET` folder to seed the initial data to the database for development purpose :
```
dotnet run refresh test-seeder
```


## Target Project Structure
This is the expected project structure after refactoring. It is generated using [tree.nathanfriend.com](https://tree.nathanfriend.com/)
### 1. Main Project Structure
<!--
Editable tree source (for future editing):
Inv_Backend_NET
  Feature
    Transaction # using strict clean architecture because of rich domain logic
      Application
        Handler
        Dto
        Read Service
      Domain
        Exception
        Entity
        Dto
        Mapper
        Value Object
      Presentation
        Dto
        Error_Translator
        Controller
    Pengajuan # Same as transaction, but this is legacy
    Product # using simple MVC
      Database
        Model/Entity
        Configuration
      Controller
      Handler
      Dto
    Reporting
    Authentication
      
-->
```
Inv_Backend_NET
└── Feature
    ├── Transaction # using strict clean architecture because of rich domain logic
    │   ├── Application
    │   │   ├── Handler
    │   │   ├── Dto
    │   │   └── Read_Service
    │   ├── Domain
    │   │   ├── Exception
    │   │   ├── Entity
    │   │   ├── Dto
    │   │   ├── Mapper
    │   │   └── Value Object
    │   └── Presentation
    │       ├── Dto
    │       ├── Error_Translator
    │       └── Controller
    ├── Pengajuan # Same as transaction feature, but this is legacy
    ├── Product # using simple MVC and Transaction Script
    │   ├── Database
    │   │   ├── Model/Entity
    │   │   └── Configuration
    │   ├── Controller
    │   ├── Handler
    │   └── Dto
    ├── Reporting
    └── Authentication
```
There is **4** main features : `Transaction`, `Product`, `Reporting`, and `Authentication`

### 2. Test Project Structure
<!--
Editable tree source (for future editing):
Inventory_Backend.Tests
  Seeder
    ProductSeeder
    TransactionSeeder
    UserSeeder
  Test_Configuration
    Constant
    Fixture
    Collection_Definition
    Logging
  Feature
    Transaction
      Integration
        Scenario1/Requirement1
        Scenario2/Requirement2
      Unit
        Scenario1/Requirement1
        Scenario2/Requirement2
    Pengajuan
    Product
    Reporting
    Authentication
    Main_Setup
-->
```
Inventory_Backend.Tests
├── Seeder
│   ├── ProductSeeder
│   ├── TransactionSeeder
│   └── UserSeeder
├── Test_Configuration
│   ├── Constant
│   ├── Fixture
│   ├── Collection_Definition
│   └── Logging
└── Feature
    ├── Transaction
    │   ├── Integration
    │   │   ├── Scenario1/Requirement1
    │   │   └── Scenario2/Requirement2
    │   └── Unit
    │       ├── Scenario1/Requirement1
    │       └── Scenario2/Requirement2
    ├── Pengajuan
    ├── Product
    ├── Reporting
    ├── Authentication
    └── Main_Setup
```
Integration testing currently utilize XUnit `Collection Fixture`. The container will be run once 
within all Integration Test using `TestWebAppFactory.cs`.