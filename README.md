## 7 Days to Die - Experimental Beta Plugin

I created this plugin simply to have WindowsGSM host/update from experimental beta branch.

## Initial Setup
When you install the server, run update once to be sure you have the experimental beta branch

### Known issue
It looks like Windows GSM doesn't specify it is done validating after an update so please wait some time before starting the server. You will receive error `-1073741819` if the update is still in progress.

## Server Configuration
In order to configure  the server, please find the `serverconfig.xml` file located in `\serverfiles\serverconfig.xml`

### Notable Fields:

|FieldName|Default  |
|--|--|
|ServerName   | "My Game Host"  |
|ServerPassword |""|
|Region|"NorthAmericaEast"|
|ServerPort|26900|
|ServerVisibility|2 (public)|
|EACEnabled |true|
