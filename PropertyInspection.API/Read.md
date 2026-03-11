PropertyInspection.SaaS
 ├─ PropertyInspection.API           → Controllers, Program.cs, appsettings.json
 ├─ PropertyInspection.Application   → Business logic / Services
 ├─ PropertyInspection.Core          → Entities, Enums
 ├─ PropertyInspection.Infrastructure → DbContext, Identity classes, EF Core setup
 └─ PropertyInspection.Shared        → DTOs


 # Login Flow

 
/***Request aayi
 → JWT verify hua
   → IdentityUserId mila
     → Domain User mila
       → UserRoles niklay   
         → Roles niklay
           → RolePermissions nikli
             → Permission match?
               → Allow / Deny***/

dotnet ef database drop --force
dotnet ef migrations add InitialCreate -p ..\PropertyInspection.Infrastructure -s . -c AppDbContext -o Migrations
dotnet ef database update -p ..\PropertyInspection.Infrastructure -s .

dotnet ef migrations add UpdateLookupEntities -p ..\PropertyInspection.Infrastructure -s . -c AppDbContext -o Migrations










Testing Types Overview

| Type             | Scope                 | Tool Examples        | Purpose                            |
| ---------------- | --------------------- | -------------------- | ---------------------------------- |
| Unit             | Single function/class | xUnit, NUnit, MSTest | Code correctness                   |
| Integration      | Multiple modules      | xUnit + TestServer   | Interaction correctness            |
| E2E / Functional | Full workflow         | Selenium, Playwright | User perspective correctness       |
| Acceptance       | Requirements          | SpecFlow, Cucumber   | Business requirement verification  |
| Performance      | Load/Stress           | JMeter, k6           | Scalability check                  |
| Security         | Vulnerabilities       | OWASP ZAP, Burp      | Protect data & system              |
| Regression       | Existing features     | Automated tests      | No breakage of old code            |
| Smoke/Sanity     | Quick check           | Manual/automated     | Build sanity / minor feature check |


