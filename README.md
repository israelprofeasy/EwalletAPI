# Ewallet
An e-wallet system that can be used by  three types of users, “Noob”, “Elite” and “Admin”.  The system is only accessible to authenticated  users.

# Tools
• Webapi - aspnet core web api
• Documentation - Swagger
• 0Auth - JWT
• Mapping - AutoMapper
• Data management - EFCore(Sql Server - database)
• Containerization - Docker
• Error Logging - nLog
• CICD - Github actions
• Testing - NUnit testing
• Deployment Platform - Heroku
• Frontend client - Angular

# Project Features
## User (Noob) 
• is only entitled to one currency (main) on signup
• All wallet funding must first be converted to main currency 
• All wallet withdrawals must first be converted to main currency
• Cannot change the main currency

## User (Elite) 
• Can have multiple wallets in different currencies with the main currency selected at signup.
• Funding in a particular currency should update the wallet with that currency or create it.
• Withdrawals in a currency with funds in the wallet of that currency should reduce the wallet balance for that currency.
• Withdrawals in a currency without a wallet balance should be converted to the main currency and withdrawn.
• Cannot change main currency

## User (Admin) 
• Cannot have a wallet.
• Cannot withdraw funds from any wallet.
• Can fund wallets for Noob or Elite users in any currency.
• Can change the main currency of any user.
• Can promote or demote Noobs or Elite users

## User (System)
• Sends confirmation mail to user on every successful transaction

# Other Features
• Heavy duty functionalities is running on their own independent thread
• The handled errors is logged in a file (File is named with the current date inside a folder called → <folder name>ErrorLogs) with the following details
• Error time and date
• Error message
• Each fresh entry of the above is separated with a next line
  
# Get Started with this app.
## On Windows using Visual studio IDE
Clone this repo and run the solution file, Preferably using visual studio 2019. Simply by clicking the solution file in the package named ('EwalletAPI.sln'). Visual studio IDE will open the solution file, and the internal server IIS will open it in your default browser

## On Windows Or Linux using VSCODE or (Other editor of choice)
Clone this repo and Change directory to the root folder 'Ewallet/EwalletAPI/Ewallet'. Open the folder containting the solution file in VSCode text editor (Preferably) or other text editor of choice. Execute a build command ('dotnet build') to resolve any dependencies. Execute the run command ('dotnet run')
