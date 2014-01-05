using System;
using Gtk;
using WebKit;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace kpasech.oauth
{
	public delegate void KeyReceived(string key, string userId, Int64 expirationTime);
	public delegate void ErrorReceived(int code, string description);
	
	public class LoginWindow:IDisposable
	{
		const string URL = "https://oauth.vk.com/authorize?client_id={0}&scope={1}&display={2}&v={3}&response_type=token";
		
		private Window _window;
		private string _app_id;
		private string _scope;
		private string _display;
		private string _api_veresion;
		private WebView _wbkt;
		private int _window_height;
		private int _window_width;
		
		public event KeyReceived OnKeyReceived;
		public event ErrorReceived OnErrorReceived;
		
		private string getDictValue(Dictionary<string, string> dict, string key, string defaultValue = default(string))
		{
			if(dict.ContainsKey(key))
			{
				return dict[key];
			}
			else
			{
				return defaultValue;
			}
		}
		
		public LoginWindow (string app_id, string scope, Dictionary<string, string> optional_parameters)
		{
			_window = new Window("LogIn");
			
			_app_id = app_id;
			_scope = scope;
			_display = getDictValue(optional_parameters, "display", "popup");
			_api_veresion = getDictValue(optional_parameters, "api_veresion", "5.5");
			_window_width = int.Parse(getDictValue(optional_parameters, "window_width", "460"));
			_window_height = int.Parse(getDictValue(optional_parameters, "window_height", "320"));
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			_window.Dispose();
		}
		#endregion
		
		private void throwError(int code, string description)
		{
			OnErrorReceived(code, description);
		}

		public void Init ()
		{
			try{
				var fxd = new Fixed();
				_window.Add(fxd);
				_wbkt = new WebView();
				fxd.Add(_wbkt);
				
				//init window
				this.SetSizeRequest(_window_width, _window_height);
				_window.AllowShrink = false;
				_window.Resizable = false;
				_window.SetPosition(WindowPosition.CenterAlways);
				
				//subscribe on events
				_window.DeleteEvent += OnDelete;
				_wbkt.LoadFinished += OnWindowLoaded;
				
				//webkit init
				_wbkt.Open(string.Format (URL, this._app_id, _scope, _display, _api_veresion));
			}
			catch(Exception ex)
			{
				this.throwError(22, "Error while window initialization: " + ex.Message);
			}
			
		}

		public void ShowAll ()
		{
			try
			{
				_window.ShowAll();
			}
			catch(Exception ex)
			{
				this.throwError(23, ex.Message);
			}
		}

		public void Close ()
		{
			try
			{
				_window.HideAll();
			}
			catch(Exception ex)
			{
				this.throwError(24, ex.Message);
			}
		}
		
		public void SetSizeRequest(int x=520, int y=320)
		{
			try
			{
				_window.SetSizeRequest(x,y);
			}
			catch(Exception ex)
			{
				this.throwError(25, ex.Message);
			}
		}
		
		protected void OnDelete(object o, DeleteEventArgs args)
		{
			this.throwError(33, "User has closed window");
		}
		
		public void OnWindowLoaded(object o, LoadFinishedArgs args)
		{
				if(_wbkt.Uri.Contains("/blank.html"))
				{
					if(_wbkt.Uri.ToLower().Contains("?error="))
					{
						// error string example http://REDIRECT_URI?error=access_denied&error_description=The+user+or+authorization+server+denied+the+request.
						var errorMessageRegexp = new Regex("error_description=[0-9,a-f]+(&|$)", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);
						var errorMessage = errorMessageRegexp.Match(_wbkt.Uri);
						if( errorMessage.Success)
						{
							var message = errorMessage.Value.Substring(18).Replace("&","");
						    //message = (message);
							message = message.Replace("+", " ");
							this.throwError(28, "Error while login to vkontakte: " + message);
						}
						else
						{
							this.throwError(27, "Error while login to vkontakte: " + _wbkt.Uri.ToLower());
						}
					}
					else
					{

						var keyRegex = new Regex("access_token=[0-9,a-f]+(&|$)", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);
						var userIdRegex = new Regex("user_id=[0-9]+(&|$)", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);
						var expirationTimeRegex = new Regex("expires_in=[0-9]+(&|$)", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);
						
						
						var keyMatch = keyRegex.Match(_wbkt.Uri);
						var userIdMatch = userIdRegex.Match(_wbkt.Uri);
						var expirationTimeMatch = expirationTimeRegex.Match(_wbkt.Uri);
						
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
							this.throwError(26, "No authentication keys has been returned");
						}
						
					}
				}
				else if(_wbkt.SearchText("error", false,true, false))
				{
					// TODO: Add error handling here
					
					//_wbkt.SelectAll();
					//_wbkt.CopyClipboard();
					
					this.OnErrorReceived(34, "Unknown error while login to vk.com");
				}
			}
	}
}