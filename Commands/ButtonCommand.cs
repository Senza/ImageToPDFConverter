using ImageToPDFConverter.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageToPDFConverter.Commands
{
    public class ButtonCommand<TViewModel> : CommandBase where TViewModel : ViewModelBase
    {
        private readonly Action command;
        public ButtonCommand(Action command)
        {
            this.command = command;
        }
        public override void Execute(object? parameter)
        {
            command();
        }
    }
}

