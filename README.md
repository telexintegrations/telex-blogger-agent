
# Telex Blogger Agent MVP Version

## Overview
The **Telex Blogger Agent** is an AI-powered assistant designed to streamline **blog content creation** within Telex. Leveraging the **Gemini API**, this agent allows users to interactively **chat** with AI to generate blog posts, making content creation more dynamic and intuitive.

## What's New in the MVP Version
✅ **Conversational AI for Blogging**: Users can now chat with the AI to refine blog topics, adjust tone, and request modifications before finalizing content.
✅ **Dynamic Webhook Retrieval**: The AI now fetches the webhook URL dynamically based on **channel ID**, making integration seamless.
✅ **Improved User Interaction**: Instead of a single request, users can refine blog content through an interactive chat session.
✅ **Refactored Codebase**: Optimized structure for enhanced maintainability and scalability.

## 1. Setting Up the Blogger Agent in Telex
To start using the **Blogger Agent**, follow these steps:

### Step 1: Activate the Integration
1. **Log in** to your **Telex Organization**.
2. Navigate to the **Integrations** section.
3. Use the deployed URL below to **add the integration** to your organization:
   
   👉 **Integration URL:**
   ```
   https://telex-blogger-agent-qdp4.onrender.com/api/v1/telex-integration
   ```
4. Locate **Telex Blogger Agent** in the integration list and **Activate** it.
5. **Enter personalization settings** (e.g., preferred tone, format, company name) to customize blog generation.
6. Click **Save** to complete the setup.

### Step 2: Chat-Based Blog Generation in Telex
Once activated, the **Blogger Agent** can be triggered via chat within any **Telex channel**.

#### Interactive Chat Mode
The AI engages in a **conversation** to refine blog content before finalizing it.

👉 **Example Usage:**
```
User: Generate a blog post on The Future of AI in Blogging.
AI: Would you like a professional or casual tone?
User: Casual.
AI: Should I include a CTA to your website?
User: Yes.
AI: Here’s a draft introduction... Does this align with your vision?
```

This chat-based interactivity ensures that users get precisely the content they need before submission.

## 2. API Endpoints For Testing

### 1️⃣ Generate Blog Post (POST)
Generates a blog post based on **user input** with personalization settings.

👉 **Endpoint URL:**
```
POST https://telex-blogger-agent-qdp4.onrender.com/api/v1/blogger-agent/generate-blog

👉 **Request Body (JSON):**
```json
{
  "message": "Generate a blog post on The Impact of AI on Content Writing",
  "channel_id": "0195bebf-a5e2-7b26-8eb5-a585a61d8188"
  "settings": [
    {
      "label": "company_name",
      "type": "text",
      "description": "Provide the company name.",
      "default": "Tech Innovators",
      "required": true
    },
    {
      "label": "company_overview",
      "type": "text",
      "description": "Provide a brief overview of the company.",
      "default": "Tech Innovators specializes in AI-powered content solutions.",
      "required": true
    },
    {
      "label": "company_website",
      "type": "text",
      "description": "Provide the company’s website URL to be included in the blog post conclusion.",
      "default": "https://www.technology-innovators.com",
      "required": false
    },
    {
      "label": "tone",
      "type": "dropdown",
      "description": "Choose the tone for the blog content.",
      "options": [ "Professional", "Casual", "Persuasive", "Informative" ],
      "default": "Casual",
      "required": true
    },
    {
      "label": "blog_length",
      "type": "dropdown",
      "description": "Select the preferred blog length.",
      "options": [ "short", "medium", "long" ],
      "default": "medium",
      "required": true
    },
    {
      "label": "format",
      "type": "dropdown",
      "description": "Choose the format for the blog output.",
      "options": [ "Full Article", "Outline", "Summary" ],
      "default": "Full Article",
      "required": false
    }
  ]
}
```

👉 **Key Changes:**
- **`channel_id`**: Telex now sends a channel ID, which the agent uses to dynamically retrieve the webhook URL.
- The AI engages users in a **chat-style interaction** before sending the final blog post.

👉 **Response:**
Once finalized, the blog post is sent **automatically** to the webhook URL associated with the provided **channel ID**.

---

### 2️⃣ Telex Integration Configuration (GET)
Retrieves the integration.json.

👉 **Endpoint URL:**
```
GET https://telex-blogger-agent-qdp4.onrender.com/api/v1/telex-integration
```

👉 **Response:**
Retrieves the integration.json configuration for the agent.

```
{"data":{"date":{"created_at":"2025-03-10","updated_at":"2025-03-10"},"descriptions":{"app_description":"AI-powered Blogging Assistant for Telex"},"integration_category":"AI & Machine Learning","integration_type":"modifier"}}
```

## 3. Project Structure
```
/Telex-Blogger-Agent
│── TelexBloggerAgent/                 
│   ├── Controllers/                    
│   │   ├── BloggerAgentController.cs   
│   │   ├── TelexIntegrationController.cs
│   │   │
│   ├── Dtos/
│   │   ├── GenerateBlogDto.cs         
│   │   ├── TelexSettingsDto.cs         
│   │   │
│   ├── Helpers/                        
│   │   ├── GeminiSettings.cs          
│   │   ├── TelexSettings.cs           
│   │   │
│   ├── IServices/
│   │   ├── IBlogAgentService.cs        
│   │   ├── ITelexIntegrationService.cs
│   │   │
│   ├── Services/                        
│   │   ├── BlogAgentService.cs        
│   │   ├── TelexIntegrationService.cs  
│   │   │
│   ├── appsettings.json
│   ├── Integration.json                
│   ├── Program.cs                     
│                               
│── README.md                           
│── .gitignore                          
│── Dockerfile                          
│── TelexBloggerAgent.sln              
```

## 4. Installing the Project Locally
To set up the project locally, follow these steps:

### Step 1: Clone the Repository
```sh
git clone https://github.com/telexintegrations/telex-blogger-agent.git
```

### Step 2: Install Dependencies
```sh
cd telex-blogger-agent
dotnet restore
```

### Step 3: Run the Application
```sh
dotnet run
```

## 5. Deployment
This application is **containerized using Docker** and deployed on **Render**.

### Current Deployment URL
👉 **Base URL:**  
```
https://telex-blogger-agent-qdp4.onrender.com
```

### Running with Docker
To deploy using **Docker**, use the following commands:

#### Build Docker Image
```sh
docker build -t telex-blogger-agent .
```

#### Run the Container
```sh
docker run -p 8080:8080 telex-blogger-agent
```

The app will be available at:
```
http://localhost:8080
```


## 6. Testing the Chat-Based Integration

1️⃣ **Send a Blog Generation Request**
```sh
curl -X POST "https://telex-blogger-agent-qdp4.onrender.com/api/v1/blogger-agent/generate-blog" \
     -H "Content-Type: application/json" \
     -d '{ "message": "Generate a blog post on How AI is Changing Blogging", "channel_id": "123456789" }'
```

2️⃣ **Chat with AI**
Wait for AI’s response and refine the blog in the chat session.

3️⃣ **Verify in Telex**
The finalized blog post will be sent to the **webhook URL** associated with the **channel ID**.

🚀 **Upgrade to the MVP version and experience AI-powered blog interactivity!** 🚀
