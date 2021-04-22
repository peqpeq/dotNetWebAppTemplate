 
 // Create migration
 dotnet ef migrations add <MigrationName> --project DAL.App.EF --startup-project WebApp 
 
 // Update database
 dotnet ef database update --project DAL.App.EF --startup-project WebApp 
