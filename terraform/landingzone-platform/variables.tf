#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
# - Terraform Variables Declarations
#--------------------------------------------------------------
variable "name_base" {}
variable "network_ip_rules" { type = list(string) }
