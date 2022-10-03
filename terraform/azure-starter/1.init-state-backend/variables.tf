#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
# - Terraform Variables Declarations
#--------------------------------------------------------------
variable "built_with" {
  type        = string
  description = "Terraform version used to build these resources"
  default     = "Terraform v1.3.0"
}
variable "built_by" { default = "Emmanuel" }
variable "location" { default = "South Central US" }
variable "codes" { default = "ussc-azint" }
variable "base_name" { default = "sayg" }
variable "suffix" { default = "tfstates" }
variable "containers" { default = ["github-embergershared-shareasyougo"] }
