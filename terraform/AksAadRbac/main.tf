#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
# - Dependencies
#--------------------------------------------------------------
data "azurerm_client_config" "this" {
  provider = azurerm.azint
}

#--------------------------------------------------------------
# - Resource Group
#--------------------------------------------------------------
#   / Create a Resource Group
resource "azurerm_resource_group" "this" {
  provider = azurerm.azint

  name     = "rg-ussc-azint-${var.name_part1}-${var.name_part2}"
  location = "South Central US"

  tags = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}

#--------------------------------------------------------------
# - AKS Cluster and associated resources
#--------------------------------------------------------------
#   / Create AKS Cluster with Azure AD AuthN + Azure AD RBAC AuthZ + local accounts disabled
resource "azurerm_kubernetes_cluster" "this" {
  provider = azurerm.azint

  name                   = "aks-${var.name_part1}-${var.name_part2}"
  location               = azurerm_resource_group.this.location
  resource_group_name    = azurerm_resource_group.this.name
  dns_prefix             = "aks${var.name_part2}"
  local_account_disabled = true
  azure_policy_enabled   = true

  default_node_pool {
    name       = "systempool"
    node_count = 2
    vm_size    = "Standard_B2s"
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin = "azure"
  }

  # Kubernetes Authorization = RBAC
  role_based_access_control_enabled = true

  # Azure AD integration + AD Managed RBAC
  azure_active_directory_role_based_access_control {
    managed                = true
    azure_rbac_enabled     = true
    admin_group_object_ids = var.aad_admin_group_object_ids
    tenant_id              = data.azurerm_client_config.this.tenant_id
  }

  microsoft_defender {
    log_analytics_workspace_id = var.law_id
  }

  oms_agent {
    log_analytics_workspace_id = var.law_id
  }

  tags = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}
#   / Get Diagnostics settings categories for AKS
data "azurerm_monitor_diagnostic_categories" "this" {
  provider = azurerm.azint

  resource_id = azurerm_kubernetes_cluster.this.id
}
#   / Setup Diagnostics settings on the AKS Cluster
resource "azurerm_monitor_diagnostic_setting" "this" {
  provider = azurerm.azint

  name                       = "${azurerm_kubernetes_cluster.this.name}-diag"
  target_resource_id         = azurerm_kubernetes_cluster.this.id
  log_analytics_workspace_id = try(var.law_id, null)
  storage_account_id         = try(var.storage_account_id, null)

  dynamic "log" {
    for_each = data.azurerm_monitor_diagnostic_categories.this.log_category_types
    content {
      category = log.value
      enabled  = true

      retention_policy {
        enabled = true
        days    = 7
      }
    }
  }

  dynamic "metric" {
    for_each = data.azurerm_monitor_diagnostic_categories.this.metrics
    content {
      category = metric.value
      enabled  = true

      retention_policy {
        enabled = true
        days    = 7
      }
    }
  }
}
#   / Create Role Assignment for the AKS CLUSTER admin
resource "azurerm_role_assignment" "cluster_admin" {
  provider = azurerm.azint

  # Required
  principal_id = var.principal_id

  # Rule = id XOR name
  role_definition_id   = null
  role_definition_name = "Azure Kubernetes Service RBAC Cluster Admin"

  scope = azurerm_kubernetes_cluster.this.id
}

/*
# Connect with kubectl through AzureAD:
az aks get-credentials -g rg-ussc-azint-research-aks-aadrbac -n aks-aadrbac

# First query promtps for Azure AD authentication with device login
# Then kubectl commands that are cluster-scoped are successfull:
kubectl get nodes
kubectl get pods -A
kubectl get ns

Create 2 namespaces:
kubectl create ns app-a
kubectl create ns app-b

# Trying to connect with a cluster local account is blocked
az aks get-credentials -g rg-ussc-azint-research-aks-aadrbac -n aks-aadrbac --admin
#*/

/*
#   / Create Role Assignment for an admin in the namespace (and delete the Cluster Admin role assignment)
resource "azurerm_role_assignment" "app-a_ns_admin" {
  provider = azurerm.azint

  # Required
  principal_id = var.principal_id

  # Rule = id XOR name
  role_definition_id   = null
  role_definition_name = "Azure Kubernetes Service RBAC Admin"

  scope = "${azurerm_kubernetes_cluster.this.id}/namespaces/app-a"
}

/*
# It is then possible to manage the namespace's resources with kubectl
## Get all resources in the namespace
kubectl get all --namespace app-a
## Create a nginx pod
kubectl run nginx --image=nginx --restart=Never --namespace app-a
## See the logs of this pod
kubectl logs pod/nginx --namespace app-a 

Note: the role assignment at namespace scope is not visible through the portal in the AKS IAM blade,
but it can be seen in the user in AzureAD blade: https://ms.portal.azure.com/#view/Microsoft_AAD_UsersAndTenants/UserProfileMenuBlade/~/SubscriptionResources/userId/<Principal Id>

AKS AAD RBAC has the 4 following builtin roles:
https://learn.microsoft.com/en-us/azure/aks/manage-azure-rbac#create-role-assignments-for-users-to-access-cluster

Azure Kubernetes Service RBAC Reader	        Allows read-only access to see most objects in a namespace. It doesn't allow viewing roles or role bindings. This role doesn't allow viewing Secrets, since reading the contents of Secrets enables access to ServiceAccount credentials in the namespace, which would allow API access as any ServiceAccount in the namespace (a form of privilege escalation)
Azure Kubernetes Service RBAC Writer	        Allows read/write access to most objects in a namespace. This role doesn't allow viewing or modifying roles or role bindings. However, this role allows accessing Secrets and running Pods as any ServiceAccount in the namespace, so it can be used to gain the API access levels of any ServiceAccount in the namespace.
Azure Kubernetes Service RBAC Admin	          Allows admin access, intended to be granted within a namespace. Allows read/write access to most resources in a namespace (or cluster scope), including the ability to create roles and role bindings within the namespace. This role doesn't allow write access to resource quota or to the namespace itself.
Azure Kubernetes Service RBAC Cluster Admin	  Allows super-user access to perform any action on any resource. It gives full control over every resource in the cluster and in all namespaces.


# Additional Kubernetes roles can be created, then binded to Azure AD Identities (Users, groups)
# Example here: https://learn.microsoft.com/en-us/azure/aks/azure-ad-rbac

#*/

/* Doesn't work:
# Tried to use "RBAC Admin" role to see Cluster level resources
resource "azurerm_role_assignment" "all_ns_admin" {
  provider = azurerm.azint

  # Required
  principal_id = var.principal_id

  # Rule = id XOR name
  role_definition_id   = null
  role_definition_name = "Azure Kubernetes Service RBAC Admin"

  scope = azurerm_kubernetes_cluster.this.id #/namespaces/default"
}
#*/
