# Show version
terraform version
# Show update message

# Show Service Principal used in Portal:
#   - Service Principal references
#   - Subscription IAM (with SPN being Owner on Subscription)


# Show init
terraform init
# Show .terraform folder

# Show main.tf
# Show auto.tfvars

# Launch a plan
terraform plan
# Show Plan result

# Launch an Apply
terraform apply
# Explain why the var.rg_name value is requested
# enter "yes" / Explain --auto-approve / or plan input

# show result in portal
# show terraform.state

# comment default for var, name in rg,
# uncomment locals, name & tags in rg,
# Launch an Apply
terraform apply
# enter a RG name
# show result as "noname" is "replaced" : Plan: 1 to add, 0 to change, 1 to destroy.
# enter "yes"
# show resutl in portal

# uncomment value in auto.tfvars
terraform apply
# show result as "typed name" is "replaced" : Plan: 1 to add, 0 to change, 1 to destroy.
# enter "yes"
# show resutl in portal

# launch a destroy plan
terraform destroy --auto-approve
# Show how to delete resources