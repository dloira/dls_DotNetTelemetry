#!/bin/bash

# wait for database to start...
for i in {30..0}; do
  if /opt/mssql-tools/bin/sqlcmd -U SA -P ${SA_PASSWORD} -Q 'SELECT 1;' &> /dev/null; then
    echo "$0: SQL Server started"
    break
  fi
  echo "$0: SQL Server startup in progress..."
  sleep 1
done

echo "$0: Initializing database by parameter 2"
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -d master -i database-create.sql
echo "$0: Inserting in database by parameter 2"
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${SA_PASSWORD} -d master -i database-populate.sql
echo "$0: SQL Server Database ready"
