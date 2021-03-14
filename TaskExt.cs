namespace TaskExceptionApp
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Threading;

  public static class TaskExt
  {
    private const TaskContinuationOptions FaultedFlag = TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously;
    
    public static Task WhenFaultedAsync(this Task task, Dispatcher dispatcher, string context)
    {
      task.ContinueWith(t =>
      {
        if (t.Exception != null)
        {
          dispatcher.BeginInvoke(() => throw new AggregateException(context, t.Exception.InnerExceptions));
        }
      }, FaultedFlag);
      
      return task;
    }
    
    public static Task WhenFaultedAsync(this Task task, SynchronizationContext syncContext, string context)
    {
      task.ContinueWith(t =>
      {
        if (t.Exception != null)
        {
          syncContext.Post(_ => throw new AggregateException(context, t.Exception.InnerExceptions), null);
        }
      }, FaultedFlag);
      
      return task;
    }
    
    public static Task WhenFaultedAsync(this Task task, string context)
    {
      return task.WhenFaultedAsync(Application.Current.Dispatcher, context);
    }
  }
}