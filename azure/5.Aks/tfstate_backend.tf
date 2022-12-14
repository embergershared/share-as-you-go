#--------------------------------------------------------------
#   Remote backend for Terraform state files
#--------------------------------------------------------------
# Ref: https://www.terraform.io/language/settings/backends/azurerm

# The authentication method used here can either be:
# - `az login` on the machine running terraform
# - set an environment variable named `ARM_ACCESS_KEY` with the storage account access key as its value


terraform {
  backend "azurerm" {
    resource_group_name  = "rg-ussc-azint-learning-tfstates"
    storage_account_name = "stusscazinttfstates"
    container_name       = "tfsates-learn-apim"
    key                  = "azure_5-aks"
  }
}
#*/
