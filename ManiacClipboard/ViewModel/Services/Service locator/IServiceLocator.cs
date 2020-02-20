using System;
using System.Collections.Generic;
using System.Text;

namespace ManiacClipboard.ViewModel
{
    public interface IServiceLocator
    {

        IClipboardService ClipboardService { get; }

        IUserMessageService UserMessageService { get; }

    }
}
