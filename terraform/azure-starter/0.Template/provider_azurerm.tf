#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
#   Provider to access Azure
#--------------------------------------------------------------
variable "tenant_id" {}
variable "subscription_id" {}
variable "client_id" {}
variable "client_secret" {}

provider "azurerm" {
  # Reference: https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs#argument-reference
  alias       = "azint" # Comment/Remove this alias as it may not be needed
  environment = "public"
  # partner_id  = ""

  tenant_id       = var.tenant_id
  subscription_id = var.subscription_id
  client_id       = var.client_id
  client_secret   = var.client_secret

  # storage_use_azuread = true # If needed

  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
    key_vault {
      purge_soft_delete_on_destroy = true
    }
    template_deployment {
      delete_nested_items_during_deletion = true
    }
    virtual_machine {
      delete_os_disk_on_deletion     = true
      graceful_shutdown              = true
      skip_shutdown_and_force_delete = false
    }
    virtual_machine_scale_set {
      force_delete                  = true
      roll_instances_when_required  = false
      scale_to_zero_before_deletion = true
    }
  }
}
