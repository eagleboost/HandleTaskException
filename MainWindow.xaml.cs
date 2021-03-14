namespace TaskExceptionApp
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Threading;

  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private readonly UnobservedStatusUpdater _unobservedStatusUpdater;
    
    public MainWindow()
    {
      InitializeComponent();

      _unobservedStatusUpdater = new UnobservedStatusUpdater(this);
      
      Application.Current.DispatcherUnhandledException += (_, e) =>
      {
        ShowException(e.Exception.Message, e.Exception);
        e.Handled = true;
      };

      TaskScheduler.UnobservedTaskException += (_, e) =>
      {
        _unobservedStatusUpdater.Stop();
        ShowException(e.Exception.Message, e.Exception);
        e.SetObserved();
      };

      Closing += (_, e) => _unobservedStatusUpdater.Stop();
    }

    private void ButtonBase_OnClick_Unobserved(object sender, RoutedEventArgs e)
    {
      TaskWithErrorAsync();
      _unobservedStatusUpdater.Start();
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

    private class UnobservedStatusUpdater
    {
      private readonly DispatcherTimer _timer = new DispatcherTimer();
      private DateTime _startTime;

      public UnobservedStatusUpdater(MainWindow window)
      {
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => window.UnobservedStatus.Text = $"Waiting for GC... {(DateTime.Now - _startTime).Seconds:0#} seconds";
      }

      public void Start()
      {
        if (!_timer.IsEnabled)
        {
          _startTime = DateTime.Now;
          _timer.Start();
        }
      }

      public void Stop()
      {
        _timer.Stop();
      }
    }
  }
}