using System;

namespace FairFlexxApps.Capture.Controls.InputKit.Abstraction
{
    public interface IValidatable
    {
        bool IsRequired { get; set; }
        bool IsValidated { get; }
        string ValidationMessage { get; set; }
        void DisplayValidation();
        event EventHandler ValidationChanged;
    }
}
