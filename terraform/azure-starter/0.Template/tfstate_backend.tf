#--------------------------------------------------------------
#   Remote backend for Terraform state files
#--------------------------------------------------------------
# Ref: https://www.terraform.io/language/settings/backends/azurerm

# The authentication method used here can EITHER be:
# - `az login` on the machine running terraform, OR
# - set an environment variable named `ARM_ACCESS_KEY` with the storage account access key as its value


terraform {
  backend "azurerm" {
    resource_group_name  = "rg-ussc-azint-notepad-tfstates"
    storage_account_name = "stnotepadtfstates"
    container_name       = "github-embergershared-notepad"
    key                  = # "azure_0-template"
  }
}
#*/
