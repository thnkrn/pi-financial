data "external_schema" "gorm" {
  program = [
    "go",
    "run",
    "-mod=mod",
    "ariga.io/atlas-provider-gorm",
    "load",
    "--path", "./internal/domain",
    "--dialect", "mysql", // | postgres | sqlite | sqlserver
  ]
}

env "local" {
  src = data.external_schema.gorm.url
  dev = "docker://mysql/8/dev"
  url = "mysql://user-srv-v2-user:password@localhost:3306/user-srv-v2-db"
  migration {
    dir = "file://migrations"
  }
  format {
    migrate {
      diff = "{{ sql . \"  \" }}"
    }
  }
  // Filter specific schemas/tables for migration
  exclude = [
    "addresses",
    "user_infos",
    "devices",
    "bank_account_v2s",
    "external_accounts",
    "suitability_tests",
    "trade_accounts",
    "user_accounts",
    "documents",
    "notification_preferences",
    "kycs",
  ]
}
