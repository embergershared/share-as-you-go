#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
#   Terraform States Resource Group
#--------------------------------------------------------------
#   / Resource Group
resource "azurerm_resource_group" "this" {
  provider = azurerm.azint
  name     = "rg-ussc-azint-notepad-tfstates"
  location = "South Central US"
  tags     = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}

#--------------------------------------------------------------
#   Terraform States Storage Account
#--------------------------------------------------------------
#   / Backend data Storage with Terraform States
resource "azurerm_storage_account" "this" {
  provider                 = azurerm.azint
  name                     = "stnotepadtfstates"
  location                 = azurerm_resource_group.this.location
  resource_group_name      = azurerm_resource_group.this.name
  account_kind             = "StorageV2"
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}

#   / Terraform States container in Main Location
resource "azurerm_storage_container" "this" {
  provider = azurerm.azint
  for_each = ["github-emberger-notepad"]

  name                  = lower(each.key)
  storage_account_name  = azurerm_storage_account.this.name
  container_access_type = "private"
}
