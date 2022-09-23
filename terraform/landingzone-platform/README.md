# Landing Zone - Platform

## Objective

This folder creates few basics for a Platform Landing Zone.

It manages:

- 1 Resource Group
- 1 Storage account for logs & terraform states
- 1 Log Analytics Workspace
- 1 Application Insights (non legacy)
- 1 Virtual Network
- 1 Key Vault
- All the Diagnostic Settings

**NOTE:** The terraform state pattern used is the `local` then move to `Azure remote backend` described here: [`terraform/azure-starter`](https://github.com/embergershared/share-as-you-go/tree/main/terraform/azure-starter/1.terraform)
