namespace TaskExceptionApp
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;

  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      Application.Current.DispatcherUnhandledException += (_, e) =>
      {
        ShowException(e.Exception.Message, e.Exception);
        e.Handled = true;
      };

      TaskScheduler.UnobservedTaskException += (_, e) =>
      {
        ShowException(e.Exception.Message, e.Exception);
        e.SetObserved();
      };
    }

    private void ButtonBase_OnClick_Unobserved(object sender, RoutedEventArgs e)
    {
      TaskWithErrorAsync();
    }
    
    private void ButtonBase_OnClick_ContinueWith(object sender, RoutedEventArgs e)
    {
      TaskWithErrorAsync().ContinueWith(t =>
      {
        if (t.Exception != null)
        {
          ShowException("Failed to execute xxx", t.Exception);
        }
      });
    }

    private void ButtonBase_OnClick_WhenFaulted(object sender, RoutedEventArgs e)
    {
      TaskWithErrorAsync().WhenFaultedAsync("Failed to execute xxx");
    }

    private void ShowException(string title, Exception ex)
    {
      if (Dispatcher.CheckAccess())
      {
        ShowExceptionCore(title, ex);
      }
      else
      {
        Dispatcher.BeginInvoke((Action) (() => ShowExceptionCore(title, ex)));
      }
    }
    
    private static void ShowExceptionCore(string title, Exception ex)
    {
      new MessageBox().ShowMessage(title, ex.ToString());
    }
    
    private static Task TaskWithErrorAsync()
    {
      return Task.Run(() => { throw new ApplicationException(); });
    }
  }
}