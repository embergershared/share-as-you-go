#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
# - Resource Group
#--------------------------------------------------------------
#   / Resource Group
resource "azurerm_resource_group" "this" {
  provider = azurerm.azint
  name     = "rg-ussc-azint-${var.name_part1}-${var.name_part2}"
  location = "South Central US"
  tags     = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}

#--------------------------------------------------------------
# - Additional resources
#--------------------------------------------------------------
#   / Resource 1



#   / Resource 2
