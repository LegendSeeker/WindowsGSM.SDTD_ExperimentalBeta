## 7 Days to Die - Experimental Beta Plugin

I created this plugin simply to have WindowsGSM host/update from experimental beta branch.

## Importing a server
The Server World Data is now moved to servers/%ID%/serverfiles/userdata. 
If you already had a Server running where you want to continue, please move everything from %appdata%\7DaysToDie\ (just put it in explorer, will expand to C:\Users\YourWindowsUser\AppData\Roaming) 
=> to WindowsGSM servers/%ID%/serverfiles/userdata (you can find it by marking the server in wgsm and click Browse => Server Files )

then replace `serverconfig.xml`. just make sure to keep:
<property name="UserDataFolder"		value="userdata" />

## Initial Setup
When you install the server, **run update once to be sure you have the experimental beta branch**

### Known issue
It looks like Windows GSM doesn't specify it is done validating after an update so please wait some time before starting the server. You will receive error `-1073741819` if the update is still in progress.

## Server Configuration
In order to configure  the server, please find the `serverconfig.xml` file located in `\serverfiles\serverconfig.xml`
the server will use https://github.com/WindowsGSM/Game-Server-Configs/blob/master/7%20Days%20to%20Die%20Dedicated%20Server/serverconfig.xml as base

### Notable Fields:

|FieldName|Default  |
|--|--|
|ServerName   | "My Game Host"  |
|ServerPassword |""|
|Region|"NorthAmericaEast"|
|ServerPort|26900|
|ServerVisibility|2 (public)|
|EACEnabled |true|
