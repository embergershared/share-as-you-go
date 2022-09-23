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

  name = lower(substr("law-${var.name_base}", 0, 64))
  #name                               = lower("law-ussc-azint-learning-lzlaw")
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

  name = lower(substr("appi-${var.name_base}", 0, 64))
  #name                = "appi-ussc-azint-learning-lzappins"
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
