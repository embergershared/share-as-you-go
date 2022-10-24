terraform {
  required_version = ">= 1.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
}
provider "azurerm" {
  tenant_id       = "" # "Tenant"
  subscription_id = "" # "Demo subscription"
  client_id       = "" # "[]-terraform"
  client_secret   = var.client_secret

  features {}
}

# Variables
variable "rg_name" {
  type        = string
  description = "Name of the Resource Group"
  default     = "default-name"
}
variable "client_secret" {
  type = any
}

# Resources
resource "azurerm_resource_group" "this" {
  # name = var.rg_name
  name     = "rg-${var.rg_name}"
  location = "eastus2"

  tags = local.base_tags
}

# Locals
/*
locals {
  # Dates formatted
  UTC_to_TZ = "-5h"
  TZ_suffix = "EST"
  now       = timestamp() # in UTC

  # Timezone based
  TZtime = timeadd(local.now, local.UTC_to_TZ)
  nowTZ  = "${formatdate("YYYY-MM-DD hh:mm", local.TZtime)} ${local.TZ_suffix}" # 2020-06-16 14:44 EST

  # Tags values
  base_tags = tomap({
    "BuiltBy"     = "Terraform 1.3.0",
    "InitiatedBy" = "Emmanuel",
    "BuiltOn"     = "${local.nowTZ}",
  })
}
#*/
