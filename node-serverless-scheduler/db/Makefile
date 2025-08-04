add-report:
	npx sequelize-cli --options-path ./.sequelize-reports migration:generate --name $(name)
add-reconcile:
	npx sequelize-cli --options-path ./.sequelize-reconcile migration:generate --name $(name)
add-portfolio-summary:
	npx sequelize-cli --options-path ./.sequelize-portfolio-summary migration:generate --name $(name)
add-bond:
	npx sequelize-cli --options-path ./.sequelize-bond migration:generate --name $(name)
migrate-report:
	npx sequelize-cli --options-path ./.sequelize-reports db:migrate
migrate-reconcile:
	npx sequelize-cli --options-path ./.sequelize-reconcile db:migrate
migrate-portfolio-summary:
	npx sequelize-cli --options-path ./.sequelize-portfolio-summary db:migrate
migrate-bond:
	npx sequelize-cli --options-path ./.sequelize-bond db:migrate
migrate:
	npx sequelize-cli --options-path ./.sequelize-reports db:migrate
	npx sequelize-cli --options-path ./.sequelize-reconcile db:migrate
	npx sequelize-cli --options-path ./.sequelize-portfolio-summary db:migrate
    npx sequelize-cli --options-path ./.sequelize-bond db:migrate
undo-report:
	npx sequelize-cli --options-path ./.sequelize-reports db:migrate:undo
undo-reconcile:
	npx sequelize-cli --options-path ./.sequelize-reconcile db:migrate:undo
undo-portfolio-summary:
	npx sequelize-cli --options-path ./.sequelize-portfolio-summary db:migrate:undo
undo-bond:
	npx sequelize-cli --options-path ./.sequelize-bond db:migrate:undo
scripts:
	./migration-script.sh $(db)
