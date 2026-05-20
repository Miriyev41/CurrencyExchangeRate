# Currency Exchange Rate

**Course name: **Network Application Development  
**Project title: **Global Currency Exchange Terminal   
**Author name(s): **Mirparvin Miriyev
**Student ID number(s): **64523 

## Short Description of the Project Functionality
This project is a robust client-server desktop application designed to simulate a real-world currency exchange office. Built using a WPF frontend and a WCF backend service, it allows users to manage virtual wallets and execute cross-currency trades.

**Key Features:**
* **Live Exchange Rates:** Integrates directly with the National Bank of Poland (NBP) REST API to fetch real-time, accurate currency exchange rates.
* **Virtual Wallets & Transactions:** Users can deposit funds into a virtual account and seamlessly exchange between currencies (e.g., USD, EUR, PLN, TRY) using calculated cross-rates.
* **Persistent Data Storage:** Utilizes Entity Framework and SQL Server to securely store user accounts, wallet balances, and a complete transaction history.
* **Historical Market Research:** Includes a built-in archive tool to query historical exchange rates from the NBP API for specific past dates.

### Instructions on How to Run the Project

Follow these steps carefully to ensure both the backend service and frontend client launch successfully:

### 1. Database Initialization (Entity Framework)
Before running the application for the first time, you must create the local database.
1. Open the `ExchangeOffice.sln` solution in Visual Studio.
2. Go to **Tools** -> **NuGet Package Manager** -> **Package Manager Console**.
3. In the console, ensure the **Default project** dropdown is set to `ExchangeOffice.Service`.
4. Type `Update-Database` and press Enter. This will apply the migrations and create the necessary SQL tables.

### 2. Configure Multiple Startup Projects
Because this is a client-server architecture, the WCF Service must launch alongside the WPF Client.
1. Right-click the top-level **Solution 'ExchangeOffice'** in the Solution Explorer.
2. Select **Properties**.
3. In the left menu, select **Startup Project**.
4. Choose the **Multiple startup projects** radio button.
5. In the list, set the `Action` dropdown to **Start** for both `ExchangeOffice.Service` and `ExchangeOffice.WpfClient`.
6. Use the up/down arrows to ensure `ExchangeOffice.Service` is listed **above** the WpfClient. Click **OK**.

### 3. Launch the Application
1. Press **F5** (or click the green **Start** button at the top of Visual Studio).
2. A browser window or background process will launch the WCF Service (IIS Express).
3. The WPF Dashboard will open automatically. You can now use the application to deposit funds and execute trades!

