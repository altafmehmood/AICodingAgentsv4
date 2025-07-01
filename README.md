# Breach Management System

A comprehensive security risk analysis application built with .NET 9.0 and Angular 20 that helps users identify data breaches associated with their email addresses and provides AI-powered risk assessments.

## Features

### Backend API (.NET 9.0)
- **Breach Search**: Search for data breaches using the Have I Been Pwned API
- **Risk Analysis**: AI-powered risk assessment using Claude AI
- **PDF Reports**: Generate professional PDF reports of security analysis
- **RESTful API**: Clean, well-documented endpoints
- **CQRS Pattern**: MediatR-based command/query separation
- **Global Exception Handling**: Structured error responses
- **Comprehensive Logging**: Detailed application logging

### Frontend (Angular 20)
- **Modern UI**: Built with Angular 20 and best practices
- **Responsive Design**: Works on desktop and mobile devices
- **Real-time Search**: Interactive breach search functionality

## API Endpoints

### Breach Controller (`/api/breach`)

- `GET /api/breach` - Get all available breaches
- `GET /api/breach/email/{email}` - Get breaches for a specific email address
- `GET /api/breach/analyze/{email}` - Analyze risk for a specific email address
- `GET /api/breach/report/{email}` - Generate and download PDF risk analysis report

### Example Usage

```bash
# Get breaches for an email
curl -X GET "https://localhost:5001/api/breach/email/test@example.com"

# Analyze risk
curl -X GET "https://localhost:5001/api/breach/analyze/test@example.com"

# Generate PDF report
curl -X GET "https://localhost:5001/api/breach/report/test@example.com" \
     -H "Accept: application/pdf" \
     --output "risk_report.pdf"
```

## Technology Stack

### Backend
- **.NET 9.0** - Core framework
- **ASP.NET Core** - Web API
- **MediatR** - CQRS pattern implementation
- **Flurl.Http** - HTTP client for external APIs
- **PuppeteerSharp** - PDF generation
- **Handlebars.Net** - HTML templating
- **xUnit** - Unit testing framework
- **NSubstitute** - Mocking framework

### Frontend
- **Angular 20** - Frontend framework
- **TypeScript** - Type-safe JavaScript
- **RxJS** - Reactive programming

### External APIs
- **Have I Been Pwned API** - Breach data source
- **Claude AI API** - Risk analysis and recommendations

## Setup and Configuration

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ and npm
- Have I Been Pwned API key
- Claude AI API key

### Configuration

Update `appsettings.json` with your API keys:

```json
{
  "HaveIBeenPwnedOptions": {
    "BaseUrl": "https://haveibeenpwned.com/api/v3",
    "ApiKey": "your-hibp-api-key",
    "UserAgent": "BreachManagementSystem/1.0"
  },
  "ClaudeOptions": {
    "BaseUrl": "https://api.anthropic.com/v1/messages",
    "ApiKey": "your-claude-api-key",
    "Model": "claude-3-5-sonnet-20240620",
    "MaxTokens": 1000
  }
}
```

### Running the Application

#### Backend API
```bash
cd Breach.Api
dotnet restore
dotnet run
```

#### Frontend
```bash
cd breach-viewer
npm install
npm start
```

#### Running Tests
```bash
cd Breach.Api.Tests
dotnet test
```

## Architecture

The application follows Clean Architecture principles with:

- **Controllers**: Handle HTTP requests and responses
- **Features**: CQRS queries and handlers organized by feature
- **Services**: External API integrations and business logic
- **Models**: Data transfer objects and domain models
- **Middleware**: Cross-cutting concerns like exception handling

### PDF Report Generation

The PDF export feature uses:
1. **Handlebars.Net** for HTML templating
2. **PuppeteerSharp** for HTML-to-PDF conversion
3. **Professional styling** with responsive design
4. **Comprehensive breach details** with risk analysis

### Report Features
- Executive summary with risk level
- Detailed breach information
- AI-generated security recommendations
- Professional formatting with charts and badges
- Print-optimized layout

## Security Features

- **API Key Authentication** for external services
- **CORS Configuration** for frontend integration
- **Input Validation** and sanitization
- **Structured Error Handling** without exposing sensitive data
- **Comprehensive Logging** for security monitoring

## Development Guidelines

### Git Commit Format
- `feat:` - New feature
- `fix:` - Bug fix
- `refactor:` - Code refactoring
- `test:` - Add tests
- `chore:` - Update dependencies
- `docs:` - Update documentation

### Code Standards
- Follow .NET coding conventions
- Implement comprehensive unit tests
- Use dependency injection
- Apply SOLID principles
- Maintain high code coverage

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## Support

For support and questions, please open an issue in the GitHub repository. 