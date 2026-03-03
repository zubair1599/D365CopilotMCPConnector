# D365Commerce MCP Server

A **Model Context Protocol (MCP) server** built with ASP.NET Core that bridges AI assistants (GitHub Copilot, Claude, and other MCP-compatible clients) with **Microsoft Dynamics 365 Commerce** — enabling natural language access to products, orders, customers, and inventory data etc.

---

##  What is MCP?

The [Model Context Protocol](https://modelcontextprotocol.io) is an open standard that allows AI assistants to securely connect to external data sources and tools. This server implements MCP over **HTTP/SSE (Server-Sent Events)**, making D365 Commerce data accessible to any compatible AI client.

---

##  Features
Just to start with, implemented few samples and the list can grow as per need. 
| Tool | Description |
|---|---|
|  **Product Search & Catalog Browsing** | Search products, browse categories, retrieve product details and attributes |
|  **Order Management** | Query sales orders, check order status, retrieve order history |
|  **Customer Data** | Look up customer profiles, purchase history, and account information |
|  **Inventory & Pricing** | Check stock levels, pricing, discounts, and availability across channels |

---

##  Architecture

The project follows **Clean Architecture** principles:

```
D365CopilotMCPConnector/
├── Application/        # Use cases, MCP tool handlers, DTOs
├── Domain/             # Core entities and business logic
├── Infrastructure/     # D365 Commerce API clients, HTTP services
├── Configuration/      # App settings, DI registrations
├── Host/               # ASP.NET Core host configuration
├── Properties/         # Launch settings
├── Program.cs          # Entry point
└── appsettings.json    # Configuration
```

---

##  Authentication

This connector authenticates with D365 Commerce via **OAuth 2.0 / Azure Active Directory (Azure AD)**. A registered Azure AD app is required with appropriate permissions to the D365 Commerce APIs.

---

##  Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later
- A running **Microsoft Dynamics 365 Commerce** environment
- An **Azure AD App Registration** with D365 API permissions
- An MCP-compatible client (GitHub Copilot, Claude Desktop, etc.)

---

##  Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/zubair1599/D365CopilotMCPConnector.git
cd D365CopilotMCPConnector
```

### 2. Configure settings

Update `appsettings.json` (or use environment variables / user secrets) with your D365 and Azure AD credentials:

```json
{
  "D365Commerce": {
    "BaseUrl": "https://your-d365-environment.dynamics.com",
    "Environment": "your-environment-name"
  },
  "AzureAd": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "Scopes": "https://your-d365-environment.dynamics.com/.default"
  }
}
```

> ⚠️ **Never commit secrets to source control.** Use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or Azure Key Vault in production.

### 3. Run the server

```bash
dotnet run
```

The MCP server will start and expose an SSE endpoint, typically at:

```
http://localhost:5000/sse
```

---

##  Connecting an MCP Client

### GitHub Copilot (VS Code)

Add the following to your `.vscode/mcp.json` or user settings:

```json
{
  "servers": {
    "d365-commerce": {
      "type": "sse",
      "url": "http://localhost:5000/sse"
    }
  }
}
```

### Claude Desktop

Add to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "d365-commerce": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/D365CopilotMCPConnector"],
      "env": {}
    }
  }
}
```

Or if running as a hosted server:

```json
{
  "mcpServers": {
    "d365-commerce": {
      "type": "sse",
      "url": "http://localhost:5000/sse"
    }
  }
}
```

---

##  Example Prompts

Once connected, you can ask your AI assistant things like:

- *"Search for running shoes under $100 in the catalog"*
- *"What's the current stock level for product SKU ABC-123?"*
- *"Show me all open orders for customer john.doe@contoso.com"*
- *"What are the available discounts on category Electronics?"*
- *"Get the order history for order number SO-00042"*

---

##  Development

### Build

```bash
dotnet build
```

### Run in Development mode

```bash
dotnet run --environment Development
```

Development settings (`appsettings.Development.json`) can override production config for local testing.

---

##  Contributing

Contributions are welcome! Please open an issue first to discuss what you'd like to change, then submit a pull request.

1. Fork the repo
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add some feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

##  Author

**Zubair** — [@zubair1599](https://github.com/zubair1599)

---

*Built with using [.NET MCP SDK](https://github.com/modelcontextprotocol/csharp-sdk) and the D365 Commerce APIs.*
