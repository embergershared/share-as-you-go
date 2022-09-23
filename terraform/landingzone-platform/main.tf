#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
#   Platform Landing Zone Resource Group
#--------------------------------------------------------------
#   / Resource Group
resource "azurerm_resource_group" "this" {
  provider = azurerm.azint

  name     = "rg-${var.name_base}"
  location = "South Central US"
  tags     = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}

#--------------------------------------------------------------
#   Platform Landing Zone Storage Account
#--------------------------------------------------------------
#   / Storage Account
resource "azurerm_storage_account" "this" {
  provider = azurerm.azint

  name                     = lower(substr("st${replace(var.name_base, "-", "")}", 0, 24))
  location                 = azurerm_resource_group.this.location
  resource_group_name      = azurerm_resource_group.this.name
  account_kind             = "StorageV2"
  account_tier             = "Standard"
  account_replication_type = "LRS"

  allow_nested_items_to_be_public = false    #Disable anonymous public read access to containers and blobs
  enable_https_traffic_only       = true     #Require secure transfer (HTTPS) to the storage account for REST API Operations
  min_tls_version                 = "TLS1_2" #Configure the minimum required version of Transport Layer Security (TLS) for a storage account and require TLS Version1.2

  identity {
    type = "SystemAssigned"
  }

  tags = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}
#   / Terraform States container
resource "azurerm_storage_container" "this" {
  provider = azurerm.azint
  for_each = toset(["tfstates"])

  name                  = lower(each.key)
  storage_account_name  = azurerm_storage_account.this.name
  container_access_type = "private"
}
#   / Gather current SPN
data "azurerm_client_config" "this" {
  provider = azurerm.azint
}
#   / Assign Storage Blob Data contributor role to current SPN
resource "azurerm_role_assignment" "ra_spn_on_stblob" {
  provider = azurerm.azint

  # Dependency forced to allow successfull deletion
  depends_on = [
    azurerm_storage_account.this
  ]

  principal_id         = data.azurerm_client_config.this.object_id
  role_definition_name = "Storage Blob Data Contributor"
  scope                = azurerm_storage_account.this.id

  description = "Assigning the `Storage Blob Data Contributor` role to the current client config to allow its access to the storage account."
}
#   / Storage Account Networking rules
resource "azurerm_storage_account_network_rules" "this" {
  provider = azurerm.azint

  # Prevents locking the Storage Account before all resources are created
  depends_on = [
    azurerm_storage_account.this,
    azurerm_storage_container.this
  ]

  storage_account_id         = azurerm_storage_account.this.id
  default_action             = "Deny"
  ip_rules                   = var.network_ip_rules
  virtual_network_subnet_ids = []
  bypass                     = ["AzureServices"]
}

#--------------------------------------------------------------
#   Platform Landing Zone Monitoring resources
#--------------------------------------------------------------
#   / Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "this" {
  provider = azurerm.azint

  name                               = lower(substr("law-${var.name_base}", 0, 64))
  location                           = azurerm_resource_group.this.location
  resource_group_name                = azurerm_resource_group.this.name
  sku                                = "PerGB2018"
  retention_in_days                  = 30
  internet_ingestion_enabled         = "true"
  internet_query_enabled             = "true"
  daily_quota_gb                     = null #var.sku == "Free" ? null : var.daily_quota_gb
  reservation_capacity_in_gb_per_day = null #var.sku == "CapacityReservation" ? var.reservation_capacity_in_gb_per_day : null

  tags = local.base_tags
}
#   / Application Insights
resource "azurerm_application_insights" "this" {
  provider = azurerm.azint

  name                = lower(substr("appi-${var.name_base}", 0, 64))
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  workspace_id        = azurerm_log_analytics_workspace.this.id

  application_type                      = "web"
  retention_in_days                     = 30
  daily_data_cap_in_gb                  = 1
  daily_data_cap_notifications_disabled = false
  internet_ingestion_enabled            = true
  internet_query_enabled                = true
  local_authentication_disabled         = false

  tags = local.base_tags
}

#--------------------------------------------------------------
#   Platform Landing Zone Key Vault
#--------------------------------------------------------------
#   / Create the Key Vault
resource "azurerm_key_vault" "this" {
  provider = azurerm.azint

  name                = lower(substr("kv-${var.name_base}", 0, 24))
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  tenant_id           = data.azurerm_client_config.this.tenant_id

  sku_name                        = "standard"
  enabled_for_deployment          = false
  enabled_for_disk_encryption     = true
  enabled_for_template_deployment = false

  enable_rbac_authorization = true

  purge_protection_enabled   = false # Should be set to true for PreProd & Production environments.
  soft_delete_retention_days = "90"

  network_acls {
    default_action             = "Deny"
    ip_rules                   = var.network_ip_rules
    virtual_network_subnet_ids = []
    bypass                     = "AzureServices"
  }

  tags = local.base_tags
}
#   / Assign Key Vault Administrator Role to the current SPN
resource "azurerm_role_assignment" "ra_spn_on_kv" {
  provider = azurerm.azint

  # Dependency forced to allow successfull deletion
  depends_on = [
    azurerm_key_vault.this
  ]

  principal_id         = data.azurerm_client_config.this.object_id
  role_definition_name = "Key Vault Administrator"
  scope                = azurerm_key_vault.this.id
}
#   / Store Storage Account Access Key in Key Vault (now that we have one)
resource "azurerm_key_vault_secret" "this" {
  provider = azurerm.azint

  # Dependency forced to allow successfull deletion
  depends_on = [
    azurerm_role_assignment.ra_spn_on_kv
  ]

  count = var.persist_access_key == true ? 1 : 0

  name         = "${azurerm_storage_account.this.name}-access-key"
  value        = azurerm_storage_account.this.primary_access_key
  key_vault_id = azurerm_key_vault.this.id

  tags = local.base_tags
}

#--------------------------------------------------------------
#   Platform Landing Zone Virtual Network
#--------------------------------------------------------------
#   / Create the Virtual Network
resource "azurerm_virtual_network" "this" {
  provider = azurerm.azint

  name                = lower(substr("vnet-${var.name_base}", 0, 80))
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  address_space       = var.address_space
  dns_servers         = var.dns_servers

  tags = local.base_tags
}

#   / Create the Subnets
resource "azurerm_subnet" "this" {
  provider = azurerm.azint

  resource_group_name  = azurerm_resource_group.this.name
  virtual_network_name = azurerm_virtual_network.this.name

  for_each = var.subnets

  name                                      = each.key
  address_prefixes                          = each.value["address_prefixes"]
  service_endpoints                         = lookup(each.value, "service_endpoints", null)
  private_endpoint_network_policies_enabled = lookup(each.value, "pe_enable", false)
  # enforce_private_link_service_network_policies = lookup(each.value, "pe_enable", false)

  dynamic "delegation" {
    for_each = lookup(each.value, "delegation", [])
    content {
      name = lookup(delegation.value, "name", null)
      dynamic "service_delegation" {
        for_each = lookup(delegation.value, "service_delegation", [])
        content {
          name    = lookup(service_delegation.value, "name", null)
          actions = lookup(service_delegation.value, "actions", null)
        }
      }
    }
  }

  depends_on = [azurerm_virtual_network.this]
}
#*/
