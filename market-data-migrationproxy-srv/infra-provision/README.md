# Database setup

## Insert data to MongoDB
```bash
mongosh --port 27018 -u root -p password --authenticationDatabase admin insert_curated_filter.js
mongosh --port 27018 -u root -p password --authenticationDatabase admin insert_curated_list.js
mongosh --port 27018 -u root -p password --authenticationDatabase admin insert_curated_member.js
mongosh --port 27018 -u root -p password --authenticationDatabase admin insert_instrument.js
```