using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Minecraft_Server_Status_Checker
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static ObservableCollection<Server> PreServers;

        private ObservableCollection<Server> Servers;
        private List<string> ServersName = new List<string>();
        public Server selected;

        private bool IsOnMultipleSelectionState;
        private VisualState PreState;

        public MainPage()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;

            if (PreServers != null)
            {
                Servers = PreServers;
                PreServers = null;

                foreach (Server server in Servers)
                {
                    ServersName.Add(server.ServerName);
                }
            }
            else if (Servers == null)
            {
                Servers = new ObservableCollection<Server>();
            }

            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // If the app starts in narrow mode - showing only the Master listView - 
            // it is necessary to set the commands and the selection mode.
            if (PageSizeStatesGroup.CurrentState == NarrowState)
            {
                VisualStateManager.GoToState(this, MasterState.Name, true);
            }
            else if (PageSizeStatesGroup.CurrentState == WideState)
            {
                // In this case, the app starts is wide mode, Master/Details view, 
                // so it is necessary to set the commands and the selection mode.
                VisualStateManager.GoToState(this, MasterDetailsState.Name, true);
                ServerList.SelectionMode = ListViewSelectionMode.Extended;

                if (selected == null)
                {
                    if (Servers.Count > 0)
                    {
                        ServerList.SelectedIndex = 0;
                    }
                    else
                    {
                        DetailContent.Navigate(typeof(EmptyDetailPage));
                    }
                    
                }
                else
                {
                    ServerList.SelectedItem = selected;
                }            
                
            }
            else
            {
                new InvalidOperationException();
            }
        }


        private void PageSizeStatesGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            bool isNarrow = e.NewState == NarrowState;
            if (isNarrow)
            {
                VisualStateManager.GoToState(this, MasterState.Name, true);
            }
            else
            {
                VisualStateManager.GoToState(this, MasterDetailsState.Name, true);
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(ServerList, isNarrow);
            if (DetailContent != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContent, !isNarrow);
            }
        }

        private void MasterDetailsStatesGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Equals(MultipleSelectionState))
            {
                IsOnMultipleSelectionState = true;
                PreState = e.OldState;
            }
            else
            {
                IsOnMultipleSelectionState = false;
            }
        }

        private void ServerList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(ServerDetailsPage), e.ClickedItem, new SuppressNavigationTransitionInfo());
        }

        private void ServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = ServerList.SelectedItem as Server;
            if (selected != null && !IsOnMultipleSelectionState)
            {
                DetailContent.Navigate(typeof(ServerDetailsPage), selected, new SuppressNavigationTransitionInfo());
                EnableContentTransitions();
            }     
            else
            {
                //DetailContent.Navigate(typeof(EmptyDetailPage));
            }         
        }

        private void EnableContentTransitions()
        {
            DetailContent.ContentTransitions.Clear();
            DetailContent.ContentTransitions.Add(new EntranceThemeTransition());
        }

        #region AddServerDialog
        private void AddServer_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            textServerName.ClearValue(TextBox.TextProperty);
            textServerAddress.ClearValue(TextBox.TextProperty);
            textServerPort.ClearValue(TextBox.TextProperty);
        }

        private void AddServer_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrEmpty(this.textServerPort.Text))
            {
                this.textServerPort.Text = "25565";
            }
            Server server = new Server(this.textServerName.Text, this.textServerAddress.Text, int.Parse(this.textServerPort.Text), Status.ServerVersion.MC_Current);

            ServersName.Add(this.textServerName.Text);
            Servers.Add(server);
            CoreManager.SaveServersList(Servers);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.ServersName.Contains(this.textServerName.Text))
            {
                this.textErr.Text = "服务器名已存在";
            }
            else if (string.IsNullOrEmpty(this.textServerAddress.Text))
            {
                this.textErr.Text = "服务器地址不能为空";
            }
            else if (!this.isNumberic(this.textServerPort.Text))
            {
                this.textErr.Text = " 端口号只能为数字";
            }
            else
            {
                this.AddServer.IsPrimaryButtonEnabled = true;
                this.textErr.Text = "";
                return;
            }

            AddServer.IsPrimaryButtonEnabled = false;
        }
        #endregion

        #region Commonds
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            await AddServer.ShowAsync(); 
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectItmesBtn_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, MultipleSelectionState.Name, true);
        }

        private void DeleteItems_Click(object sender, RoutedEventArgs e)
        {
            Server[] InDeleteServerList = new Server[ServerList.SelectedItems.Count];
            ServerList.SelectedItems.CopyTo(InDeleteServerList, 0);
            foreach (Server server in InDeleteServerList) 
            {
                Servers.Remove(server);
                ServersName.Remove(server.ServerName);
            }

            CoreManager.SaveServersList(Servers);

            if (PreState != null)
            {
                VisualStateManager.GoToState(this, PreState.Name, true);
            }

            if (Servers.Count > 0)
            {
                ServerList.SelectedIndex = 0;
            }
            else
            {
                ServerList.SelectedItem = null;
            } 
        }

        private void CancelSelection_Click(object sender, RoutedEventArgs e)
        {
            if (PreState != null)
            {
                VisualStateManager.GoToState(this, PreState.Name, true);
            }
        }
        #endregion

        public bool isNumberic(string str)
        {
            foreach (char c in str)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
   
    }
}
