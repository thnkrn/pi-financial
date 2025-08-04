data "external_schema" "gorm" {
  program = [
    "go",
    "run",
    "-mod=mod",
    "ariga.io/atlas-provider-gorm",
    "load",
    "--path", "./internal/domain/ssodb",
    "--dialect", "mysql", // | postgres | sqlite | sqlserver
  ]
}

env "local" {
  src = data.external_schema.gorm.url
  dev = "docker://mysql/8/dev"
  url = "mysql://root:P@ssword@localhost:3306/sso_db"
  migration {
    dir = "file://migrations"
  }
  format {
    migrate {
      diff = "{{ sql . \"  \" }}"
    }
  }
}
