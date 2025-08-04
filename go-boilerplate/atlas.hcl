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
  url = "mysql://go-boilerplate-user:password@localhost:3306/go-boilerplate-db"
  migration {
    dir = "file://migrations"
  }
  format {
    migrate {
      diff = "{{ sql . \"  \" }}"
    }
  }
}
