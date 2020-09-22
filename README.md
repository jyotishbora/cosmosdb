# IasCosmosDBLibrary
IasCosmosDBLibrary provides a generic interface for adding and retrieving documents to Azure Cosmos DB
```csharp
//CosmosClient instance should be singleton
var cosmosClient = new CosmosClient(endpointuri, primaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDbApi" });

var containerFactory= new ContainerFactory(cosmosClient);

var cosmosService= new CosmosService(containerFactory, dbId,containerId);
```
Below are two methods offered by cosmosService
```csharp
Task<ItemResponse<T>> AddItemAsync<T>(T item, string partitionKey);

/*
Predicate can be used to provide a linq expression to filter documents, which will be executed on the server
maxRecords indicated the maximum number of records to fetch in one batch
If there are more records to fetch, the API will provide a continuation token, which can be passed in the next call to bring the next batch. If there are no more records to fetch, continuaiton token will be null
 */
Task<(IEnumerable<T> Results, string ContinuationToken)> RetrieveItemsAsync<T>(Expression<Func<T, bool>> predicate, int maxRecords, string partitionKey, string continuationToken);
```


# Ias.CosmosDB API

Ias.CosmosDB API provides an API endpoint to add or retrieve documents to Azure cosmos DB.  This API uses IasCosmosDBLibrary behind the scene. Currently the API is specifically tailored to add and retrieve Audit documents, but in future more APIs can be added.

### Samples
#### Add a document
```json
curl --location --request POST 'https://localhost:5001/api/auditlogging' \

--header 'Content-Type: application/json' \

--data-raw '{
	"id": "cf55a8db-c591-4600-bb5f-e7b1179caeb4",
	"EntityId": 14,
	"EntityName": "MasterDealer",
	"AuditKey": "MasterDealer-14",
	"ApplicationName": "Insight - MasterDealer Module",
	"UserId": 464,
	"EventName": null,
	"EventType": 2,
	"AuditText": "MD with id 14 has been mutated",
	"TimeStamp": "2020-08-11T13:17:33.0556928-04:00",
	"Target": {
		"Name": "MasterDealer",
		"FullyQualifiedName": "MasterDealer",
		"InitialState": {
			"Address": {
				"City": "East Lance",
				"Country": "US",
				"Street": "Ervin Mountains",
				"Zip": "28104"
				},
			"ContractId": 2053483333,
			"Email": "Seamus_White54@yahoo.com",
			"PhoneNumber": "1-713-544-2792 x5625",
			"MasterDealerName": "Morar, Keebler and Collins",
			"MasterDealerId": 14
			},
		"FinalState": {
			"Address": {
				"City": "East Lance",
				"Country": "US",
				"Street": "Ervin Mountains",
				"Zip": "28104"
				},
			"ContractId": 2053483333,
			"Email": "Nola.Bayer41@yahoo.com",
			"PhoneNumber": "(483) 669-8146 x40560",
			"MasterDealerName": "Morar, Keebler and Collins",
			"MasterDealerId": 14
		}
	}
}'
```
####  Get Documents
```
Without Contunation Token:
curl -X GET "https://localhost:5001/api/AuditLogging/documents/MasterDealer14?maxCount=10" -H "accept: text/plain"

With Continuation Token:
curl -X GET "https://localhost:5001/api/AuditLogging/documents/MasterDealer14?maxCount=10&continuationToken=%5B%7B%22token%22%3A%22%2BRID%3A~Mj4uAJMPlQHchR4AAAAAAA%3D%3D%23RT%3A1%23TRC%3A10%23ISV%3A2%23IEO%3A65551%23FPC%3AAgF6enoMANyFBMAwgAQAAACQAw%3D%3D%22%2C%22range%22%3A%7B%22min%22%3A%22%22%2C%22max%22%3A%22FF%22%7D%7D%5D" -H "accept: text/plain"


Response:
{
    "auditEvents": [
        {
            "id": "72f69e9c-594d-4eeb-aa4b-af11c1e7a488",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 261,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:51:00.0341332-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"East Margotville\",\"Country\":\"US\",\"Street\":\"Dibbert Mountain\",\"Zip\":\"33464\"},\"ContractId\":-1800585611,\"Email\":\"Brooklyn.Will@yahoo.com\",\"PhoneNumber\":\"1-733-222-1932 x3093\",\"MasterDealerName\":\"Abshire Group\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"East Margotville\",\"Country\":\"US\",\"Street\":\"Dibbert Mountain\",\"Zip\":\"33464\"},\"ContractId\":-1800585611,\"Email\":\"Aglae_Kuhic@yahoo.com\",\"PhoneNumber\":\"413.718.6658 x549\",\"MasterDealerName\":\"Abshire Group\",\"MasterDealerId\":14}"
            }
        },
        {
            "id": "9375ddc6-bd53-4838-b525-790a18ef8e79",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 121,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:51:00.8132464-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"Port Opalland\",\"Country\":\"US\",\"Street\":\"June Wells\",\"Zip\":\"78440\"},\"ContractId\":-425983433,\"Email\":\"Sandrine.Heller@gmail.com\",\"PhoneNumber\":\"(344) 649-0794 x4672\",\"MasterDealerName\":\"Senger, Hermiston and Rohan\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"Port Opalland\",\"Country\":\"US\",\"Street\":\"June Wells\",\"Zip\":\"78440\"},\"ContractId\":-425983433,\"Email\":\"Maybell_MacGyver41@hotmail.com\",\"PhoneNumber\":\"1-459-928-1566 x474\",\"MasterDealerName\":\"Senger, Hermiston and Rohan\",\"MasterDealerId\":14}"
            }
        },
        {
            "id": "46c3c2dd-1fcf-4c4c-b6a4-681ef3982e09",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 919,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:51:28.4403191-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"North Marcelluston\",\"Country\":\"US\",\"Street\":\"Hamill Islands\",\"Zip\":\"68000\"},\"ContractId\":-1313472504,\"Email\":\"Harmony_Donnelly55@gmail.com\",\"PhoneNumber\":\"988.507.2002 x746\",\"MasterDealerName\":\"Mitchell - Witting\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"North Marcelluston\",\"Country\":\"US\",\"Street\":\"Hamill Islands\",\"Zip\":\"68000\"},\"ContractId\":-1313472504,\"Email\":\"Elissa10@hotmail.com\",\"PhoneNumber\":\"1-669-581-5338\",\"MasterDealerName\":\"Mitchell - Witting\",\"MasterDealerId\":14}"
            }
        },
        {
            "id": "f3284e0c-baed-4ab7-b196-49cbf5f31651",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 817,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:51:29.4252176-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"Hilperttown\",\"Country\":\"US\",\"Street\":\"Harvey Pines\",\"Zip\":\"81522\"},\"ContractId\":-1440606066,\"Email\":\"Angelita_Champlin12@yahoo.com\",\"PhoneNumber\":\"513.529.0429 x9685\",\"MasterDealerName\":\"Runolfsson Inc\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"Hilperttown\",\"Country\":\"US\",\"Street\":\"Harvey Pines\",\"Zip\":\"81522\"},\"ContractId\":-1440606066,\"Email\":\"Joe_Breitenberg@hotmail.com\",\"PhoneNumber\":\"366-507-0537\",\"MasterDealerName\":\"Runolfsson Inc\",\"MasterDealerId\":14}"
            }
        },
        {
            "id": "c6758201-76f0-4f47-83c5-c66da472df2e",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 709,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:51:29.6589484-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"Abbottberg\",\"Country\":\"US\",\"Street\":\"Garnet Trail\",\"Zip\":\"86158-0184\"},\"ContractId\":246335100,\"Email\":\"Carrie.Veum@gmail.com\",\"PhoneNumber\":\"549-451-4093 x61002\",\"MasterDealerName\":\"Bailey - Jones\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"Abbottberg\",\"Country\":\"US\",\"Street\":\"Garnet Trail\",\"Zip\":\"86158-0184\"},\"ContractId\":246335100,\"Email\":\"Mallory.Pagac@hotmail.com\",\"PhoneNumber\":\"(820) 617-7783 x730\",\"MasterDealerName\":\"Bailey - Jones\",\"MasterDealerId\":14}"
            }
        },
        {
            "id": "efb4f3be-fc9e-4b96-87d3-03b6c01a0687",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 544,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:51:46.3865746-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"Jacquesstad\",\"Country\":\"US\",\"Street\":\"Melvin Glens\",\"Zip\":\"98908\"},\"ContractId\":-1223832945,\"Email\":\"Roberto.Schamberger@yahoo.com\",\"PhoneNumber\":\"569-981-4841\",\"MasterDealerName\":\"Metz - Adams\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"Jacquesstad\",\"Country\":\"US\",\"Street\":\"Melvin Glens\",\"Zip\":\"98908\"},\"ContractId\":-1223832945,\"Email\":\"Harmon_Gerlach60@gmail.com\",\"PhoneNumber\":\"783.295.5551 x341\",\"MasterDealerName\":\"Metz - Adams\",\"MasterDealerId\":14}"
            }
        },
        {
            "id": "2d981b70-b8d7-4d54-a023-1ea4c3d4c16e",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 394,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:51:46.8362616-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"Rennerfurt\",\"Country\":\"US\",\"Street\":\"Mayer Trace\",\"Zip\":\"83426-7032\"},\"ContractId\":277071853,\"Email\":\"Liliana_Corkery@hotmail.com\",\"PhoneNumber\":\"(499) 334-0782 x7792\",\"MasterDealerName\":\"Langworth Group\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"Rennerfurt\",\"Country\":\"US\",\"Street\":\"Mayer Trace\",\"Zip\":\"83426-7032\"},\"ContractId\":277071853,\"Email\":\"Teagan56@gmail.com\",\"PhoneNumber\":\"1-815-381-9891 x418\",\"MasterDealerName\":\"Langworth Group\",\"MasterDealerId\":14}"
            }
        },
        {
            "id": "674684e1-4f36-4e1e-8618-d00909b2b8a5",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 846,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:52:03.3653-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"Port Gregory\",\"Country\":\"US\",\"Street\":\"Alisha Passage\",\"Zip\":\"45050\"},\"ContractId\":403560994,\"Email\":\"Lesly.Steuber26@hotmail.com\",\"PhoneNumber\":\"1-424-466-8831\",\"MasterDealerName\":\"Harvey - Powlowski\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"Port Gregory\",\"Country\":\"US\",\"Street\":\"Alisha Passage\",\"Zip\":\"45050\"},\"ContractId\":403560994,\"Email\":\"Jasper5@hotmail.com\",\"PhoneNumber\":\"407.551.5608\",\"MasterDealerName\":\"Harvey - Powlowski\",\"MasterDealerId\":14}"
            }
        },
        {
            "id": "620c3e18-a35d-4027-be24-3495b54cefed",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 626,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:52:04.6538116-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"Jerdeville\",\"Country\":\"US\",\"Street\":\"Huel Oval\",\"Zip\":\"99869\"},\"ContractId\":935838891,\"Email\":\"Armando.Bosco6@yahoo.com\",\"PhoneNumber\":\"876.300.3591\",\"MasterDealerName\":\"Kris Inc\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"Jerdeville\",\"Country\":\"US\",\"Street\":\"Huel Oval\",\"Zip\":\"99869\"},\"ContractId\":935838891,\"Email\":\"Fanny.Swift@hotmail.com\",\"PhoneNumber\":\"871.595.3151 x55593\",\"MasterDealerName\":\"Kris Inc\",\"MasterDealerId\":14}"
            }
        },
        {
            "id": "7a17323e-f242-4657-ab3e-60932caffd3b",
            "entityId": 14,
            "entityName": "MasterDealer",
            "auditKey": "MasterDealer14",
            "applicationName": "Insight - MasterDealer Module",
            "userId": 496,
            "eventName": null,
            "eventType": 2,
            "auditText": "MD with id 14 has been mutated",
            "timeStamp": "2020-08-11T14:52:05.5370364-04:00",
            "target": {
                "name": "MasterDealer",
                "fullyQualifiedName": "MasterDealer",
                "initialState": "{\"Address\":{\"City\":\"South Tyshawnport\",\"Country\":\"US\",\"Street\":\"Monique Mountain\",\"Zip\":\"45468\"},\"ContractId\":1194512820,\"Email\":\"Kiana77@hotmail.com\",\"PhoneNumber\":\"571-917-9565 x1468\",\"MasterDealerName\":\"Labadie - Becker\",\"MasterDealerId\":14}",
                "finalState": "{\"Address\":{\"City\":\"South Tyshawnport\",\"Country\":\"US\",\"Street\":\"Monique Mountain\",\"Zip\":\"45468\"},\"ContractId\":1194512820,\"Email\":\"Dennis.Morissette41@hotmail.com\",\"PhoneNumber\":\"619-657-8519\",\"MasterDealerName\":\"Labadie - Becker\",\"MasterDealerId\":14}"
            }
        }
    ],
    "continuationToken": "[{\"token\":\"+RID:~Mj4uAJMPlQHchR4AAAAAAA==#RT:1#TRC:10#ISV:2#IEO:65551#FPC:AgF6enoMANyFBMAwgAQAAACQAw==\",\"range\":{\"min\":\"\",\"max\":\"FF\"}}]"
}


```
