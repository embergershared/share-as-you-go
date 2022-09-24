#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
# - Terraform Variables Declarations
#--------------------------------------------------------------
variable "name_part1" {
  type    = string
  default = "research"
}
variable "name_part2" {
  type    = string
  default = "aadrbac"
}

variable "aad_admin_group_object_ids" {
  type        = list(string)
  description = "(Optional) The list of Azure AD Object IDs to add to the cluster Administrators. Can be both Users and Groups."
  default     = null
}
variable "principal_id" {}

variable "law_id" {
  default = null
}

variable "storage_account_id" {
  default = null
}
