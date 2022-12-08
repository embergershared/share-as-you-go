# Lab: Deploying a workload to AKS

Reference: [3-exercise-create-aks-cluster](https://docs.microsoft.com/en-us/training/modules/aks-deploy-container-app/3-exercise-create-aks-cluster?tabs=windows)

Commands to run in Bash cloud shell

## 1. Export env var key-value pairs

```bash
export RESOURCE_GROUP=rg-contoso-video
export CLUSTER_NAME=aks-contoso-video
export LOCATION=eastus
```

```pwsh
$RESOURCE_GROUP="rg-contoso-video"
$CLUSTER_NAME="aks-contoso-video"
$LOCATION="eastus"
```

## 2. Create the AKS Cluster

### Create Resource Group

```both
az group create --name=$RESOURCE_GROUP --location=$LOCATION
```

### Create the AKS cluster

```bash
az aks create \
--resource-group $RESOURCE_GROUP \
--name $CLUSTER_NAME \
--node-count 2 \
--enable-addons http_application_routing \
--generate-ssh-keys \
--node-vm-size Standard_B2s \
--network-plugin azure \
--windows-admin-username localadmin
```

```pwsh
az aks create `
--resource-group $RESOURCE_GROUP `
--name $CLUSTER_NAME `
--node-count 2 `
--enable-addons http_application_routing `
--generate-ssh-keys `
--node-vm-size Standard_B2s `
--network-plugin azure `
--windows-admin-username localadmin
```

Note: it will ask for the windows nodes password (length must be >14)

### Add a Windows node pool to the AKS cluster

```bash
az aks nodepool add \
--resource-group $RESOURCE_GROUP \
--cluster-name $CLUSTER_NAME \
--name winp \
--node-count 2 \
--node-vm-size Standard_B2s \
--os-type Windows
```

```pwsh
az aks nodepool add `
--resource-group $RESOURCE_GROUP `
--cluster-name $CLUSTER_NAME `
--name winp `
--node-count 2 `
--node-vm-size Standard_B2s `
--os-type Windows
```

## 3. Connect/Manage cluster with kubectl

### Get AKS cluster credentials

```both
az aks get-credentials --name $CLUSTER_NAME --resource-group $RESOURCE_GROUP
```

Note: it is added to the `~/.kube/config` (Linux) | `C:\Users\emberger\.kube\config` (Windows)

### Get nodes list

```both
kubectl get nodes
```

## 4. Create a deployment manifest

### Create a deployment manifest YAML

```both
touch deployment.yaml
```

### Edit the file

### Apply the deployment

```both
kubectl apply -f ./deployment.yaml
```

### Check the deployment status

```both
kubectl get deploy contoso-website
```

Note: get more details with kubectl describe deployment.apps/contoso-website

## 5. Enable network access to an application

### Create manifest file for the service

```bash
touch service.yaml
```

### Apply the service

```both
kubectl apply -f ./service.yaml
```

### Check the service deployment

```both
kubectl get service contoso-website
```

## 6. Create an ingress

### Create the ingress manifest

```bash
touch ingress.yaml
```

### Get the host's FQDN

```bash
az aks show \
-g $RESOURCE_GROUP \
-n $CLUSTER_NAME \
-o tsv \
--query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
```

```pwsh
az aks show `
-g $RESOURCE_GROUP `
-n $CLUSTER_NAME `
-o tsv `
--query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
```

Note: returned `<23aede0a825d49ebaf34.eastus.aksapp.io>`

### Update the ingress.yaml

Line 10:

`    - host: contoso.23aede0a825d49ebaf34.eastus.aksapp.io`

### Apply the ingress

```both
kubectl apply -f ./ingress.yaml
```

### Check the ingress

```both
kubectl get ingress contoso-website
```

### Check the website

[http://contoso.9fae41b60d4443cfba7d.eastus.aksapp.io/](http://contoso.9fae41b60d4443cfba7d.eastus.aksapp.io/)

Note: if getting an error:

- check the http://-IP- to get an nginx 404 error, then 
- check the DNS domain resolves: `nslookup contoso.9fae41b60d4443cfba7d.eastus.aksapp.io`

## 7. - DO NOT PERFORM - Clean-up

### Delete the Resource Group

```both
az group delete --name $RESOURCE_GROUP
```

### Delete the kubectl context

```both
kubectl config delete-context aks-contoso-video
```
