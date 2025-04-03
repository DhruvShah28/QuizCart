# QuizCart

*QuizCart* is a collaborative and educational platform where members participate in academic assessments and share the cost of nutritious study ingredients (BrainFoods). It features a system for managing subjects, assessments, purchases, and educational food-based resources.

## Features

- *Member Management*
  - Add, update, and delete members.
  - Track member spending and calculate owed balances.
- *Subject Management*
  - Create, update, delete, and view subjects.
  - Link subjects to members.
- *Assessment System*
  - Manage assessments linked to specific subjects.
  - Associate assessments with brain food items (ingredients used for study sessions).
- *Brain Food Tracking*
  - Create and update brain food combinations (ingredient + assessment + quantity).
  - View nutritional benefits and associated purchases.
- *Purchase Management*
  - Record purchases of brain foods by members.
  - Calculate cost distribution based on usage.
- *Admin Web Pages (MVC Views)*
  - Razor views for each module with CRUD support and data binding.
  - Confirmation and error handling views included.

## Technologies Used

- *Backend*: ASP.NET Core MVC (C#)
- *Frontend*: Razor Pages
- *Database*: Microsoft SQL Server, Entity Framework Core
- *Architecture*: MVC Pattern with Dependency Injection and Repository-like Service Layer

## Design and Architecture

### Layered Structure

- *Controllers*: Handle web requests and responses.
- *Interfaces*: Contracts between services and controllers for loose coupling.
- *Services*: Business logic and data operations.
- *Models & DTOs*: Represent core data structures, view-specific objects, and transfer data between layers.
- *ViewModels*: Shape data for specific views.

### Execution Flow

1. User interacts with a Razor page.
2. *Page Controller* receives the request.
3. Controller calls the appropriate *Service* through its *Interface*.
4. Service interacts with the *Database (DbContext)* and returns data.
5. Controller passes *ViewModel* to the *View* for rendering.

### Flow of Execution:

1. *Page Controller* handles an HTTP request.
2. The *Page Controller* prepares and passes the data via a *View Model*.
3. The *View Model* holds the necessary data and structure for the view.
4. The *View* renders the data and presents it to the user.

This design ensures that each part of the application has a clear responsibility, improving maintainability, flexibility, and testability.


## Collaboration Details

Our team collaborated extensively on designing the data models and populating the database. Following this, Himani and I divided responsibilities, each taking charge of three entities for controllers, services, interfaces, page controllers, and views. This structured approach enabled us to efficiently develop and integrate various components while ensuring consistency and maintainability across the project.

## Contact

For any queries or suggestions, reach out to [Himani Bansal](https://github.com/Himani1609) or [Dhruv Shah](https://github.com/DhruvShah28)