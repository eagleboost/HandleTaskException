using System.Windows;

namespace TaskExceptionApp
{
  public partial class MessageBox : Window
  {
    public MessageBox()
    {
      InitializeComponent();
      CloseButton.Click += (s, e) => Close();
    }

    public bool? ShowMessage(string title, string msg)
    {
      Title = title;
      Message.Text = msg;
      
      return ShowDialog();
    }
  }
}