# Zendesk Json Search Project

This project is to search a mocked database using json files by available property name and value.

## Design Thinking

As a platform product, extensibility is very important. 
Although there are only two json files (user and ticket) provided, an ideal solution needs to be able to handle any json files with same format. So instead of creating specific class for user and ticket, I created a base entity "Entity" to handle any entity json files (user, ticket, or any other entity json files)
In order to handle dynamic data for different entities, I manually created a json file called "metadata.json". This file inclcudes all metadata for entities and relationships.

Here is definition of metadata.json:
```json
{
  "entities": [
    {
      "filename": "This is physical file name",
      "displayname": "This is the display name of the entity",
      "entityname": "This is the real entity name. It must be unique",
      "entitycode": "This is the entity uniqe code. This is used to mapping entity metdata when structure gets more complicated. (e.x. we introduce enumerations and save as different table)",
      "nameproperty": "This is the property name which will be saved in name field. Name field is used to be displayed when it is used as lookup.",
      "properties": [
        "This include all properties that entity contains"        
      ],
      "indexingproperties": [
        "This include all properties that requires indexing to be created" 
      ]
    }
  ],
  "relationships": [
    {
      "fromentity": "This is the related from entity name (in provided case it is user entity)",
      "fromproperty": "This is always the id (primary key) column (in provided case it is user entity _id column)",
      "toentity": "This is the related to entity name (in provided case it is ticket entity)",
      "toproperty": "This is the foreigh key property name (in provided case it is ticket entity assignee_id column)"
    }
  ]
}
```
In real production environment the metadata should be saved in database as well.
By this design who project is generic. As long as json files are in correct format, the app can handle without any code change.

## Extension Option

As the app is fully extendable, it can allow user to do redevelopment. User is able to create new entities and relationships they want to have.
In Zendesk.JsonSearch.Models project, there is a EarlyBound folder including User and Ticket classes. These classes should be generated by an automation tool provided to user, so user can generate specific classes which can help them make redevelopment process easier.


## Installation

### Running using Docker

1. Install docker from https://docs.docker.com/get-docker/
2. Navigate to the root project folder in command line window (bash, cmd, etc)

To run app,
3. Type "Docker build -t kevin.zendesk.jsonsearch ." (image name can be changed)
4. Type "Docker run -ti kevin-zendesk-jsonsearch"

To run tests,
5. Type "Docker build --target testrunner -t kevin.zendesk.jsonsearch.test ."
6. Type "Docker run -ti kevin.zendesk.jsonsearch.test"

### Open source code

Source code can be opened with any text editor. But if local run is required, .net core sdk is required to be installed.
1. Go to https://dotnet.microsoft.com/en-us/download and select target OS
2. Download .Net 6.0 SDK and install

#### IDE

##### VS Code
1. Download and install VS Code from https://code.visualstudio.com/download
2. Open VS Code, to to Extensions, search c# in marketplace then install, or install from https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp 
