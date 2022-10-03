#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
# - Resource Group
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
# - Additional resources
#--------------------------------------------------------------
#   / Resource 1



#   / Resource 2
