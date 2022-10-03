#
# Copyright (c) 2022 Emmanuel Bergerat
#

#--------------------------------------------------------------
#   Locals
#--------------------------------------------------------------
resource "time_static" "this" {} # Allows to store the creation time in the state

locals {
  # Dates formatted
  UTC_to_TZ   = "-4h"
  TZ_suffix   = "EST"
  now         = timestamp()              # in UTC
  created_now = time_static.this.rfc3339 # is UTC

  # Timezone based DateTime
  created_TZtime = timeadd(local.created_now, local.UTC_to_TZ)
  created_TZ     = "${formatdate("YYYY-MM-DD hh:mm", local.created_TZtime)} ${local.TZ_suffix}" # 2022-06-16 14:44 EST
  now_TZtime     = timeadd(local.now, local.UTC_to_TZ)
  now_TZ         = "${formatdate("YYYY-MM-DD hh:mm", local.now_TZtime)} ${local.TZ_suffix}" # 2022-06-16 14:44 EST

  # Tags values
  base_tags = tomap({
    "BuiltWith"   = var.built_with,
    "BuiltBy"     = var.built_by,
    "BuiltOn"     = "${local.created_TZ}",
    "RefreshedOn" = "${local.now_TZ}",
  })
}
