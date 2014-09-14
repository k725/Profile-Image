using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CoreTweet;

namespace ProfileImage
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		private OAuth.OAuthSession auth;
		private Tokens token;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void browserOpenButton_Click(object sender, RoutedEventArgs e)
		{
			if (consumerKey.Text != string.Empty && consumerSecret.Text != string.Empty)
			{
				auth = OAuth.Authorize(consumerKey.Text, consumerSecret.Text);
				Process.Start(auth.AuthorizeUri.AbsoluteUri);
			}
			else
			{
				MessageBox.Show("CK / CS Empty!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void authenticationButton_Click(object sender, RoutedEventArgs e)
		{
			token = auth.GetTokens(pinNumber.Text);
		}

		private void imageSourceButton_Click(object sender, RoutedEventArgs e)
		{
			var img = new OpenFileDialog();
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
			token.Account.UpdateProfileImage(image => new FileInfo(imageSource.Text));
		}
	}
}