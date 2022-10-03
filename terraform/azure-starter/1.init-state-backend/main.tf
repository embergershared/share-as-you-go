#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
#   Terraform States Resource Group
#--------------------------------------------------------------
#   / Resource Group
resource "azurerm_resource_group" "this" {
  provider = azurerm.azint

  name     = "rg-${var.codes}-${var.base_name}-${var.suffix}"
  location = var.location

  tags = local.base_tags
  #lifecycle { ignore_changes = [tags["RefreshedOn"]] }
}

#--------------------------------------------------------------
#   Terraform States Storage Account
#--------------------------------------------------------------
#   / Backend data Storage with Terraform States
resource "azurerm_storage_account" "this" {
  provider = azurerm.azint

  name                     = lower(substr(replace("st${var.base_name}${var.suffix}", "-", ""), 0, 24))
  location                 = azurerm_resource_group.this.location
  resource_group_name      = azurerm_resource_group.this.name
  account_kind             = "StorageV2"
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = local.base_tags
  #lifecycle { ignore_changes = [tags["RefreshedOn"]] }
}

#   / Terraform States container in Main Location
resource "azurerm_storage_container" "this" {
  provider = azurerm.azint

  for_each = toset(var.containers)

  name                  = lower(substr(each.key, 0, 64))
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
# Get our client public IP
data "http" "icanhazip" {
  url = "http://icanhazip.com"
}
# Lock the Storage Account networking
resource "azurerm_storage_account_network_rules" "this" {
  provider = azurerm.azint

  # Prevents locking the Storage Account before all resources are created
  depends_on = [
    azurerm_storage_account.this,
    azurerm_storage_container.this
  ]

  storage_account_id         = azurerm_storage_account.this.id
  default_action             = "Deny"
  ip_rules                   = ["${chomp(data.http.icanhazip.response_body)}"]
  virtual_network_subnet_ids = []
  bypass                     = ["AzureServices"]
}
