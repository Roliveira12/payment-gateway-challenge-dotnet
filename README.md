# Project Documentation

## Project Structure
The project was developed following the **Clean Architecture** structure. This approach ensures a clear separation of concerns, making the codebase more maintainable and scalable.

## Payment Authorization Endpoint
A new route was created:

**Endpoint:** `POST /payments/authorize`

**Payload:**
```json
{
  "cardNumber": "1234567891011",
  "expiryDate": "12/2025",
  "currency": "USD",
  "amount": 101,
  "cvv": 123
}
```

This endpoint is responsible for handling payment authorization requests.

## IBankApi Implementation
The endpoint interacts with an **IBankApi** interface, implemented using the **Refit** library.

### Why Use Refit?
Refit simplifies API communication by providing an easy-to-use, strongly-typed REST API client. It automatically generates HTTP requests from interfaces, reducing boilerplate code and improving maintainability.

### Infrastructure Layer
To maintain proper separation of concerns, the repository was moved to the **Infrastructure Layer**. The **Refit** configuration for API communication was also placed in this layer.

Additionally, some components were reorganized:
- **Authorization Enums** were moved to the **Domain Layer** to ensure better separation of concerns.
- A **global exception-handling middleware** was created to standardize error responses and improve robustness. This middleware was introduced to keep the code **clean** by reducing the need for excessive `try-catch` blocks spread across the application.
- The properties **ExpiryMonth** and **ExpireYear** were merged into a single field to improve the API design and enhance integration with clients.

## Input Validation
Input validation is handled using **FluentValidation**.

### Why Use FluentValidation?
- Provides a clean and declarative way to validate models.
- Supports complex validation rules with ease.
- Improves testability and maintainability by decoupling validation logic from business logic.

## Modularity and Low Coupling
The project was designed to be modular, leveraging **use cases** to decouple business logic from infrastructure and presentation layers. This approach enhances portability and maintainability.

## Testing Strategy
Unit tests were implemented using:
- **xUnit** as the main testing framework.
- **NSubstitute** for mocking dependencies.
- **Theory and InlineData** for parameterized testing, improving test coverage and efficiency.

This testing approach ensures that the application remains reliable and maintainable as it evolves.

---
