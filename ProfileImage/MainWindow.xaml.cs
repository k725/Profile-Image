using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using TweetSharp;

namespace ProfileImage
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		private OAuthRequestToken requestToken;
		private string accessToken;
		private string accessTokenSecret;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void browserOpenButton_Click(object sender, RoutedEventArgs e)
		{
			TwitterService service = new TwitterService(consumerKey.Text, consumerSecret.Text);
			if (consumerKey.Text != string.Empty && consumerSecret.Text != string.Empty)
			{
				requestToken = service.GetRequestToken();
				Process.Start(service.GetAuthenticationUrl(requestToken).ToString());
			}
			else
			{
				MessageBox.Show("CK/CS Error!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void authenticationButton_Click(object sender, RoutedEventArgs e)
		{
			TwitterService service = new TwitterService(consumerKey.Text, consumerSecret.Text);
			OAuthAccessToken access = service.GetAccessToken(requestToken, pinNumber.Text);
			service.AuthenticateWith(access.Token, access.TokenSecret);
			accessToken = access.Token;
			accessTokenSecret = access.TokenSecret;

			TwitterUser user = service.VerifyCredentials(new VerifyCredentialsOptions());
			screenName.Text = user.ScreenName;
			profileImage.Text = user.ProfileImageUrlHttps;
			backImage.Text = user.ProfileBackgroundImageUrlHttps;
			userLocation.Text = user.Location;
			userName.Text = user.Name;
		}

		private void imageSourceButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog img = new OpenFileDialog();
			img.Title = "Select image file.";
			if (img.ShowDialog() == true)
			{
				// https://dev.twitter.com/docs/api/1.1/post/account/update_profile_image
				// ImageSize 700KB
				FileInfo fi = new FileInfo(img.FileName);
				if (fi.Length >= 716800)
				{
					MessageBox.Show("File size is too large.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				else
				{
					imageSource.Text = img.FileName;
				}
			}
		}

		private void imageUploadButton_Click(object sender, RoutedEventArgs e)
		{
			TwitterService service = new TwitterService(consumerKey.Text, consumerSecret.Text);
			service.AuthenticateWith(accessToken, accessTokenSecret);

			var updateOpt = new UpdateProfileImageOptions();
			updateOpt.ImagePath = imageSource.Text;
			service.UpdateProfileImage(updateOpt);

			TwitterUser user = service.VerifyCredentials(new VerifyCredentialsOptions());
			screenName.Text = user.ScreenName;
			profileImage.Text = user.ProfileImageUrlHttps;
			backImage.Text = user.ProfileBackgroundImageUrlHttps;
			userLocation.Text = user.Location;
			userName.Text = user.Name;
		}
	}
}