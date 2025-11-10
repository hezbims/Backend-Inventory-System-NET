# Main App Setup
1. Create a new file called `appsettings.Local.json` in the `Inventory_Backend_NET` folder.
2. Here is the structure of `appsettings.Local.json` :
```

```
3. Then set the environement variable `ASPNETCORE_ENVIRONMENT` to value `Local`. In linux, you can set it in `~/.bashrc` by adding this line :
```
export ASPNETCORE_ENVIRONMENT=Local
```

# Test Automation Setup
To run the automated tests, you must install docker in your local machine.

### For Windows
for windows, you must install WSL to be able to run docker. Run this command on the powershell :
```

```



# Target Project Structure
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
