<image src="RetroVault/images/TransparentSplash.png" height="150px"/>

# Overview
I have too much retro stuff! Old games, consoles etc. So, I decided to create a RetroVault to get organized. Once organized I can buy even more stuff because, you know, it’s organized. Mkey? 

Setting out I had three goals:
* Backend storage with an API to store, manipulate and retrieve retro items.
* A client application to catalog, list and update retro items.
* A mobile friendly frontend so that I can carry my retro items digitally with me – at all times!
  * Also known as **Duplicate Prevention**. "Do I have an analog joystick for my Dragon 32? Yes, I have!"

I’m currently running RetroVault on my own server and I have cataloged more than 400 items so I guess it’s reached the somewhat “stable” phase already. 

IMAGES From PHONE, WEB and Windows Application.

## 3in1 - The Kinder Egg Of Software
RetroVault consists of three seperate programs that work in perfect harmony to fulfill your cataloging needs. In this section we will go through all three in a bit more detail which might make it easier to navigate the code itself.

> [!NOTE]
> During development you can of course configure all three projects to start up in Visual Studio to test it locally. 

### RetroVaultAPI - Backend API
This is the backend storage with an API to manipulate your retro items. The REST API supports the following:
* **Search For Items**
  * Accepts name, category and system as input, returns items that match
  * Supports pagination    
* **Update Item**
* **Create Item**
* **Upload Thumbnail**
* **Download Thumbnail**
  * Basically handled by the built-in web server and the fact that the thumbnails are stored in a folder that is served out (use static files).
* **Get Item By Id**
* **Get All Items**
  * Don't really use this one in the final product so probably should remove this one… Unless I want to drag out all data and make some reports or something… ok, I’ll leave it in until I decide. 

To be able to use the client you first need to setup and launch the RetroVaultAPI – your backend. I'm running mine on Linux (Ubuntu), hosted on an LXC in Proxmox. As the web server I use the default one (Kestrel) that is included in the dotnet framework (configured to use HTTPS). 

As storage the backend uses SQLite. Nothing fancy there, but easy enough to change for a more powerful solution later if the need arises. The backend also enables you to store and retrieve one thumbnail image per item. These images are stored on disk in a directory named thumbnails. Only used for making search results more interesting. Storing real data about your items is discussed below when we look at the client app. 

> [!NOTE]
> The **RetroVault.Shared** project contains code that is shared by the three programs mentioned here. Especially it contains RetroVault.Shared.VaultApiClient.cs that makes it easier to use the REST API from C#.  
 
### RetroVault - Client Side Application
This is a Windows Forms application. “And why would you be so…” Well, it’s super fast for me to create as I have created one too many over the years. And if it works, it works, right? Oh, look at that custom splash screen on boot!

The client app lets you search your collection – look at and update details about specific items. And create new ones. 

In addition to taking advantage of the backend API to store, search and retrieve items it also enables you to store **MORE DATA** about your items. Like documents, more images, music, maybe even a full dump of your physical media (in case that is allowed in your part of the world). This data will be stored in a configured file path be it on local disk or your super duper NAS over Samba. If you can reach your folder then you can configure it for use. Data for a specific item with a given {id} will be stored under the configured folder/{id}.

The data is only available through this client or by directly browsing the disk.

### RetroVaultWebApp - The Web APP (Optional, but is it really?)
It IS optional BUT it enables you to see your collection while on the road. Which when you pass a certain number of games is essential to keep your stack of duplicates under control. 

The Web App is implemented in dotnet using Razor pages. I run it on the same LXC Linux server as the API. In this way I only need to expose the Web App publically as all communication with the RetroVaultAPI is done on the same server and rendered before the resulting HTML is shipped to the awaiting customer (me). The Web App is exposed to the scary internet through a Cloudflare tunnel. And no, I won't detail how that is done because it really isn't my expertise.  

There is no user management implemented as there is only one username and password supported which is hard coded on the server and needs to be set before the Web App is started. Or you could just use the default which is "admin" and "pass" if you don't care. 

Since there is no user management implemented there's no possibility to host multiple collections for multiple people either. But it should be pretty easy to add if you have the need (and at least some skill). 

### Outro
That’s it. If you're still left with more burning questions then have a look at the code.  

##
> [!CAUTION]
> Code may unexpectedly explode at any time, possibly unscrewing your hard drive and throwing it out the window. 
> Use at your own risk!	
