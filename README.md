# Telex Blogger Agent (v1)

## Overview
The **Telex Blogger Agent** is an AI-powered blogging assistant that generates high-quality blog posts using the **Gemini API**. It integrates seamlessly with **Telex**, allowing users to automate blog content creation.

## 1. Setting Up the Blogger Agent in Telex
To begin using the **Blogger Agent** in Telex, follow these steps:

### Step 1: Activate the Integration
1. **Log in** to your **Telex Organization**.
2. Navigate to the **Integrations** section.
3. Use the deployed URL of the integration JSON file below to **add the integration** to your organization:
   
   👉 **Integration URL:**
   ```
   https://telex-blogger-agent-qdp4.onrender.com/api/v1/telex-blogger-agent/integration.json
   ```
4. Locate **Telex Blogger Agent** in the integration list and **Activate** it.
5. **Enter your channel’s webhook URL** in the settings field to allow the agent to send generated blog posts to your desired channel.
6. Click **Save** to complete the setup.

### Step 2: Generating Blog Content in Telex
Once the integration is active, you can use the **Blogger Agent** inside any **Telex chat/channel**.

#### Triggering the Agent
You can generate a blog post by sending a command in Telex:

👉 **Command Format:**
```
/blogger-agent generate <topic>
```

👉 **Example Usage:**
```
/blogger-agent generate "Generate a blog on The Future of AI in Blogging"
```

The agent will **automatically send the generated blog post** to the webhook URL you provided during integration.

## 2. API Endpoints
The Blogger Agent provides two API endpoints:

### 1️⃣ Generate Blog Post (POST)
Generates a blog post based on user input.

👉 **Endpoint URL:**
```
POST https://telex-blogger-agent-qdp4.onrender.com/api/v1/telex-blogger-agent/generate-blog
```

👉 **Request Body (JSON):**
```json
{
  "message": "The Impact of AI on Content Writing"
}
```

👉 **Response:**
```json
{
  "message": "AI has revolutionized content writing by enhancing efficiency, creativity, and personalization..."
}
```

Once the blog is generated, it will be **automatically sent** to the provided webhook URL.

### 2️⃣ Telex Integration Configuration (GET)
Retrieves the integration settings.

👉 **Endpoint URL:**
```
GET https://telex-blogger-agent-qdp4.onrender.com/api/v1/telex-blogger-agent/integration
```

👉 **Response:**
```json
{
  "name": "Telex Blogger Agent",
  "version": "v1",
  "status": "active",
  "settings": [
    {
      "label": "Webhook URL",
      "type": "string",
      "required": true
    }
  ]
}
```

## 3. Project Structure
```
/telex-blogger-agent
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
│   ├── integration.json                
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
The app will be available at:
```
http://localhost:5000
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
docker run -p 8080:80 telex-blogger-agent
```

## 6. Testing the Integration
To test if the **Blogger Agent** is working as expected:

1️⃣ **Send a Blog Generation Request**
```sh
curl -X POST "https://telex-blogger-agent-qdp4.onrender.com/api/v1/telex-blogger-agent/generate-blog" \
     -H "Content-Type: application/json" \
     -d '{ "message": "How AI is Changing Blogging" }'
```

2️⃣ **Check Response**
Ensure the output contains the generated blog content.

3️⃣ **Verify in Telex**
Check your Telex channel to see if the generated blog post was sent to the **webhook URL** you provided.

## Conclusion
The **Telex Blogger Agent** simplifies blog writing by leveraging **AI-powered content generation**. With seamless integration into **Telex**, users can generate high-quality blog posts effortlessly.

🚀 **Start using it today and automate your blogging process!** 🚀

### Main Updates in This Version:
✅ **Webhook URL setting**: Users must provide a webhook URL to receive generated blog posts.  
✅ **Simplified commands**: No extra settings required in Telex commands.  
✅ **Automated posting**: Blog posts are sent directly to the configured Telex webhook.

