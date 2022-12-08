# Cluster Auto scaler

Reference:
<https://learn.microsoft.com/en-us/training/modules/aks-cluster-autoscaling/3-exercise-cluster-autoscaler>


## Scale up contoso-video

```bash
az login

export RESOURCE_GROUP=rg-contoso-video-keda
export CLUSTER_NAME=contoso-video

az aks get-credentials --name $CLUSTER_NAME --resource-group $RESOURCE_GROUP

curl -L https://aka.ms/learn-cluster-scalability-init?v=$RANDOM | bash
```

## Enable autoscaler

```pwsh
$RESOURCE_GROUP="rg-contoso-video-keda"
$LOCATION="westus2"
$CLUSTER_NAME="contoso-video-keda"
```

```bash
az aks update `
  -g $RESOURCE_GROUP `
  -n $CLUSTER_NAME `
  --enable-cluster-autoscaler `
  --min-count 1 `
  --max-count 10
```

List the nodes

```pwsh
kubectl get nodes -w
```

### Tweak profile

```pwsh
az aks update `
  -g $RESOURCE_GROUP `
  -n $CLUSTER_NAME `
  --cluster-autoscaler-profile scan-interval=5s `
    scale-down-unready-time=5m `
    scale-down-delay-after-add=5m
```

### See autoscaler status

```pwsh
kubectl describe cm cluster-autoscaler-status -n kube-system
```
