#
# Copyright (c) 2022 Emmanuel Bergerat
#


#--------------------------------------------------------------
# - Policy Definition
#--------------------------------------------------------------
#   / Deny Role Assignments policy
#
# - Generate the locals for policy definition
#
locals {
  filepath          = "${path.root}/policy_definitions/${var.policyDefinitionJsonFile}"
  policy_definition = jsondecode(file(local.filepath))
}

# Notes:
# The value of 'field' must be one of: 'Name, Type, Location, Tags, Kind, FullName, identity.type, identity.userAssignedIdentities, id' or an alias, e.g. \"Microsoft.Compute/virtualMachines/imagePublisher\".

# To list the Resource providers:
# az provider list --query [*].namespace

# To get the alias(es) for the Authorization provider:
# az provider show --namespace Microsoft.Authorization --expand "resourceTypes/aliases" --query "resourceTypes[].aliases[].name"

# Ref: https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.management.authorization.models.roleassignment.principaltype?view=azure-dotnet-preview
# Gets or sets the principal type of the assigned principal ID. Possible values include: 'User', 'Group', 'ServicePrincipal', 'Unknown', 'DirectoryRoleTemplate', 'ForeignGroup', 'Application', 'MSI', 'DirectoryObjectOrGroup', 'Everyone'


#
# - Create the Policy Definition resource
#
resource "azurerm_policy_definition" "this" {
  provider = azurerm.azint

  # Required Resource attributes
  name         = coalesce(var.name, local.policy_definition.name)
  display_name = coalesce(var.display_name, local.policy_definition.properties.displayName)
  policy_type  = "Custom" # Possible values: [BuiltIn, Custom, NotSpecified].
  mode         = coalesce(var.mode, local.policy_definition.properties.mode)

  # Optional Resource attributes
  management_group_id = var.management_group_id
  description         = var.description != null ? var.description : try(length(local.policy_definition.properties.description) > 0 ? local.policy_definition.properties.description : null)
  policy_rule         = try(length(local.policy_definition.properties.policyRule) > 0 ? jsonencode(local.policy_definition.properties.policyRule) : null)
  parameters          = try(length(local.policy_definition.properties.parameters) > 0 ? jsonencode(local.policy_definition.properties.parameters) : null)
  metadata            = try(length(local.policy_definition.properties.metadata) > 0 ? jsonencode(local.policy_definition.properties.metadata) : null)
}

#--------------------------------------------------------------
# - Resource Group
#--------------------------------------------------------------
#   / Resource Group
resource "azurerm_resource_group" "this" {
  provider = azurerm.azint

  name     = "rg-ussc-azint-learning-policytesting"
  location = "South Central US"

  tags = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}

#--------------------------------------------------------------
# - Policy Assignment
#--------------------------------------------------------------
#   / Assign the Policy to the Resource Group
resource "azurerm_resource_group_policy_assignment" "this" {
  provider = azurerm.azint

  # Mandatory resource attributes
  name                 = "PA-DenyRoleAssign-On-RGpolicytesting-Except1Principal"
  resource_group_id    = azurerm_resource_group.this.id
  policy_definition_id = azurerm_policy_definition.this.id

  # # Optional resource attributes
  # location     = var.location
  # description  = var.description
  # display_name = var.display_name
  # metadata     = try(length(var.metadata) > 0, false) ? jsonencode(var.metadata) : null
  # parameters   = try(length(var.parameters) > 0, false) ? jsonencode(var.parameters) : null
  parameters = <<PARAMS
      {
        "principalId": {
          "value": "${var.principal_id_spn}"
        }
      }
  PARAMS
  # not_scopes   = var.not_scopes
  # enforce      = var.enforcement_mode
}

#--------------------------------------------------------------
# - Storage Account in the RG
#--------------------------------------------------------------
resource "azurerm_storage_account" "this" {
  provider = azurerm.azint

  name                     = "sttotestradenypolicy"
  resource_group_name      = azurerm_resource_group.this.name
  location                 = azurerm_resource_group.this.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = local.base_tags
  lifecycle { ignore_changes = [tags["BuiltOn"]] }
}


#--------------------------------------------------------------
# - Try Role Assignments creation on the Storage Account
#--------------------------------------------------------------
#   / Passing role assignment
resource "azurerm_role_assignment" "spn" {
  provider = azurerm.azint

  # Required
  principal_id = var.principal_id_spn

  # Rule = id XOR name
  role_definition_id   = null
  role_definition_name = "Contributor"

  scope = azurerm_storage_account.this.id
}

#   / Failing role assignment
resource "azurerm_role_assignment" "user" {
  provider = azurerm.azint

  # Required
  principal_id = var.principal_id_user

  # Rule = id XOR name
  role_definition_id   = null
  role_definition_name = "Contributor"

  scope = azurerm_storage_account.this.id
}
#*/
