#--------------------------------------------------------------
#   Remote backend for Terraform state files
#--------------------------------------------------------------
# Ref: https://www.terraform.io/language/settings/backends/azurerm

# The authentication method used here can either be:
# - `az login` on the machine running terraform
# - set an environment variable named `ARM_ACCESS_KEY` with the storage account access key as its value

/*
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-ussc-azint-sayg-tfstates"
    storage_account_name = "stsaygtfstates"
    container_name       = "github-embergershared-shareasyougo"
    key                  = "azure-starter_1-init-state-backend"
  }
}
#*/
