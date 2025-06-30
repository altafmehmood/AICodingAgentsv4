# Breach Management System

A comprehensive data breach monitoring and analysis platform built with .NET 9.0 and Angular 20, featuring AI-powered risk assessment and real-time breach intelligence.

## 🚀 Features

### Core Functionality
- **Real-time Breach Data**: Integration with Have I Been Pwned API for up-to-date breach intelligence
- **AI Risk Analysis**: Claude AI-powered risk assessment and recommendations
- **Advanced Filtering**: Date range filtering and search capabilities
- **Health Monitoring**: Comprehensive system health checks and monitoring
- **Responsive Design**: Mobile-first UI with Angular Material

### Security & Performance
- **Rate Limiting**: API protection and abuse prevention
- **Caching**: Redis-based caching for optimal performance
- **Secure Configuration**: Azure Key Vault integration for secrets management
- **CORS Protection**: Secure cross-origin resource sharing
- **Error Handling**: Comprehensive error management and user feedback

## 🏗️ Architecture

### Backend (.NET 9.0)
```
├── BreachManagementSystem.Api/          # Web API layer
├── BreachManagementSystem.Core/         # Domain models and interfaces
├── BreachManagementSystem.Infrastructure/ # External integrations and services
└── BreachManagementSystem.Tests/        # Unit tests
```

**Technology Stack:**
- ASP.NET Core 9.0 Web API
- Clean Architecture with CQRS (MediatR)
- Redis caching with StackExchange.Redis
- Azure Key Vault for secrets management
- Flurl.Http for external API communication
- xUnit for testing

### Frontend (Angular 20)
```
├── src/app/
│   ├── core/                    # Singleton services and models
│   ├── features/               # Feature-based modules
│   │   ├── breach-management/  # Breach data components
│   │   └── health/            # Health monitoring components
│   └── shared/                # Reusable components
```

**Technology Stack:**
- Angular 20 with standalone components
- Angular Material for UI components
- RxJS for reactive programming
- TypeScript 5.8+
- SCSS for styling

## 🛠️ Setup Instructions

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ with npm
- Redis server
- Azure Key Vault (for production)

### Backend Setup

1. **Navigate to backend directory:**
   ```bash
   cd backend
   ```

2. **Restore packages:**
   ```bash
   dotnet restore
   ```

3. **Configure secrets (Development):**
   The application includes fallback values for development. For production, configure Azure Key Vault with:
   - `redis-connection-string`
   - `hibp-api-key` (Have I Been Pwned API key)
   - `anthropic-claude-api-key` (Claude AI API key)

4. **Run the API:**
   ```bash
   dotnet run --project src/BreachManagementSystem.Api
   ```

   The API will be available at `https://localhost:7076`

### Frontend Setup

1. **Navigate to frontend directory:**
   ```bash
   cd frontend
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Start development server:**
   ```bash
   npm start
   ```

   The application will be available at `http://localhost:4200`

## 📋 API Endpoints

### Breach Management
- `GET /api/breach` - Get all breaches with optional filtering
- `GET /api/breach/{breachName}` - Get specific breach details
- `GET /api/breach/{breachName}/risk-analysis` - Get AI risk analysis for breach
- `POST /api/breach/risk-analysis` - Get risk analysis for multiple breaches

### Health Monitoring
- `GET /api/health` - Get detailed system health status
- `GET /health` - Simple health check endpoint

## 🔧 Configuration

### Backend Configuration (appsettings.json)
```json
{
  "AzureKeyVault": {
    "VaultUrl": "https://akhs-keyvault.vault.azure.net/"
  }
}
```

### Frontend Configuration (environment.ts)
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7076'
};
```

## 🧪 Testing

### Backend Tests
```bash
cd backend
dotnet test
```

### Frontend Tests
```bash
cd frontend
npm test
```

## 🚀 Deployment

### Backend Deployment
- Deploy to Azure App Service
- Configure Azure Key Vault secrets
- Set up Redis cache instance
- Configure CORS for production domain

### Frontend Deployment
- Build for production: `npm run build`
- Deploy to CDN or static hosting
- Update environment configuration for production API URL

## 📊 Monitoring & Observability

### Health Checks
- Redis cache connectivity
- Have I Been Pwned API availability
- System resource monitoring

### Logging
- Structured logging with Microsoft.Extensions.Logging
- Error tracking and performance monitoring
- Request/response logging for debugging

## 🔒 Security Features

### API Security
- Rate limiting (100 requests/minute)
- CORS protection
- Input validation and sanitization
- Secure error responses

### Data Protection
- Azure Key Vault for secrets management
- No sensitive data in logs
- HTTPS enforcement
- Secure API key handling

## 🎯 Core Components

### Backend Services
- **HaveIBeenPwnedService**: External API integration with caching
- **ClaudeAiRiskAnalysisService**: AI-powered risk assessment
- **RedisCacheService**: High-performance caching layer
- **HealthCheckService**: System monitoring and diagnostics

### Frontend Components
- **BreachListComponent**: Responsive data table with filtering
- **BreachDetailComponent**: Detailed breach information with AI analysis
- **HealthDashboardComponent**: Real-time system health monitoring

## 📈 Performance Optimizations

### Backend
- Redis caching (1-hour TTL for breach data, 24-hour TTL for AI analysis)
- Async/await patterns throughout
- Connection pooling for external APIs
- Health check optimizations

### Frontend
- OnPush change detection strategy
- Lazy loading for route-based code splitting
- HTTP interceptors for loading states
- Responsive design with mobile optimization

## 🔄 Status

### ✅ Completed
- Clean Architecture backend setup
- Azure Key Vault integration
- Have I Been Pwned API integration
- Redis caching infrastructure
- Core domain models and DTOs
- Breach data endpoints with filtering
- Claude AI risk analysis service
- Rate limiting and security middleware
- Health check endpoints
- Angular 20 standalone components
- Responsive UI with Angular Material
- API services and HTTP interceptors
- Error handling and user feedback

### 🚧 Pending
- PDF generation functionality
- Comprehensive unit tests
- Deployment pipeline configuration

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📞 Support

For support and questions, please open an issue in the GitHub repository.