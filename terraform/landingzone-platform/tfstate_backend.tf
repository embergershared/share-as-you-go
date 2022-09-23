#--------------------------------------------------------------
#   Remote backend for Terraform state files
#--------------------------------------------------------------
# Ref: https://www.terraform.io/language/settings/backends/azurerm

# The authentication method used here can either be:
# - `az login` on the machine running terraform
# - set an environment variable named `ARM_ACCESS_KEY` with the storage account access key as its value


terraform {
  backend "azurerm" {
    resource_group_name  = "rg-ussc-azint-lz-platform"
    storage_account_name = "stusscazintlzplatform"
    container_name       = "tfstates"
    key                  = "tf-landingzone-platform"
  }
}
#*/
