#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
# - Terraform Variables' values
#--------------------------------------------------------------
name_base = "ussc-azint-lz-platform"

address_space = ["10.3.0.0/16"]
subnets = {
  "platform" = {
    address_prefixes  = ["10.3.1.0/24"]
    pe_enable         = false
    service_endpoints = null # ["Microsoft.Sql", "Microsoft.ServiceBus", "Microsoft.Web"]
    delegation        = []
  },
  "loadblancer" = {
    address_prefixes  = ["10.3.2.0/24"]
    pe_enable         = false
    service_endpoints = null
    delegation        = []
  },
  "pe" = {
    address_prefixes  = ["10.3.3.0/24"]
    pe_enable         = true
    service_endpoints = null
    delegation        = []
  }
}
