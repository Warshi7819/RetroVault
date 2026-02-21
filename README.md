LOGO Centered

# Overview
WHY
* Stop worrying if you have one
* Easily lookup what you have for a system
* Do I have actual analog joysticks for my Dragon32? Yes, I do!
 
IMAGES From PHONE, WEB and Windows Application. 


## 3in1 - The Kinder egg Of Software
RetroVault consists of three seperate programs that work in perfect harmony to fulfill your cataloging needs.

> [!NOTE]
> During development you can of course configure all three projects to start up in Visual Studio to test it locally. 

### RetroVaultAPI - Backend API
I decided to create an REST API to handle the items I want to catalog. It supports the following:
* **Search For Items**
  * Accepts name, category and system as input, returns items that match
  * Supports pagination    
* **Update Item**
* **Create Item**
* **Upload Thumbnail**
* **Download Thumbnail**
  * Basically handled by the built-in web server and the fact that the thumbnails are stored in a folder that is served out (use static files).
* **Get All Items**
  * Don't really use this one, so maybe up for deletion...
* **Get Item By Id**
  * Don't really use this one either... 

To be able to use the client you first need to setup and launch the RetroVaultAPI. I'm running mine on Linux (Ubuntu), hosted on an LXC in Proxmox. As the web server I use the default one (Kestrel) that is included in the dotnet framework (configured to use HTTPS).  

As storage the backend uses SQLite. Nothing fancy there, but easy enough to change for a more powerfull solution later if the need arises. The backend also enables you to store and retrieve one thumbnail image per item. These images are stored on disk in a directory named thumbnails. 

> [!NOTE]
> The **RetroVault.Shared** project contains code that is shared by the three programs mentioned here. Especially it contains RetroVault.Shared.VaultApiClient.cs that makes it easier to use the REST API from C#.  
 

### Client Side - Windows Forms



### Sever Side - Web APP (Optional, but is it really?)
It IS optional BUT it enables you to see your collection while on the road. Which when you pass a certain number of games is essential to keep your stack of duplicates under control. 

The Web App is implemented in dotnet using Razor pages. I run it on the same LXC Linux server as the API. In this way I only need to expose the Web App publically as all communication with the RetroVaultAPI is done on the server and rendered before the resulting HTML is shipped to the awaiting customer (me). There is no user management implemented as there is only one username and password supported which is hard coded on the server and needs to be set before the Web App is started. It's not possible to host multiple collections for multiple people using this project. But it's pretty easy to add if you have the need (and at least some skill). 

##
> [!CAUTION]
> Code may unexpectedly explode at any time, possibly unscrewing your hard drive and throwing it out the window. 
> Use at your own risk!	
