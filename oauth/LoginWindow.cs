using System;
using Gtk;
using WebKit;
using System.Text.RegularExpressions;


namespace kpasech.oauth
{
	public delegate void KeyReceived(string key, string userId, Int64 expirationTime);
	
	public class LoginWindow:IDisposable
	{
		private Window window;
		private string _url;
		
		public event KeyReceived OnKeyReceived;
		
		public LoginWindow (string url)
		{
			window = new Window("LogIn");
			_url = url;
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			//window.Dispose();
		}
		#endregion

		#region implementation
		public void Init ()
		{
			var fxd = new Fixed();
			window.Add(fxd);
			var wbkt = new WebView();
			fxd.Add(wbkt);
			
			//init window
			window.SetSizeRequest(520,320);
			window.AllowShrink = false;
			window.Resizable = false;
			window.SetPosition(WindowPosition.CenterAlways);
			
			window.DeleteEvent += (o, args) => 
			{
				Application.Quit();
				Console.Clear();
				Console.WriteLine("'error': 'User has closed window'");
				Environment.Exit(1);
			};			
			
			//webkit init
			wbkt.Open(this._url);
			
			wbkt.LoadFinished += (o, args) => {
				if(wbkt.Uri.Contains("/blank.html"))
				{
					if(wbkt.Uri.ToLower().Contains("?error="))
					{
						//Error happened
						
						//TODO: add normal error handling 
						// error string example http://REDIRECT_URI?error=access_denied&error_description=The+user+or+authorization+server+denied+the+request.
						throw new ApplicationException("Error while login to vkontakte: " + wbkt.Uri.ToLower());
					}
					else
					{

						var keyRegex = new Regex("access_token=[0-9,a-f]+(&|$)", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);
						var userIdRegex = new Regex("user_id=[0-9]+(&|$)", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);
						var expirationTimeRegex = new Regex("expires_in=[0-9]+(&|$)", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);
						
						
						var keyMatch = keyRegex.Match(wbkt.Uri);
						var userIdMatch = userIdRegex.Match(wbkt.Uri);
						var expirationTimeMatch = expirationTimeRegex.Match(wbkt.Uri);
						
						if(keyMatch.Success && userIdMatch.Success && expirationTimeMatch.Success)
						{
							OnKeyReceived(
								keyMatch.Value.Substring(13).Replace("&",""),
								userIdMatch.Value.Substring(8).Replace("&",""),
								Convert.ToInt64(expirationTimeMatch.Value.Substring(11).Replace("&",""))
								);
						}
						else
						{
							throw new ApplicationException("Error while login to vkontakte");
						}
						
					}
				}
			};
			
		}

		public void ShowAll ()
		{
			window.ShowAll();
		}

		public void Close ()
		{
			window.HideAll();
		}
		
		#endregion
	}
}