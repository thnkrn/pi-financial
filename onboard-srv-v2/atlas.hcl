data "external_schema" "gorm" {
  program = [
    "go",
    "run",
    "-mod=mod",
    "ariga.io/atlas-provider-gorm",
    "load",
    "--path", "./internal/core/domain",
    "--dialect", "mysql", // | postgres | sqlite | sqlserver
  ]
}

env "local" {
  src = data.external_schema.gorm.url
  dev = "docker://mysql/8/dev"
  url = "mysql://onboard_service_user:password@localhost:3304/onboard_db"
  migration {
    dir = "file://migrations"
  }
  format {
    migrate {
      diff = "{{ sql . \"  \" }}"
    }
  }
}
