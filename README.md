oauth-vk
========

The tool for recieving oauth token from vk.com

Dependency:
___________
	mono
	webkit
	gtk-sharp
	webkit-sharp



Info:
_____

The oauth.exe will open window with login page and after access confirmation will return json with tokens to console.



Parameters:
___________


APP_ID -- vk Application ID.

SCOPE  -- Requested permissions( The list of supported permissions listed http://vk.com/dev/permissions).

display --  Authorization window appearance, the following options are supported: page, popup, touch and wap (popup default).  

api_veresion -- vk  API version(5.5 default).

size -- window size (460,320 default)



Usage:

	oauth.exe <APP_ID> <SCOPE> [display=<DISPLAY> size=<WIDTH, HEIGHT> api_veresion=<API VERSION>]
