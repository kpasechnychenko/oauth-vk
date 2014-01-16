using System;
using Gtk;
using System.Collections.Generic;
using System.Linq;

namespace kpasech.oauth
{
	class MainClass
	{
		const string RESULT_JSON = @"{{""auth_token"": ""{0}"", ""userId"": ""{1}"", ""expirationTime"": ""{2}""}}";
		const string ERROR_JSON = @"{{""error"": ""access denied"",""code"": ""{0}"", ""description"": ""{1}""}}";
		
		public static void Main (string[] args)
		{
			try
			{
				if(args.Length < 2)
				{
					OnError(31 ,"APP_ID or SCOPE is not specified");
				}
				
				var optParam = new Dictionary<string, string>();
				if(args.Length > 2)
				{
					var args_to_parse = (new List<string>(args)).Skip(2).ToList<string>();
					foreach(string arg in args_to_parse)
					{
						var arr = arg.Split('=');
						if(arr.Length == 2)
						{
							if(arr[0] == "size")
							{
								var size = arr[1].Split(',');
								if(size.Length == 2)
								{
									optParam.Add("window_width", size[0]);
									optParam.Add("window_height", size[1]);
								}
								continue;
							}
							optParam.Add(arr[0], arr[1]);
						}
					}
				}
				
				Application.Init ();
				
				var starter = new LoginWindow(args[0], args[1], optParam);
				var result = "";
				var result_submitted = false;
				
				starter.OnKeyReceived += (key, userId, expirationTime) => 
				{
					result = string.Format(RESULT_JSON, key, userId, expirationTime);
					result_submitted = true;
					starter.Close();
					Application.Quit();
				};
				
				starter.OnErrorReceived += OnError;
				
				starter.Init();
				starter.ShowAll();
				
				Application.Run ();
				
				if(!result_submitted)
				{
					OnError(30 ,"Token was requested but didn't received");
				}
				
				Console.Clear();
				Console.WriteLine(result);
				Environment.Exit(0);
			}
			catch(Exception ex)
			{
				OnError(29 ,ex.Message);
			}
		}
		
		public static void OnError(int code, string description)
		{
			Application.Quit();
			Console.Clear();
			Console.WriteLine(string.Format(ERROR_JSON, code.ToString(), description));
			Environment.Exit(1);
		}
	}
}
