#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
# - Resource Group
#--------------------------------------------------------------
#   / Resource Group
resource "azurerm_resource_group" "this" {
  provider = azurerm.azint
  name     = "rg-ussc-azint-learning-appsvc"
  location = "South Central US"
  tags     = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}

#--------------------------------------------------------------
# - Application Service
#--------------------------------------------------------------
#   / App Service Plan (= Hosting plan)
resource "azurerm_service_plan" "this" {
  provider            = azurerm.azint
  name                = "plan-ussc-azint-learning-windows"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  sku_name            = "F1"
  os_type             = "Windows"

  tags = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}

#   / App Service - Windows
resource "azurerm_windows_web_app" "this" {
  provider            = azurerm.azint
  name                = "app-webapi-minimal"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_service_plan.this.location
  service_plan_id     = azurerm_service_plan.this.id

  site_config {
    always_on = false
  }

  app_settings = {
    ASPNETCORE_ENVIRONMENT = "Development"
  }
}
