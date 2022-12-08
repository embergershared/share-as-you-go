# Install a Helm chart

<https://learn.microsoft.com/en-us/training/modules/aks-app-package-management-using-helm/1-introduction>

SubscriptionId=$(az account list --query '[0].id' -o tsv)
. <(wget -q -O - <https://raw.githubusercontent.com/MicrosoftDocs/mslearn-aks/main/infrastructure/setup/setup.sh> ) -s $SubscriptionId -n learn-helm-deploy-aks --use-acr false --install-dot-net false

https://raw.githubusercontent.com/MicrosoftDocs/mslearn-aks/main/infrastructure/setup/init-env.sh

## Discover Helm

### Add a Helm repo

`helm repo add azure-marketplace https://marketplace.azurecr.io/helm/v1/repo`

`helm repo list`

`helm search repo aspnet`

### Deploy a Helm chart

`helm install aspnet-webapp azure-marketplace/aspnet-core`

`helm list`

`helm get manifest aspnet-webapp`

`kubectl get pods -o wide -w`

`helm delete aspnet-webapp`

### Install a Helm chart with set values

`helm install --set replicaCount=5 aspnet-webapp azure-marketplace/aspnet-core`

`kubectl get pods -o wide -w`

`helm delete aspnet-webapp`

### Review the Helm chart folder structure

`ls $HOME/.cache/helm/repository -l`

`helm dependency build ./drone-webapp-chart`

`helm install drone-webapp ./drone-webapp-chart`

`helm list`

`helm history drone-webapp`

`helm upgrade drone-webapp ./drone-webapp-chart`

`helm rollback drone-webapp 1`

