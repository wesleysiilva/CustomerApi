# CI/CD Pipeline Setup Guide

This guide walks you through setting up the GitHub Actions CI/CD pipeline for automatic building, testing, and deployment.

## Overview

The pipeline consists of 3 main jobs:
1. **Build & Test**: Compiles code, runs unit/integration tests
2. **Build & Push Docker Image**: Creates a Docker container image
3. **Deploy to Azure**: Deploys the image to Azure Container Apps

## Prerequisites

Before setting up, you need:

### 1. Docker Hub Account (Free)
- Go to https://hub.docker.com/
- Sign up for a free account
- Note your username and password (you'll need these)

### 2. Azure Account
- Go to https://azure.microsoft.com/
- Create a free account (includes $200 credit for 30 days)
- You'll need to set up Azure Container Apps and Container Registry

### 3. GitHub Repository
- Your code must be on GitHub (push to main branch)

---

## Step 1: Set Up GitHub Secrets

These are encrypted credentials stored in GitHub. The pipeline will use them without exposing them publicly.

### How to Add Secrets to GitHub:

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add the following secrets:

#### Secret 1: Docker Username
- **Name**: `DOCKER_USERNAME`
- **Value**: Your Docker Hub username

#### Secret 2: Docker Password
- **Name**: `DOCKER_PASSWORD`
- **Value**: Your Docker Hub password (or access token)

#### Secret 3: Azure Credentials
- **Name**: `AZURE_CREDENTIALS`
- **Value**: Azure service principal (see instructions below)

#### Secret 4: Azure Resource Group
- **Name**: `AZURE_RESOURCE_GROUP`
- **Value**: Your Azure resource group name

---

## Step 2: Set Up Azure

### 2a. Create Azure Resource Group

```bash
az group create --name customerapi-rg --location eastus
```

### 2b. Create Azure Container Registry (to store Docker images)

```bash
az acr create --resource-group customerapi-rg --name customerapiregistry --sku Basic
```

### 2c. Create Azure Service Principal (for GitHub to authenticate)

```bash
az ad sp create-for-rbac --name "github-actions" --role Contributor --scopes /subscriptions/{subscription-id}/resourceGroups/customerapi-rg
```

This command outputs JSON with fields like:
- `appId` (maps to `clientId`)
- `password` (maps to `clientSecret`)
- `tenant` (maps to `tenantId`)

Before pasting as the `AZURE_CREDENTIALS` secret, transform the JSON by renaming:
```json
{
  "clientId": "<appId>",
  "clientSecret": "<password>",
  "subscriptionId": "<subscription-id>",
  "tenantId": "<tenant>"
}
```

Replace `<subscription-id>` with your actual subscription ID from the command output or get it with: `az account show --query id -o tsv`

**Paste the transformed JSON as the `AZURE_CREDENTIALS` secret value.**

### 2d. Create Container Apps Environment

```bash
az containerapp env create  --name customerapi-env --resource-group customerapi-rg --location eastus 
```

### 2e. Create Container App

```bash
az containerapp create --name customerapi --resource-group customerapi-rg --environment customerapi-env --image mcr.microsoft.com/azuredocs/containerapps-helloworld:latest --target-port 80 --ingress 'external' --query properties.configuration.ingress.fqdn
```

---

## Step 3: Update Workflow File (if needed)

The `.github/workflows/ci-cd.yml` file is already created. You may need to adjust:

- **Docker registry**: Currently uses Docker Hub. To use Azure Container Registry instead, update the login and image URLs
- **Azure resource group**: Update `${{ secrets.AZURE_RESOURCE_GROUP }}` if different
- **.NET version**: Currently 10.0. Change if you use a different version

---

## Step 4: Test the Pipeline

### Manual Trigger:

1. Make a small change to your code
2. Commit and push to `main` branch:
   ```bash
   git add .
   git commit -m "Test CI/CD pipeline"
   git push origin main
   ```

3. Go to your GitHub repository → **Actions**
4. Watch the pipeline run in real-time
5. Click on the workflow to see logs

### View Results:

- ✅ All green = Success
- ❌ Red = Failed (click to see error logs)
- Test results appear under "Artifacts"

---

## Step 5: Deployment Flow

Once the pipeline succeeds:

1. **Build succeeds** → Code compiles ✓
2. **Tests pass** → Unit and integration tests pass ✓
3. **Docker image created** → Pushed to Docker Hub/ACR ✓
4. **Deployed to Azure** → Your app is live! 🎉

Your app will be accessible at: `https://customerapi.{region}.azurecontainerapps.io`

---

## Troubleshooting

### Pipeline Fails at Docker Login
- Verify `DOCKER_USERNAME` and `DOCKER_PASSWORD` are correct
- Check if Docker account allows token auth (create personal access token if needed)

### Pipeline Fails at Azure Login
- Verify `AZURE_CREDENTIALS` JSON is complete and valid
- Check service principal has correct permissions

### Tests Fail
- Check if paths in workflow match your actual project structure
- Run tests locally: `dotnet test` from `src/` directory

### Docker Image Not Found
- Verify image was built: Check Docker Hub or ACR
- Check image tag: Should be `username/customerapi:latest`

---

## Next Steps

1. ✅ Set up Docker Hub account
2. ✅ Set up Azure account
3. ✅ Add GitHub secrets
4. ✅ Create Azure resources
5. ✅ Push code to trigger pipeline
6. ✅ Monitor and celebrate! 🚀

---

## Useful Commands

Monitor pipeline:
```bash
# View GitHub Actions locally (requires gh CLI)
gh run list
gh run view {run-id}
```

View Azure deployment:
```bash
# Get app URL
az containerapp show -n customerapi -g customerapi-rg --query properties.configuration.ingress.fqdn

# View logs
az containerapp logs show -n customerapi -g customerapi-rg
```

---

## Cost Considerations

- **Docker Hub**: Free tier (limited builds)
- **Azure**: ~$20-50/month for small app (free tier available for first month)
- **GitHub Actions**: Free tier (2,000 minutes/month for public repos)

---

## Security Best Practices

✅ Never commit secrets to git
✅ Use GitHub Secrets for credentials
✅ Rotate access tokens periodically
✅ Limit service principal permissions
✅ Review pipeline logs for sensitive data

