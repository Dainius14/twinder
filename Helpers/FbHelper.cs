using SimpleBrowser;
using System.Threading;

namespace Twinder.Helpers
{
	static class FbHelper
	{
		private const string URL = @"https://www.facebook.com/v2.6/dialog/oauth?redirect_uri=fb464891386855067%3A%2F%2Fauthorize%2F&scope=user_birthday,user_photos,user_education_history,email,user_relationship_details,user_friends,user_work_history,user_likes&response_type=token%2Csigned_request&client_id=464891386855067";
		private const string USER_AGENT = "Mozilla/5.0 (Linux; U; en-gb; KFTHWI Build/JDQ39) AppleWebKit/535.19 (KHTML, like Gecko) Silk/3.16 Safari/535.19";
		

		// NONE OF THIS WORKS
		public static string GetFbToken(string email, string password)
		{
			string token = string.Empty;

			Browser browser = new Browser();
			browser.UserAgent = USER_AGENT;
			browser.Navigate(URL);

			// Enters email, password and logins
			browser.Find(ElementType.TextField, FindBy.Name, "email").Value = email;
			browser.Find("u_0_2").Value = password;
			browser.Find(ElementType.Button, FindBy.Name, "login").Click();

			// IF SMS authentication is enabled, after aprooving from the browser manually, clicks ok to continue
			// TODO not hang the app
			while (browser.ContainsText("Enter Security Code to Continue"))
			{
				browser.Navigate(browser.Url);
				Thread.Sleep(1000);
			}

			//browser.Find("approvals_code").Value = "416961";
			browser.Find(ElementType.Button, FindBy.Name, "submit[Submit Code]").Click();
			browser.Find(ElementType.Button, FindBy.Name, "submit[Continue]").Click();
			browser.Find(ElementType.Button, FindBy.Name, "submit[This was me]").Click();
			browser.Find(ElementType.Button, FindBy.Name, "submit[Continue]").Click();

			// Tinder is already authorized prompt, presses ok
			//browser.Find("u_0_1").Click();

			// access_token= lenght is 13
			int indexOfTokenEnd = browser.CurrentHtml.IndexOf("access_token=") + 13;
			token = browser.CurrentHtml.Remove(0, indexOfTokenEnd);
			token = token.Remove(token.IndexOf("&"));

			return null;
		}
	}
}
