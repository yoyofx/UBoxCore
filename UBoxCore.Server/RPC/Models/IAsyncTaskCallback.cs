using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace UBoxCore.Server.RPC.Models
{
    public interface IAsyncTaskCallback
    {
        string CallbackName {  get; }
        void InvokeAsync();

        event AsyncCompletedEventHandler AsyncCompleted;

        event AsyncProgressChangedEventHandler AsyncProgressChanged;
    }
}
