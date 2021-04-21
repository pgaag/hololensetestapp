using Kinemic.Gesture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409



namespace TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private readonly Engine _engine;
        private readonly Dictionary<string, string> Dict = new Dictionary<string, string>()
        {
            {"Picker", "Holt teil aus dem Lager"},
            {"Monteur", "Montiert Teile nach Anleitung"},
            {"Tester", "Testet Werkstücke auf korrektheit"}
        };

        public MainPage()
        {
            this.InitializeComponent();
            //this.Picker.IsEnabled = false;
            this.Monteur.IsChecked = true;
            RoleDescription.Text = Dict["Monteur"];

            _engine = Engine.Default;
            _engine.GestureDetected += Engine_GestureDetected;
            this._engine.ConnectionStateChanged += Engine_ConnectionStateChanged;
        }

        private void Engine_ConnectionStateChanged(Engine sender, ConnectionStateChangedEventArgs args)
        {
            _engine.Vibrate(args.Band, 300);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // connect to the nearest Kinemic Band
            _engine.ConnectStrongest();

        }

        private void TesterChecked(object sender, RoutedEventArgs e)
        {

        }

        private void Engine_GestureDetected(Engine sender, GestureDetectedEventArgs e)
        {
            // give haptic feedback
            _engine.Vibrate(e.Band, 300);

            // do gesture specific stuff
            switch (e.Gesture)
            {
                case Gesture.ROTATE_RL:
                    SwitchRole('D');
                    break;
                case Gesture.ROTATE_LR:          
                    SwitchRole('U');
                    break;
                case Gesture.CIRCLE_R:
                    break;
                case Gesture.CIRCLE_L:
                    break;
                case Gesture.SWIPE_R:
                    break;
                case Gesture.SWIPE_L:
                    break;
                case Gesture.SWIPE_UP:
                    break;
                case Gesture.SWIPE_DOWN:
                    break;
                case Gesture.CHECK_MARK:
                    this.ShowRole();
                    break;
                case Gesture.CROSS_MARK:
                    break;
                case Gesture.EARTOUCH_L:
                    System.Environment.Exit(0);
                    break;
            }

        }

        private async void ShowRole()
        {

            foreach (RadioButton item in RoleList.Items)
            {
                // IsChecked return may not be a bool
                if (true == item.IsChecked)
                {
                    MessageDialog dialog = new MessageDialog("Du hat den " + item.Content.ToString() + " ausgewählt.", "Rolle");
                    await dialog.ShowAsync();
                    break;
                }
            }
        }

        private void SwitchRole(char direction)
        {
            //TODO: Ugly, to Improve
            if (direction == 'U' || direction == 'D')
            {
                int listSize = RoleList.Items.Count;
                for (int i = 0; i < listSize; i++)
                {
                    RadioButton item = (RadioButton)RoleList.Items[i];

                    // IsChecked return may not be a bool
                    if (true == item.IsChecked)
                    {
                        i += direction=='U'?-1:1;

                        // Check for Rollover
                        if (i < 0)
                        {
                            i =+ (listSize - 1);
                        }
                        else if (i >= listSize)
                        {
                            i %= listSize;
                        }

                        item = (RadioButton)RoleList.Items[i];
                        item.IsChecked = true;
                        RoleDescription.Text = Dict[item.Content.ToString()];
                        break;
                    }
                }
            }
        }
    }
}
