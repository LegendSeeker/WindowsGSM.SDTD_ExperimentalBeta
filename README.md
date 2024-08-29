## 7 Days to Die - Experimental Beta Plugin

I created this plugin simply to have WindowsGSM host/update from experimental beta branch.

## Importing a server
The Server World Data is now moved to servers/%ID%/serverfiles/userdata. 
If you already had a Server running where you want to continue, please move everything from %appdata%\7DaysToDie\ (just put it in explorer, will expand to C:\Users\YourWindowsUser\AppData\Roaming) 
=> to WindowsGSM servers/%ID%/serverfiles/userdata (you can find it by marking the server in wgsm and click Browse => Server Files )

then replace the wgsm `serverconfig.xml` with your existing one . just make sure to keep/add:
<property name="UserDataFolder"		value="userdata" />

## Initial Setup
When you install the server, **run update once to be sure you have the experimental beta branch**

### Known issue
- It looks like Windows GSM doesn't specify it is done validating after an update so please wait some time before starting the server. You will receive error `-1073741819` if the update is still in progress.
- if your connection to the server fails with Server is still initializing after your gave it 15 min to start(frist start takes a while and should not be interrupted, it can break your save
  - look at the console(toggle console, could be that you need to toggle it three times) or
  - last log (Browse => Server Files => 7DaysToDieServer_Data => the last "output_log_dedi__*.txt")
  - check if it contains "SSL certificate problem: unable to get local issuer certificate". if yes:
    - https://community.7daystodie.com/topic/32449-a21-windows-dedicated-server-eos-error-fix/ 

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
