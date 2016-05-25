using FelicidApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FelicidApp.View
{
    public sealed partial class UserNameDialog : ContentDialog
    {
        public UserNameDialog()
        {
            this.InitializeComponent();

            UserName.Text = ConfigurationService.UserName;
            DeviceId.Text = ConfigurationService.DeviceId;
            DeviceKey.Text = ConfigurationService.DeviceKey;
        }

        private void OnButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(UserName.Text)
                || string.IsNullOrWhiteSpace(DeviceId.Text)
                || string.IsNullOrWhiteSpace(DeviceKey.Text))
            {
                args.Cancel = true;
            }
            else
            {
                ConfigurationService.UserName = UserName.Text;
                 ConfigurationService.DeviceId= DeviceId.Text;
                ConfigurationService.DeviceKey= DeviceKey.Text;
            }
        }
    }
}
