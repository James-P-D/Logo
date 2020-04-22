using System.Windows.Forms;
using System.Drawing;

namespace Logo
{
  public static class ThreadHelper
  {
    #region SetText

    delegate void SetTextCallback(Form form, Control control, string text);

    /// <summary>
    /// Set text property of various controls
    /// </summary>
    /// <param name="form">The calling form</param>
    /// <param name="control"></param>
    /// <param name="text"></param>
    public static void SetText(Form form, Control control, string text)
    {
      // InvokeRequired required compares the thread ID of the 
      // calling thread to the thread ID of the creating thread. 
      // If these threads are different, it returns true. 
      if (control.InvokeRequired)
      {
        var setTextCallback = new SetTextCallback(SetText);
        form.Invoke(setTextCallback, form, control, text);
      }
      else
      {
        control.Text = text;
      }
    }

    #endregion

    #region AddText

    delegate void AddTextCallback(Form form, Control control, string text);

    /// <summary>
    /// Set text property of various controls
    /// </summary>
    /// <param name="form">The calling form</param>
    /// <param name="control"></param>
    /// <param name="text"></param>
    public static void AddText(Form form, Control control, string text)
    {
      // InvokeRequired required compares the thread ID of the 
      // calling thread to the thread ID of the creating thread. 
      // If these threads are different, it returns true. 
      if (control.InvokeRequired)
      {
        var addTextCallback = new AddTextCallback(AddText);
        form.Invoke(addTextCallback, form, control, text);
      }
      else
      {
        if (control.Text.Length > 1000)
        {
          control.Text = text;
        }
        else
        {
          control.Text += text;
        }
      }
    }

    #endregion

    #region ScrollToEnd

    delegate void ScrollToEndCallback(Form form, Control control);

    /// <summary>
    /// Set text property of various controls
    /// </summary>
    /// <param name="form">The calling form</param>
    /// <param name="control"></param>
    public static void ScrollToEnd(Form form, Control control)
    {
      // InvokeRequired required compares the thread ID of the 
      // calling thread to the thread ID of the creating thread. 
      // If these threads are different, it returns true. 
      if (control.InvokeRequired)
      {
        var scrollToEndCallback = new ScrollToEndCallback(ScrollToEnd);
        form.Invoke(scrollToEndCallback, form, control);
      }
      else
      {
        ((TextBox) control).SelectionStart = ((TextBox) control).Text.Length;
        ((TextBox) control).ScrollToCaret();
      }
    }

    #endregion

    #region SetImage

    delegate void SetImageCallback(Form form, PictureBox control, Image image);

    public static void SetImage(Form form, PictureBox control, Image image)
    {
      // InvokeRequired required compares the thread ID of the 
      // calling thread to the thread ID of the creating thread. 
      // If these threads are different, it returns true. 
      if (control.InvokeRequired)
      {
        var setImageCallback = new SetImageCallback(SetImage);
        form.Invoke(setImageCallback, form, control, image);
      }
      else
      {
        control.Image = image;
      }
    }

    #endregion
  }
}