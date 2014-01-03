using System;
using Gtk;
using System.Threading;

namespace kpasech.oauth
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try
			{
				if(args.Length == 0)
				{
					throw new ApplicationException("Login URL is not specified");
				}
				
				Application.Init ();
				
				var starter = new LoginWindow(args[0]);
				var result = "'auth_token': '{0}', 'userId': '{1}, 'expirationTime': '{2}'";
				var result_submitted = false;
				
				starter.OnKeyReceived += (key, userId, expirationTime) => 
				{
					result = string.Format(result, key, userId, expirationTime);
					result_submitted = true;
					starter.Close();
					Application.Quit();
				};
				
				starter.Init();
				starter.ShowAll();
				
				//Console.Clear();
				
				Application.Run ();
				if(!result_submitted)
				{
					throw new ApplicationException("Token was requested but didn't received");
				}
				
				Console.Clear();
				Console.WriteLine(result);
				Environment.Exit(0);
			}
			catch(Exception ex)
			{
				Application.Quit();
				Console.Clear();
				Console.WriteLine(string.Format("'error': '{0}'", ex.Message));
				Environment.Exit(1);
			}
		}
	}
}
