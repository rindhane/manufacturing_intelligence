## Refactoring : 

1. DB connectivity string inputs through System.Configuration
1. in webservice, xmlwriter to have utf-8 output instead of utf-16  


## Features : 
1. Add logging 
    1. Especially for try {} cases
1. log the payload sent in JsonToken
1. change the location tag dynamic from environment
1. option from serverConfig to activate https
2. option to provider developer certficate for https

## Tests : 
1. testMain of DbConnector to be tested from the command line in the production built phase 