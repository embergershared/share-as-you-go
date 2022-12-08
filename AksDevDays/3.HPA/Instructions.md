<https://learn.microsoft.com/en-us/training/modules/aks-application-autoscaling-native/3-exercise-scaling>

`kubectl apply -f hpa.yaml`

`kubectl get hpa contoso-website`

let's generate traffic using a tool called hey, we'll run the command:
`hey -n 100000 -c 100 -m GET http://contoso.23aede0a825d49ebaf34.eastus.aksapp.io`

Need HEY. Sources are here: [https://github.com/rakyll/hey](https://github.com/rakyll/hey)

Monitor HPA:
`kubectl get hpa contoso-website -w`
and the deployment:
`kubectl get deploy contoso-website -w`
