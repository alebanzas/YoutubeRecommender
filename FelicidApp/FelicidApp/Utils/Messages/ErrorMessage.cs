using System;
using Windows.UI.Xaml;

namespace FelicidApp.Utils.Messages
{
    public class ErrorMessage
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
        public static Visibility Visibility { get; internal set; }

        public ErrorMessage(string title, string message)
            : this(title, message, null)
        {
        }

        public ErrorMessage(string title, string message, Exception exception)
        {
            Title = title;
            Message = message;
            Exception = exception;
        }
    }
}
