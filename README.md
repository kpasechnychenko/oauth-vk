oauth-vk
========

The tool for recieving oauth token from vk.com

Dependency:
	mono
	webkit
	gtk-sharp
	webkit-sharp


Usage:

	oauth.exe 'https://oauth.vk.com/authorize?client_id=<APP_ID>&scope=<SCOPE>&display=popup&response_type=token'

	Command will open window with login page and after access confirmation will return json with tokens to console.