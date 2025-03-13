# AI Blogger Agent - V1.0

## Overview
The **AI Blogger Agent (V1.0)** is an AI-powered integration for **Telex** that allows users to generate structured blog drafts via messaging. Users can request AI-generated blog posts by sending a message to Telex, which forwards the request to an AI API that generates and returns a structured blog post.

## Features
- Users can generate AI-written blog drafts with **title, introduction, body, and conclusion**.
- Seamlessly integrates with **Telex** to handle user requests and responses.
- Provides a **REST API** for blog generation.
- Uses **ILogger** for logging.

## Project Structure
```
telex-blogger-agent/
?-- TelexBloggerAgent.sln
?-- Dockerfile
?-- .gitignore
?-- README.md
?-- TelexBloggerAgent/
?   ?-- Controllers/
?   ?   ??? BloggerAgentController.cs
?   ?   ??? TelexIntegrationController.cs
?   ?-- Dtos/
?   ?   ??? GenerateBlogDto.cs
?   ?   ??? Setting.cs
?   ?-- Helpers/
?   ?   ??? GeminiSettings.cs
?   ?   ??? TelexSettings.cs
?   ?-- IServices/
?   ?   ??? IBlogAgentService.cs
?   ?   ??? ITelexIntegrationService.cs
?   ?-- Models/ (Empty for now)
?   ?-- Services/
?   ?   ??? BlogAgentService.cs
?   ?   ??? TelexIntegrationService.cs
?   ?-- Program.cs
?   ?-- integration.json
```

## Installation & Setup
### **1. Clone the Repository**
```sh
  git clone https://github.com/your-repo/blogger-agent.git
  cd blogger-agent
```

### **2. Run the Application**
#### Without Docker:
```sh
  dotnet run --project TelexBloggerAgent
```

#### With Docker (Optional):
```sh
  docker build -t telex-blogger-agent .
  docker run -p 5000:5000 telex-blogger-agent
```

## API Endpoints
### **1. Generate Blog Post**
**Endpoint:** `POST /api/v1/generate-blog`

**Request Body:**
```json
{
  "message": "string",
  "settings": [
    {
      "label": "string",
      "type": "string",
      "required": true,
      "default": "string"
    }
  ]
}
```

**Response:**
```json
"Generated blog post content as a string message"
```

### **2. Telex Integration Check**
**Endpoint:** `GET /api/v1/telex-integration`

**Response:**
```json
"Telex integration is active."
```

## Deployment
The AI Blogger Agent (V1.0) is currently deployed on **Render**.

**Base URL:**
```
https://telex-blogger-agent-qdp4.onrender.com
```

## Testing
- No automated tests implemented at this stage.

## Contribution Guidelines
1. Fork the repository and create a new branch.
2. Commit your changes with clear messages.
3. Open a pull request for review.

## Roadmap
- ? **V1.0**: Basic blog draft generation.
- ? **Future Versions**: Enhancements such as user customization, SEO optimization, and style variations.

## License
MIT License

