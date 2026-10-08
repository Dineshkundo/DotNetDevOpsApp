# DotNetDevOpsApp — Beginner DevOps CI/CD Setup Guide

A beginner-friendly, step-by-step guide for building and deploying a standalone **ASP.NET Core .NET 8 application** on an AWS EC2 Linux server using **GitHub + Jenkins + SSH + systemd**.

---

## 1. What We Are Building

This project demonstrates a real-world DevOps CI/CD pipeline:

```text
Developer
   │
   │ git push
   ▼
GitHub
   │
   │ Jenkins checks repository
   ▼
Jenkins EC2 #1
   │
   ├── Restore
   ├── Build
   ├── Test
   ├── Publish
   └── Deploy
          │
          │ SSH / SCP
          ▼
Application EC2 #2
   │
   ├── /opt/dotnetapp
   ├── systemd
   └── ASP.NET Core application
```

The application is published as a **self-contained Linux application**.

This means the application server does **not need the .NET runtime or SDK installed**.

---

# 2. Technologies Used

## Application

- C#
- ASP.NET Core
- .NET 8
- REST API
- xUnit

## DevOps

- Git
- GitHub
- Jenkins
- AWS EC2
- Linux / Ubuntu
- SSH
- SCP
- systemd
- Bash
- CI/CD

---

# 3. AWS Architecture

We use two separate EC2 instances.

## EC2 #1 — Jenkins Server

Purpose:

```text
Jenkins
.NET SDK
Git
CI/CD pipeline
```

Example architecture:

```text
EC2 #1
Jenkins Server
      │
      │ SSH/SCP
      ▼
EC2 #2
Application Server
```

## EC2 #2 — Application Server

Purpose:

```text
ASP.NET Core application
systemd service
application files
```

The application runs internally on:

```text
127.0.0.1:5000
```

---

# 4. Why Two EC2 Instances?

Keeping Jenkins and the application server separate is closer to a real DevOps environment.

```text
              AWS
               │
       ┌───────┴────────┐
       │                │
       ▼                ▼
   EC2 #1            EC2 #2
   Jenkins           Application
       │                │
       │ SSH/SCP        │
       └───────────────►│
```

Jenkins builds and deploys the application.

The application server only runs the application.

---

# 5. Local Development Environment

The project was developed inside GitHub Codespaces.

Project directory:

```bash
/workspaces/DotNetDevOpsApp
```

Check the current directory:

```bash
pwd
```

Expected:

```text
/workspaces/DotNetDevOpsApp
```

---

# 6. Install / Verify .NET 8

Check installed SDKs:

```bash
dotnet --list-sdks
```

The project uses .NET 8.

Example:

```text
8.0.425
10.0.401
```

The project uses `global.json` to select .NET 8.

## global.json

```json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestFeature"
  }
}
```

Check the selected SDK:

```bash
dotnet --version
```

Expected:

```text
8.0.425
```

---

# 7. Project Structure

The project structure is:

```text
DotNetDevOpsApp/
│
├── .git/
├── .gitignore
├── global.json
├── Jenkinsfile
├── README.md
│
├── DotNetDevOpsApp/
│   ├── Controllers/
│   │   └── ProductsController.cs
│   │
│   ├── Models/
│   │   └── Product.cs
│   │
│   ├── Program.cs
│   └── DotNetDevOpsApp.csproj
│
├── DotNetDevOpsApp.Tests/
│   ├── ProductsControllerTests.cs
│   └── DotNetDevOpsApp.Tests.csproj
│
└── deploy/
    └── dotnetapp.service
```

---

# 8. Create the ASP.NET Core Application

Create the Web API:

```bash
dotnet new webapi \
  -n DotNetDevOpsApp \
  --framework net8.0 \
  --use-controllers
```

Create the xUnit test project:

```bash
dotnet new xunit \
  -n DotNetDevOpsApp.Tests \
  --framework net8.0
```

Add a reference from the test project to the application:

```bash
dotnet add \
  DotNetDevOpsApp.Tests/DotNetDevOpsApp.Tests.csproj \
  reference \
  DotNetDevOpsApp/DotNetDevOpsApp.csproj
```

---

# 9. Application Endpoints

The application provides:

## Health Check

```text
GET /health
```

Example:

```bash
curl http://localhost:5000/health
```

Response:

```json
{
  "status": "Healthy",
  "application": "DotNetDevOpsApp",
  "timestamp": "..."
}
```

## Get Products

```text
GET /api/products
```

Example:

```bash
curl http://localhost:5000/api/products
```

## Get Product

```text
GET /api/products/{id}
```

Example:

```bash
curl http://localhost:5000/api/products/1
```

## Create Product

```text
POST /api/products
```

## Update Product

```text
PUT /api/products/{id}
```

## Delete Product

```text
DELETE /api/products/{id}
```

---

# 10. Test the Application Locally

Build:

```bash
dotnet build \
  ./DotNetDevOpsApp/DotNetDevOpsApp.csproj \
  --configuration Release
```

Expected:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

Run tests:

```bash
dotnet test \
  ./DotNetDevOpsApp.Tests/DotNetDevOpsApp.Tests.csproj \
  --configuration Release
```

Expected:

```text
Passed: 3
Failed: 0
Total: 3
```

---

# 11. Run the Application Locally

Run:

```bash
dotnet run --project ./DotNetDevOpsApp
```

The application will listen on a development port such as:

```text
http://localhost:5177
```

Test:

```bash
curl http://localhost:5177/health
```

Stop the application with:

```text
Ctrl + C
```

---

# 12. Git Configuration

The project is stored in Git.

Check:

```bash
git status
```

Check the remote:

```bash
git remote -v
```

Example:

```text
origin https://github.com/Dineshkundo/DotNetDevOpsApp
```

---

# 13. .gitignore

Generated files should not be committed.

Example `.gitignore`:

```gitignore
**/bin/
**/obj/
publish/
.vscode/
*.user
*.suo
```

Important:

```text
publish/
```

is ignored because Jenkins generates the production artifact during the CI/CD pipeline.

---

# 14. Push Code to GitHub

Stage the project:

```bash
git add .
```

Check:

```bash
git status
```

Commit:

```bash
git commit -m "Add .NET 8 API and Jenkins CI/CD pipeline"
```

Push:

```bash
git push origin main
```

---

# 15. Jenkins EC2 #1 Setup

Jenkins is installed on the first EC2 instance.

Check Jenkins:

```bash
sudo systemctl status jenkins
```

Jenkins runs on:

```text
http://<JENKINS_PUBLIC_IP>:8080
```

Check the Jenkins port:

```bash
sudo ss -lntp | grep 8080
```

---

# 16. Jenkins Initial Password

If Jenkins requires the initial administrator password:

```bash
sudo cat /var/lib/jenkins/secrets/initialAdminPassword
```

Use that password during the initial Jenkins setup.

---

# 17. Verify .NET for Jenkins

The .NET SDK must be available to the Jenkins user.

Run:

```bash
sudo -u jenkins /usr/bin/dotnet --version
```

Expected:

```text
8.0.425
```

This is important because Jenkins executes the pipeline as the `jenkins` user.

---

# 18. Jenkins → Application EC2 SSH

Jenkins needs SSH access to EC2 #2.

The Jenkins user has an SSH key.

Test:

```bash
sudo -u jenkins -H bash -lc \
'ssh -o BatchMode=yes ubuntu@172.31.28.18 "hostname"'
```

If successful, the command returns the hostname of EC2 #2.

Example:

```text
ip-172-31-28-18
```

This proves:

```text
Jenkins EC2 #1
      │
      │ SSH
      ▼
Application EC2 #2
```

---

# 19. Application Server Directory

On EC2 #2:

```bash
sudo mkdir -p /opt/dotnetapp
```

Create the application user:

```bash
sudo useradd --system --no-create-home dotnetapp
```

Set ownership:

```bash
sudo chown dotnetapp:dotnetapp /opt/dotnetapp
```

Set permissions:

```bash
sudo chmod 755 /opt/dotnetapp
```

---

# 20. Why `/opt/dotnetapp`?

`/opt` is commonly used for optional third-party or locally managed applications.

Our application is stored here:

```text
/opt/dotnetapp
```

Example:

```text
/opt/dotnetapp/
├── DotNetDevOpsApp
├── DotNetDevOpsApp.dll
├── *.dll
├── *.json
└── runtime files
```

---

# 21. Self-Contained Deployment

This is one of the most important parts of the project.

The application is published using:

```bash
dotnet publish \
  DotNetDevOpsApp/DotNetDevOpsApp.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output publish
```

Important options:

```text
--runtime linux-x64
```

Means:

```text
Build for 64-bit Linux
```

And:

```text
--self-contained true
```

Means:

```text
Include the .NET runtime with the application.
```

Therefore, EC2 #2 does not need the .NET SDK/runtime installed.

---

# 22. Standalone Executable

The published application contains an executable:

```text
publish/DotNetDevOpsApp
```

Make it executable:

```bash
chmod +x publish/DotNetDevOpsApp
```

The application can then be started directly:

```bash
./DotNetDevOpsApp
```

This is a standalone/headless backend application.

There is no desktop UI required.

There is no browser required for the application itself.

---

# 23. systemd Service

Instead of manually running:

```bash
./DotNetDevOpsApp
```

we use Linux `systemd`.

Service file:

```text
/etc/systemd/system/dotnetapp.service
```

Configuration:

```ini
[Unit]
Description=DotNetDevOpsApp ASP.NET Core Application
After=network.target

[Service]
Type=exec
User=dotnetapp
Group=dotnetapp
WorkingDirectory=/opt/dotnetapp
ExecStart=/opt/dotnetapp/DotNetDevOpsApp
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://127.0.0.1:5000
NoNewPrivileges=true
PrivateTmp=true

[Install]
WantedBy=multi-user.target
```

---

# 24. Enable the Service

Reload systemd:

```bash
sudo systemctl daemon-reload
```

Enable at boot:

```bash
sudo systemctl enable dotnetapp
```

Start:

```bash
sudo systemctl start dotnetapp
```

Check:

```bash
sudo systemctl status dotnetapp
```

Expected:

```text
Active: active (running)
```

---

# 25. Application Logs

View logs:

```bash
sudo journalctl -u dotnetapp
```

Follow logs live:

```bash
sudo journalctl -u dotnetapp -f
```

This is useful when troubleshooting application failures.

---

# 26. Application Port

The application listens on:

```text
127.0.0.1:5000
```

Check:

```bash
curl http://127.0.0.1:5000/health
```

The application is intentionally bound to localhost.

This means it is not directly exposed to the Internet.

---

# 27. Jenkins Deployment Flow

The Jenkinsfile performs:

```text
Checkout
    ↓
Restore
    ↓
Build
    ↓
Test
    ↓
Publish
    ↓
Deploy
    ↓
Health Check
```

---

# 28. Restore

Jenkins restores the application:

```bash
dotnet restore DotNetDevOpsApp/DotNetDevOpsApp.csproj \
  --runtime linux-x64
```

And restores the test project:

```bash
dotnet restore DotNetDevOpsApp.Tests/DotNetDevOpsApp.Tests.csproj
```

---

# 29. Build

Jenkins builds:

```bash
dotnet build \
  DotNetDevOpsApp/DotNetDevOpsApp.csproj \
  --configuration Release \
  --no-restore
```

If the build fails, deployment stops.

---

# 30. Test

Jenkins executes:

```bash
dotnet test \
  DotNetDevOpsApp.Tests/DotNetDevOpsApp.Tests.csproj \
  --configuration Release \
  --no-restore
```

Currently the project has three tests.

Expected:

```text
Passed: 3
Failed: 0
```

If tests fail:

```text
Test ❌
   ↓
Deploy does not happen
```

This protects the production server from a broken build.

---

# 31. Publish

Jenkins creates a Linux self-contained deployment:

```bash
dotnet restore "$PROJECT" \
    --runtime linux-x64

dotnet publish "$PROJECT" \
    --configuration Release \
    --runtime linux-x64 \
    --self-contained true \
    --output publish \
    --no-restore
```

The result is stored in:

```text
publish/
```

---

# 32. Deploy

Jenkins creates a temporary release directory on EC2 #2:

```text
/tmp/dotnetapp-release-<BUILD_NUMBER>
```

It copies the published files using SCP.

Conceptually:

```text
Jenkins
   │
   │ SCP
   ▼
EC2 #2
/tmp/dotnetapp-release-1
```

Then the old application files are replaced.

---

# 33. Restart Application

Jenkins executes:

```bash
sudo systemctl stop dotnetapp
```

Then copies the new application:

```text
/tmp/release
       ↓
/opt/dotnetapp
```

Ownership is corrected:

```bash
sudo chown -R dotnetapp:dotnetapp /opt/dotnetapp
```

Executable permission is applied:

```bash
sudo chmod +x /opt/dotnetapp/DotNetDevOpsApp
```

Then:

```bash
sudo systemctl start dotnetapp
```

---

# 34. Deployment Health Check

After deployment Jenkins verifies systemd:

```bash
sudo systemctl is-active --quiet dotnetapp
```

Then it checks the application:

```bash
curl --fail \
  --silent \
  --show-error \
  http://127.0.0.1:5000/health
```

If the health check succeeds:

```text
Deployment successful
```

If it fails:

```text
Pipeline FAILURE
```

---

# 35. Jenkins Pipeline Result

A successful deployment looks like:

```text
Checkout       ✅
Restore        ✅
Build          ✅
Test           ✅
Publish        ✅
Deploy         ✅
Health Check   ✅
```

This means the application has successfully gone from:

```text
Developer
   ↓
GitHub
   ↓
Jenkins
   ↓
Linux artifact
   ↓
Application Server
   ↓
Running systemd service
```

---

# 36. Security / AWS Security Groups

## Jenkins EC2

Port:

```text
8080
```

should preferably be allowed only from your own public IP.

Do not permanently expose Jenkins to:

```text
0.0.0.0/0
```

unless you have a deliberate security setup.

## Application EC2

SSH:

```text
22
```

should preferably allow access from the Jenkins EC2 security group.

HTTP:

```text
80
```

can be public if the application is intended to be publicly accessible.

Application port:

```text
5000
```

should NOT be publicly exposed.

The application listens only on:

```text
127.0.0.1:5000
```

---

# 37. Optional Nginx Layer

If the application needs public HTTP access, Nginx can sit in front:

```text
Internet
   │
   ▼
Nginx :80
   │
   ▼
127.0.0.1:5000
   │
   ▼
DotNetDevOpsApp
```

Example Nginx configuration:

```nginx
server {
    listen 80;
    server_name _;

    location / {
        proxy_pass http://127.0.0.1:5000;
        proxy_http_version 1.1;

        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Test configuration:

```bash
sudo nginx -t
```

Restart:

```bash
sudo systemctl restart nginx
```

---

# 38. Complete Deployment Architecture

The final architecture is:

```text
                        GitHub
                           │
                           │ git push
                           ▼
                ┌─────────────────────┐
                │     Jenkins EC2     │
                │                     │
                │  Checkout           │
                │  Restore            │
                │  Build              │
                │  Test               │
                │  Publish            │
                └──────────┬──────────┘
                           │
                           │ SSH / SCP
                           ▼
                ┌─────────────────────┐
                │  Application EC2    │
                │                     │
                │  /opt/dotnetapp     │
                │         │           │
                │         ▼           │
                │      systemd        │
                │         │           │
                │         ▼           │
                │ DotNetDevOpsApp     │
                │    127.0.0.1:5000   │
                └─────────────────────┘
```

---

# 39. How to Deploy a New Version

Make a code change.

Example:

```text
ProductsController.cs
```

Then:

```bash
git add .
git commit -m "Update product API"
git push origin main
```

Jenkins gets the new code and runs:

```text
Checkout
   ↓
Restore
   ↓
Build
   ↓
Test
   ↓
Publish
   ↓
Deploy
   ↓
Restart
   ↓
Health Check
```

No manual application deployment is required.

---

# 40. Useful Troubleshooting Commands

## Jenkins status

```bash
sudo systemctl status jenkins
```

## Jenkins logs

```bash
sudo journalctl -u jenkins -f
```

## Application status

```bash
sudo systemctl status dotnetapp
```

## Application logs

```bash
sudo journalctl -u dotnetapp -f
```

## Check application port

```bash
sudo ss -lntp | grep 5000
```

## Test application

```bash
curl http://127.0.0.1:5000/health
```

## Check application files

```bash
ls -lh /opt/dotnetapp
```

## Check ownership

```bash
ls -ld /opt/dotnetapp
```

---

# 41. Important DevOps Concepts Learned

This project demonstrates:

### CI

Continuous Integration:

```text
Git push
   ↓
Build
   ↓
Test
```

### CD

Continuous Delivery/Deployment:

```text
Build
   ↓
Publish
   ↓
Deploy
   ↓
Restart
   ↓
Health Check
```

### Infrastructure

```text
AWS EC2
Linux
Security Groups
```

### Automation

```text
Jenkins
Bash
SSH
SCP
systemd
```

### Application Deployment

```text
.NET 8
Self-contained deployment
linux-x64
```

### Operations

```text
systemd
journalctl
health checks
Linux permissions
```

---

# 42. Beginner Explanation

If you are completely new, remember the process like this:

**1. Developer writes code**

```text
C# application
```

**2. Code is stored in GitHub**

```text
git push
```

**3. Jenkins gets the code**

```text
checkout
```

**4. Jenkins builds it**

```text
dotnet build
```

**5. Jenkins tests it**

```text
dotnet test
```

**6. Jenkins creates a Linux application**

```text
dotnet publish
```

**7. Jenkins copies it to EC2**

```text
SCP
```

**8. Linux starts the application**

```text
systemd
```

**9. Jenkins checks whether it works**

```text
/health
```

That is the complete CI/CD deployment cycle.

---

# 43. Future Improvements

Once this basic pipeline is understood, the project can be extended with:

- Docker
- Docker Compose
- Kubernetes
- Helm
- AWS ECR
- AWS ECS
- AWS EKS
- Terraform
- Ansible
- SonarQube
- Prometheus
- Grafana
- ELK/OpenSearch
- Slack/Teams notifications
- Blue-green deployment
- Rolling deployment
- Automated rollback
- HTTPS/TLS
- Secrets management
- Database integration
- Application-to-application communication
- Kafka/RabbitMQ
- Microservices

---

# 44. Final Goal

The main purpose of this project is not simply to deploy a .NET application.

It demonstrates the complete DevOps lifecycle:

```text
                SOURCE
                  │
                  ▼
                GitHub
                  │
                  ▼
              JENKINS CI
                  │
          ┌───────┼────────┐
          ▼       ▼        ▼
       Build     Test    Quality
          │
          ▼
       Package
          │
          ▼
       Deploy
          │
          ▼
       Linux EC2
          │
          ▼
       systemd
          │
          ▼
      Application
          │
          ▼
      Health Check
```

This is the foundation for moving from a beginner CI/CD project toward a production-style DevOps environment.
