<https://learn.microsoft.com/en-us/training/modules/aks-app-scale-keda/>

# Scaling using KEDA

## Create the cluster

```pwsh
$RESOURCE_GROUP="rg-contoso-video-keda"
$LOCATION="westus2"
$CLUSTER_NAME="contoso-video-keda"

az group create -n $RESOURCE_GROUP -l $LOCATION

az aks create `
  -g $RESOURCE_GROUP `
  -n $CLUSTER_NAME `
  --node-count 1 `
  --node-vm-size Standard_DS3_v2 `
  --generate-ssh-keys `
  --node-osdisk-type Ephemeral `
  --enable-addons http_application_routing

az aks get-credentials -n $CLUSTER_NAME -g $RESOURCE_GROUP
```

## Create the Azure Redis cache

```pwsh
$REDIS_NAME="redis-contoso-video-1285562"

az redis create --location $LOCATION --name $REDIS_NAME --resource-group $RESOURCE_GROUP --sku Basic --vm-size c0 --enable-non-ssl-port

$REDIS_HOST = az redis show -n $REDIS_NAME -g $RESOURCE_GROUP -o tsv --query "hostName"
$REDIS_KEY = az redis list-keys --name $REDIS_NAME --resource-group $RESOURCE_GROUP -o tsv --query "primaryKey"
```

## Deploy KEDA

### Check access to cluster

``` bash
kubectl get nodes
```

### Deploy KEDA

```bash
kubectl apply -f https://github.com/kedacore/keda/releases/download/v2.2.0/keda-2.2.0.yaml
```

### Check KEDA installation

```bash
kubectl get pods --namespace keda
```

### Create a Redis container locally to connect to the Azure Cache for Redis we created earlier

```pwsh
docker run -it --rm redis redis-cli -h $REDIS_HOST -a $REDIS_KEY
```

### Push data in the redis cache
```
lpush keda Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris eget interdum felis, ac ultricies nulla. Fusce vehicula mattis laoreet. Quisque facilisis bibendum dui, at scelerisque nulla hendrerit sed. Sed rutrum augue arcu, id maximus felis sollicitudin eget. Curabitur non libero rhoncus, pellentesque orci a, tincidunt sapien. Suspendisse laoreet vulputate sagittis. Vivamus ac magna lacus. Etiam sagittis facilisis dictum. Phasellus faucibus sagittis libero, ac semper lorem commodo in. Quisque tortor lorem, sollicitudin non odio sit amet, finibus molestie eros. Proin aliquam laoreet eros, sed dapibus tortor euismod quis. Maecenas sed viverra sem, at porta sapien. Sed sollicitudin arcu leo, vitae elementum
```

### Check result

```
llen keda
```

### Exit Redis

```
exit
```

### Tune then deploy `deployment.yaml`

```bash
kubectl apply -f ./deployment.yaml
```

### Check deployment

```bash
kubectl get pods
```


### Scaling with KEDA

#### Apply the KEDA scaling

```bash
kubectl apply -f ./scaled-object.yaml
```

See the CRD:

```bash
kubectl get pods -w
```

#### Watch the pods

```bash
kubectl get pods -w
```

## Clean the resources
