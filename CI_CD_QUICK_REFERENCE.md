# CI/CD Pipeline - Quick Reference

## 📊 What Was Created

### Files Added:
1. **`.github/workflows/ci-cd.yml`** - GitHub Actions workflow (the automation script)
2. **`Dockerfile`** - Blueprint for containerizing your app
3. **`CI_CD_SETUP_GUIDE.md`** - Complete setup instructions

---

## 🔄 Pipeline Flow

```
Developer pushes to main branch
         ⬇️
GitHub Actions triggered
         ⬇️
┌─────────────────────────────────────┐
│ Job 1: Build & Test                 │
├─────────────────────────────────────┤
│ ✓ Restore dependencies              │
│ ✓ Build application                 │
│ ✓ Run unit tests                    │
│ ✓ Run integration tests             │
│ ✓ Generate test report              │
└─────────────────────────────────────┘
         ⬇️ (only if Job 1 passes)
┌─────────────────────────────────────┐
│ Job 2: Build & Push Docker Image    │
├─────────────────────────────────────┤
│ ✓ Build Docker container            │
│ ✓ Push to Docker Hub                │
│ ✓ Tag with commit SHA               │
└─────────────────────────────────────┘
         ⬇️ (only if Job 2 passes)
┌─────────────────────────────────────┐
│ Job 3: Deploy to Azure              │
├─────────────────────────────────────┤
│ ✓ Authenticate with Azure           │
│ ✓ Update Container App              │
│ ✓ Deploy new image                  │
│ ✓ App goes live!                    │
└─────────────────────────────────────┘
         ⬇️
App is running in Azure ☁️
Users can access it via URL
```

---

## 🛠️ What Each File Does

### `.github/workflows/ci-cd.yml`
- **Trigger**: When you `git push` to `main` branch
- **Build step**: Compiles your C# code
- **Test step**: Runs both unit and integration tests
- **Report**: Publishes test results to GitHub
- **Docker**: Creates container image
- **Deploy**: Pushes to Azure

### `Dockerfile`
- **Multi-stage build** for efficiency
- **Stage 1 (Build)**: Compiles your application
- **Stage 2 (Publish)**: Prepares for release
- **Stage 3 (Runtime)**: Minimal runtime image (~200MB)
- **Ports**: Exposes 80 and 443 for HTTP/HTTPS

---

## 📝 Next Steps (In Order)

### 1️⃣ Get Accounts (if you don't have them)
- [ ] Docker Hub: https://hub.docker.com/ (free)
- [ ] Azure: https://azure.microsoft.com/ (free trial)

### 2️⃣ Add GitHub Secrets (GitHub Settings)
- [ ] `DOCKER_USERNAME`
- [ ] `DOCKER_PASSWORD`
- [ ] `AZURE_CREDENTIALS`
- [ ] `AZURE_RESOURCE_GROUP`

### 3️⃣ Set Up Azure Resources
```bash
# Create resource group
az group create --name customerapi-rg --location eastus

# Create container registry
az acr create --resource-group customerapi-rg --name customerapiregistry --sku Basic

# Create container app environment
az containerapp env create --name customerapi-env --resource-group customerapi-rg --location eastus

# Create container app
az containerapp create --name customerapi --resource-group customerapi-rg --environment customerapi-env --image mcr.microsoft.com/azuredocs/containerapps-helloworld:latest --target-port 80 --ingress 'external'
```

### 4️⃣ Test the Pipeline
```bash
# Make a small change to your code
git add .
git commit -m "Test CI/CD"
git push origin main

# Watch it run: GitHub repo → Actions tab
```

---

## ✅ How to Know It Worked

**Success Indicators:**
- ✅ All 3 jobs show green checkmarks on GitHub Actions
- ✅ Test report shows 100% tests passing
- ✅ Docker image appears in Docker Hub
- ✅ Container App URL shows your running app

**Check Results:**
1. Go to GitHub repo → Actions tab
2. Click the latest workflow run
3. Expand each job to see details
4. View artifacts (test reports)

---

## 🚀 After First Deploy

### Your app is now:
- **Automated**: Tests run on every push
- **Reliable**: Bad code is caught before deployment
- **Scalable**: Runs in Azure with automatic scaling
- **Monitored**: See test results and deployment history

### Subsequent Updates:
```bash
# Just push code - pipeline handles everything!
git push origin main

# Pipeline automatically:
# 1. Tests your code
# 2. Builds Docker image
# 3. Deploys to Azure
```

---

## 🐛 Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| Docker login fails | Check `DOCKER_PASSWORD` - use personal access token instead of password |
| Azure deployment fails | Verify `AZURE_CREDENTIALS` JSON is complete and service principal has permissions |
| Tests fail in pipeline but pass locally | Check file paths - workflow uses Linux paths (case-sensitive) |
| Pipeline won't trigger | Make sure you pushed to `main` branch, not another branch |

---

## 📚 Learn More

- **GitHub Actions**: https://github.com/features/actions
- **Docker**: https://docs.docker.com/
- **Azure Container Apps**: https://learn.microsoft.com/en-us/azure/container-apps/

---

## 💡 Key Concepts

| Term | Meaning |
|------|---------|
| **CI/CD** | Continuous Integration/Deployment - automate build, test, deploy |
| **Workflow** | A GitHub Actions automation script |
| **Job** | A task within a workflow (build, test, deploy) |
| **Step** | An individual command within a job |
| **Secret** | Encrypted credential stored in GitHub |
| **Docker Image** | A packaged version of your app with all dependencies |
| **Container** | A running instance of a Docker image |
| **Azure Container Apps** | Managed service to run containers in Azure |

---

## 📞 When Things Go Wrong

1. **Check workflow logs**: GitHub Actions shows detailed error messages
2. **Run locally first**: Test `dotnet build` and `dotnet test` on your machine
3. **Verify secrets**: Make sure all 4 GitHub secrets are set correctly
4. **Check Azure resources**: Verify resource group and container app exist
5. **Review Dockerfile**: Ensure paths and commands are correct for your project structure

---

## 🎯 Success Criteria

Pipeline is working when:
- ✅ Every `git push` to main triggers the workflow
- ✅ Build completes in ~2-3 minutes
- ✅ All tests pass (0 failures)
- ✅ Docker image is pushed to Docker Hub
- ✅ App automatically deploys to Azure
- ✅ You can visit the app URL in a browser

---

**Happy Deploying! 🚀**
