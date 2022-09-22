#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
# - Required Terraform providers & versions
#--------------------------------------------------------------
terraform {
  required_version = ">= 1.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
}
