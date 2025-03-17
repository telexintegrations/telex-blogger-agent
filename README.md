# Telex Blogger Agent V1.1

## Overview
The **Telex Blogger** Agent is an AI-driven assistant that leverages the **Gemini API** to generate high-quality blog posts. Designed for seamless integration with Telex, it enables users to automate their **blog content** creation effortlessly.

## What's New in V1.1
✅ **Personalized Content Generation**: Users can now apply **custom personalization settings** to enhance blog content.
✅ **Company Website for CTA**: The AI now automatically includes a **call-to-action (CTA)** linking to the user’s company website in the blog post conclusion.
✅ **Enhanced API Flexibility**: The agent dynamically retrieves settings based on labels, making it more reusable and adaptable.
✅ **Refactored Codebase**: Improved structure for better scalability and maintainability.

## 1. Setting Up the Blogger Agent in Telex
To begin using the **Blogger Agent** in Telex, follow these steps:

### Step 1: Activate the Integration
1. **Log in** to your **Telex Organization**.
2. Navigate to the **Integrations** section.
3. Use the deployed URL of the integration JSON file below to **add the integration** to your organization:
   
   👉 **Integration URL:**
   ```
   https://telex-blogger-agent-qdp4.onrender.com/api/v1/telex-integration
   ```
4. Locate **Telex Blogger Agent** in the integration list and **Activate** it.
5. **Enter your channel’s webhook URL** and **input the personalization options** provided in the settings field. These options help tailor the AI-generated blog posts according to your preferences.
6. Click **Save** to complete the setup.

### Step 2: Generating Blog Content in Telex
Once the integration is active, you can use the **Blogger Agent** inside any **Telex channel**.

#### Triggering the Agent
You can generate a blog post by sending a message in Telex with an optional context provided:

👉 **Example Usage:**
```
"Generate a blog post on The Future of AI in Blogging"
```

The agent will **automatically send a structured blog post** to the webhook URL you provided during integration setup in the format of title, introduction, body, and conclusion.

## 2. API Endpoints For Testing
The Blogger Agent provides two API endpoints:

---

### 1️⃣ Generate Blog Post (POST)
Generates a blog post based on user input with **personalization settings**.

👉 **Endpoint URL:**
```
POST https://telex-blogger-agent-qdp4.onrender.com/api/v1/blogger-agent/generate-blog
```

👉 **Request Body (JSON):**
```json
{
  "message": "Generate a blog post on The Impact of AI on Content Writing",
  "settings": [
    {
      "label": "webhook_url",
      "type": "text",
      "required": true,
      "default": "https://your-webhook-url"
    },
    {
      "label": "company_website",
      "type": "text",
      "required": false,
      "default": "https://your-company-website.com"
    }
  ]
}
```

👉 **Response:**
The response is a plain string containing the message you sent. The blog request is processed in the background.

```
Generate a blog post on The Impact of AI on Content Writing
```

Once the blog is generated, it will be **automatically sent** to the channel with the webhook URL you provided. The AI will also **append a call-to-action** at the end linking to your company website (if provided).

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

## 6. Testing the Integration
To test if the **Blogger Agent** is working as expected:

1️⃣ **Send a Blog Generation Request**
```sh
curl -X POST "https://telex-blogger-agent-qdp4.onrender.com/api/v1/blogger-agent/generate-blog" \
     -H "Content-Type: application/json" \
     -d '{ "message": "Generate a blog post on How AI is Changing Blogging" }'
```

2️⃣ **Check Response**
Ensure the output contains the generated blog content.

3️⃣ **Verify in Telex**
Check your Telex channel to see if the generated blog post was sent to the **channel webhook URL** you provided.

🚀 **Upgrade to V1.1 and take your blog automation to the next level!** 🚀

