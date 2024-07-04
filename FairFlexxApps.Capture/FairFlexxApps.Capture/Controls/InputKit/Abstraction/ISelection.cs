using System;

namespace FairFlexxApps.Capture.Controls.InputKit.Abstraction
{
    public interface ISelection
    {
        bool IsSelected { get; set; }
        object Value { get; set; }
        bool IsDisabled { get; set; }
        event EventHandler Clicked;
    }
}
