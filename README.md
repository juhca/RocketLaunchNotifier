Welcome to my "Rocket launch Notifier project"


This project retrieves the upcoming rocket launches (during next week), sends an e-mail notification to the saved e-mails in the SqLite DB or In-Memory DB.

When a new e-mail is added, the upcoming launches happening at least from today, are retreived from the DB and sent to the new email.

Every hour a bacground service retrieves new upcoming launches, in which I only retrieves launches that received an update from my last successfull api request.


![image](https://github.com/user-attachments/assets/dc219ccf-639c-49bd-8831-c2f9e4b8c293)

The app can be run in 2 way: with "in-memory DB" or with "SqLite DB".
This can be changed in the appsetting.json (option: datastore)

![image](https://github.com/user-attachments/assets/e2974e06-b7ca-4607-b7fc-0a0ccd8aec39)


**How to run**

Open VisualStudio/JetBrains Rider ~ Project PocketLauncherNotifier (should be set as start-up) And run Configuration "RocketLauncherNotifier: IIS Express", if everything is fine, SwaggerUI should open up in your default Internet browser.
*Changes for full operation*
Some things are redacted:
* appsettings.json: proper "EmailConfig" is needed
* InMemoryEmailRepository: proper emails are needed
* RocketLaunchesTests: Proper EmailConfig() is needed


* **Startup**

Every hour a background service is run to get the new values, so the user doesn't have to do anything.
The bacground service is inside "BackgroundServices/RocketLaunchBackgroundService.cs"

* **/api/rocket-launches/get-all-from-api**

  Retrieves data from the "https://ll.thespacedevs.com/2.3.0/launches" api.
  
  If startup it doesn't append "last_updated__gte" (which is equal to the last successful api call to the api).
  
  That is done, so in startup all upcoming are retrieved and then only the updates are retrieved, so sending e-mails service is simplified.
  In my case we presume that the user can subscribe to all launches and not to a specific (subset) launch.
  
* **/api/rocket-launches/get-saved**

Retrieves only launches saved from in the DB. The data is refreshed every hour/day (depends on the bg service), but the user can refresh it, with **get-all-from-api** call.

* **/api/rocket-launches/get-upcoming**

Retrieves only saved launches that had not happened yet (launch data >= Datetime.Utcnow)

* **api/email/save-email**
When saving an email an email notification is sent for the upcoming launches.

# Project Organization

* **ApiTests**
Contains api tests and the basic user flow.
  
* **UnitTests**
Unit tests for testing the service, only implemented rocketLaunchService since it's the main one
  
* **Domain**
  
It contains:
* Business Logic: Core rules (e.g., "a rocket launch notification must be sent 24 hours before launch").

* Entities: Business-critical models (e.g., RocketLaunch, EmailSubscription).

* Interfaces (Contracts): Abstraction of external dependencies (e.g., IRocketLaunchRepository, IEmailService).

Why It Exists:
* Ensures application’s core rules are independent of frameworks, databases, or external services.
* Promotes testability by allowing you to mock infrastructure dependencies.
* Provides a single source of truth for business rules.
  
* **Infrastructure**

Data Access: Database implementations (e.g., SQLite, Entity Framework Core).
Why It Exists:
* Allows swapping dependencies without rewriting core logic (e.g., switching from SQLite to PostgreSQL).
  
* **RocketLauncheNotifier**
Both controllers and the DTO's, also the implementation of the Background-service.


