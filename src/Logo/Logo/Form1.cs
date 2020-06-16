using Executor;
using LogicalParser;
using StringParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using LogicalParser.Commands;
using LogicalParser.Objects;

//TODO;c
// How do we know when thread has ended? And make sure we disable Run and Step buttons when running, otherwise exceptions
// Fix for error line numbers (are we not working correctly with comments?)
// Check variables (do we need that value in there? Nope! just need to get multiple-inheritance working with interfaces)
// Check if(..) {..} else if {..} !!
// Move error strings to resources file
// Check 'Unable to parse' in general. 

namespace Logo
{
  public partial class Form1 : Form
  {
    private Image imageWithoutTurtle;
    private Image imageWithTurtle;
    private bool compiled = false;
    private bool wrapBorders = false;
    private bool updateTextBoxes = true;
    private bool stepMode = false;

    private List<Command> commands;
    private List<LogoObject> objects;

    private bool isDirty = false;
    private const string InitialDirectory = @".\Samples";
    private string currentFilename;
    private readonly Executor.Executor executor;

    public Form1()
    {
      InitializeComponent();

      wrapBordersCheckBox.Checked = wrapBorders;
      updateUICheckBox.Checked = updateTextBoxes;
      executor = new Executor.Executor();
      executor.AddOutputTextEvent += Executor_AddOutputTextEvent;
      executor.UpdateEvent += Executor_UpdateEvent;
      this.initialiseTextboxFontSizeSettings();

      programTextBox.Text = string.Empty;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      InitialiseCanvasAndTurtle();
    }

    private void InitialiseCanvasAndTurtle()
    {
      var imageWidth = pictureBox1.Width;
      var imageHeight = pictureBox1.Height;
      
      imageWithoutTurtle = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);
      using (var grp = Graphics.FromImage(imageWithoutTurtle))
      {
        grp.FillRectangle(Brushes.White, 0, 0, imageWidth, imageHeight);
      }

      Turtle turtle=new Turtle();
      DrawTurtle(turtle);
      UpdateTextboxes(turtle);
      ThreadHelper.SetImage(this, pictureBox1, imageWithTurtle);
    }

    private void UpdateImage(Turtle turtle, int x1, int y1)
    {
      using (var grp = Graphics.FromImage(imageWithoutTurtle))
      {
        var xCenter = imageWithoutTurtle.Width / 2;
        var yCenter = imageWithoutTurtle.Height / 2;

        var startX = x1 + xCenter;
        var startY = y1 + yCenter;
        var endX = (int)turtle.X + xCenter;
        var endY = (int)turtle.Y + yCenter;

        var pen = new Pen(Color.FromArgb(turtle.ColorA, turtle.ColorR, turtle.ColorG, turtle.ColorB));
        grp.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));

        if (wrapBorders)
        {
          if (
            endX < 0 || endX > imageWithoutTurtle.Width ||
            endY < 0 || endY > imageWithoutTurtle.Height)
          {
            if (endX < 0)
            {
              startX = x1 + xCenter + imageWithoutTurtle.Width;
              turtle.SetX(turtle.X + imageWithoutTurtle.Width);
            }
            else if (endX > imageWithoutTurtle.Width)
            {
              startX = x1 + xCenter - imageWithoutTurtle.Width;
              turtle.SetX(turtle.X - imageWithoutTurtle.Width);
            }

            if (endY < 0)
            {
              startY = y1 + yCenter + imageWithoutTurtle.Height;
              turtle.SetY(turtle.Y + imageWithoutTurtle.Height);
            }
            else if (endY > imageWithoutTurtle.Height)
            {
              startY = y1 + yCenter - imageWithoutTurtle.Height;
              turtle.SetY(turtle.Y - imageWithoutTurtle.Height);
            }

            endX = (int)turtle.X + xCenter;
            endY = (int)turtle.Y + yCenter;

            grp.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
          }
        }
      }
    }

    private void DrawTurtle(Turtle turtle)
    {
      var x = turtle.X;
      var y = turtle.Y;
      var bottomLeftX = x;
      var bottomLeftY = y;
      var bottomRightX = x;
      var bottomRightY = y;

      turtle.CalculateNewPosition(turtle.CalculateNewDirection(45), -20, ref bottomLeftX, ref bottomLeftY);
      turtle.CalculateNewPosition(turtle.CalculateNewDirection(-45), -20, ref bottomRightX, ref bottomRightY);

      imageWithTurtle = (Image)imageWithoutTurtle.Clone();
      float xCenter = imageWithTurtle.Width / 2f;
      float yCenter = imageWithTurtle.Height / 2f;

      using (var grp = Graphics.FromImage(imageWithTurtle))
      {
        var pen = Pens.Black;

        grp.DrawLine(pen, new Point((int)(turtle.X + xCenter), (int)(turtle.Y + yCenter)),
          new Point((int)(bottomLeftX + xCenter), (int)(bottomLeftY + yCenter)));
        grp.DrawLine(pen, new Point((int)(turtle.X + xCenter), (int)(turtle.Y + yCenter)),
          new Point((int)(bottomRightX + xCenter), (int)(bottomRightY + yCenter)));
        grp.DrawLine(pen, new Point((int)(bottomLeftX + xCenter), (int)(bottomLeftY + yCenter)),
          new Point((int)(bottomRightX + xCenter), (int)(bottomRightY + yCenter)));
      }
    }

    private void UpdatePicture(Turtle turtle, int x1, int y1)
    {
      if (turtle.IsPenDown)
      {
        double tolerance = 0.0001;
        if (Math.Abs(turtle.X - x1) > tolerance || Math.Abs(turtle.Y - y1) > tolerance)
        {
          UpdateImage(turtle, x1, y1);
        }
      }

      if (turtle.IsVisible)
      {
        DrawTurtle(turtle);
        ThreadHelper.SetImage(this, pictureBox1, imageWithTurtle);
      }
      else
      {
        ThreadHelper.SetImage(this, pictureBox1, imageWithoutTurtle);
      }
    }
    
    void Executor_UpdateEvent(Turtle turtle, int x1, int y1)
    {
      UpdatePicture(turtle, x1, y1);
      UpdateTextboxes(turtle);
      if (!this.stepMode)
      {
        this.executor.ResumeThread();
      }
      else
      {
        ThreadHelper.SetButtonEnabled(this, this.stepButton, true);
        ThreadHelper.SetButtonEnabled(this, this.runButton, true);
      }
    }

    void Executor_AddOutputTextEvent(string text)
    {
      this.AddOutputText(text);
    }

    private void stopButton_Click(object sender, EventArgs e)
    {
      this.Stop();
    }

    private void Stop()
    {
      this.executor.Running = false;
      this.loadButton.Enabled = true;
      this.stepButton.Enabled = true;
      this.runButton.Enabled = true;
      if (this.stepMode)
      {
        this.stepMode = false;
        this.executor.ResumeThread();
      }
    }

    private void LoadButton_Click(object sender, EventArgs e)
    {
      bool savedUpdateTextBoxes = this.updateTextBoxes;
      this.updateTextBoxes = true;
      this.LoadProgram();
      this.updateTextBoxes = savedUpdateTextBoxes;
    }

    private void StepButton_Click(object sender, EventArgs e)
    {
      this.Step();
    }

    private void Step()
    {
      if (!this.executor.Running)
      {
        this.stepMode = true;
        this.Run();
      }
      else
      {
        this.executor.ResumeThread();
      }
    }

    private void runButton_Click(object sender, EventArgs e)
    {
      if (this.stepMode)
      {
        this.executor.ResumeThread();
        this.stepMode = false;
        this.stepButton.Enabled = false;
      }
      else
      {
        this.Run();
      }
    }
    
    private void initialiseTextboxFontSizeSettings()
    {
      setTextboxFontSizeSettings(this.programTextBox.Font.Size);
    }

    private void setTextboxFontSizeSettings(float fontSize)
    {
      this.programTextBox.Font = new Font("Courier New",
        fontSize, System.Drawing.FontStyle.Regular,
        System.Drawing.GraphicsUnit.Point,
        0);
      this.outputTextBox.Font = new Font("Courier New",
        fontSize, System.Drawing.FontStyle.Regular,
        System.Drawing.GraphicsUnit.Point,
        0);
    }

    private void increaseFontSizeButton_Click(object sender, EventArgs e)
    {
      float fontSize = Math.Max(5F, Math.Min(this.programTextBox.Font.Size + 1F, 20F));
      this.setTextboxFontSizeSettings(fontSize);
    }

    private void decreaseFontSizeButton_Click(object sender, EventArgs e)
    {
      float fontSize = Math.Max(5F, Math.Min(this.programTextBox.Font.Size - 1F, 20F));
      this.setTextboxFontSizeSettings(fontSize);
    }

    private void StringTokeniserAddOutputText(string text)
    {
      AddOutputText(text);
    }

    private bool LoadProgram()
    {
      this.compiled = false;
      ClearOutputText();
      var stringTokeniser = new StringTokeniser();
      try
      {
        stringTokeniser.AddOutputTextEvent += StringTokeniserAddOutputText;
        var allLines = programTextBox.Text.Split('\n');
        var stringTokens = stringTokeniser.Parse(allLines);

        var logicalParser = new Parser();
        logicalParser.Parse(stringTokens, out this.commands, out this.objects);

        AddOutputText("*** Compile success ***");
        this.compiled = true;
        return true;
      }
      catch (Exception ex)
      {
        AddOutputText("ERROR: " + ex.Message);
        AddOutputText("*** Compile failed ***");
        return false;
      }
      finally
      {
        stringTokeniser.AddOutputTextEvent -= StringTokeniserAddOutputText;
      }
    }

    private void Run()
    {
      // If the Logo program has been changed, 
      if (!this.compiled)
      {
        if (!this.LoadProgram())
        {
          return;
        }
      }

      this.InitialiseCanvasAndTurtle();

      var mainBreak = false;
      var mainContinue = false;
      var backgroundThread = new Thread(() => executor.Execute(commands, objects, new Turtle(), 0, ref mainBreak, ref mainContinue));
      executor.Running = true;

      this.loadButton.Enabled = false;
      this.stepButton.Enabled = false;
      this.runButton.Enabled = false;

      backgroundThread.Start();
    }

    private void UpdateTextboxes(Turtle turtle)
    {
      if (this.updateTextBoxes)
      {
        ThreadHelper.SetText(this, turtleXTextBox, turtle.X.ToString(CultureInfo.InvariantCulture));
        ThreadHelper.SetText(this, turtleYTextBox, turtle.Y.ToString(CultureInfo.InvariantCulture));
        ThreadHelper.SetText(this, turtleDirectionTextBox, turtle.Direction.ToString(CultureInfo.InvariantCulture));

        ThreadHelper.SetText(this, turtleRColourTextBox, turtle.ColorR.ToString());
        ThreadHelper.SetText(this, turtleGColourTextBox, turtle.ColorG.ToString());
        ThreadHelper.SetText(this, turtleBColourTextBox, turtle.ColorB.ToString());
        ThreadHelper.SetText(this, turtleAColourTextBox, turtle.ColorA.ToString());
      }
    }

    private void programTextBox_TextChanged(object sender, EventArgs e)
    {
      this.isDirty = true;
      this.compiled = false;

      if (!string.IsNullOrEmpty(currentFilename))
      {
        saveToolStripMenuItem.Enabled = true;
      }

      saveAsToolStripMenuItem.Enabled = true;
    }

    #region New/Open/Save/SaveAs

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var openFileDialog = new OpenFileDialog();
      openFileDialog.InitialDirectory = InitialDirectory;
      openFileDialog.Filter = @"logo files (*.lgl)|*.lgl|All files (*.*)|*.*";
      openFileDialog.DefaultExt = "lgl";
      var dialogResult = openFileDialog.ShowDialog();
      if (dialogResult == DialogResult.OK)
      {
        programTextBox.Text = string.Empty;

        var allLines = File.ReadAllLines(openFileDialog.FileName);

        foreach (var line in allLines)
        {
          programTextBox.Text += line + "\r\n";
        }

        isDirty = false;
        saveToolStripMenuItem.Enabled = false;
        saveAsToolStripMenuItem.Enabled = false;

        currentFilename = openFileDialog.FileName;
      }
    }

    private void newToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (isDirty)
      {
        var dialogResult = MessageBox.Show("Do you want to save your changes?", "New", MessageBoxButtons.YesNoCancel);

        if (dialogResult == DialogResult.Yes)
        {
          if (string.IsNullOrEmpty(currentFilename))
          {
            if (SaveAs())
            {
              isDirty = false;
              saveToolStripMenuItem.Enabled = false;
              saveAsToolStripMenuItem.Enabled = false;
              currentFilename = string.Empty;
              programTextBox.Text = string.Empty;
            }
          }
          else
          {
            if (Save())
            {
              isDirty = false;
              saveToolStripMenuItem.Enabled = false;
              saveAsToolStripMenuItem.Enabled = false;
              currentFilename = string.Empty;
              programTextBox.Text = string.Empty;
            }
          }
        }
      }
      else
      {
        isDirty = false;
        saveToolStripMenuItem.Enabled = false;
        saveAsToolStripMenuItem.Enabled = false;
        currentFilename = string.Empty;
        programTextBox.Text = string.Empty;
      }
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(currentFilename))
      {
        SaveAs();
      }
      else
      {
        Save();
      }
    }

    private bool Save()
    {
      try
      {
        File.WriteAllText(currentFilename, programTextBox.Text);
        isDirty = false;
        saveToolStripMenuItem.Enabled = false;
        saveAsToolStripMenuItem.Enabled = false;

        return true;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Unable to save to file {currentFilename}\nException: {ex.Message}");
        return false;
      }
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SaveAs();
    }

    private bool SaveAs()
    {
      var saveFileDialog = new SaveFileDialog();
      saveFileDialog.InitialDirectory = InitialDirectory;
      saveFileDialog.Filter = @"logo files (*.lgl)|*.lgl|All files (*.*)|*.*";
      saveFileDialog.DefaultExt = "lgl";
      var dialogResult = saveFileDialog.ShowDialog();
      if (dialogResult == DialogResult.OK)
      {
        try
        {
          File.WriteAllText(saveFileDialog.FileName, programTextBox.Text);

          isDirty = false;
          currentFilename = saveFileDialog.FileName;
          saveToolStripMenuItem.Enabled = false;
          saveAsToolStripMenuItem.Enabled = false;

          return true;
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Unable to save to file {currentFilename}\nException: {ex.Message}");
        }
      }

      return false;
    }

    #endregion

    private void wrapBordersCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (sender is CheckBox)
      {
        wrapBorders = (sender as CheckBox).Checked;
      }
    }

    private void updateUICheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (sender is CheckBox)
      {
        updateTextBoxes = (sender as CheckBox).Checked;
      }
    }

    private void ClearOutputText()
    {
      ThreadHelper.SetText(this, outputTextBox, string.Empty);
    }

    private void AddOutputText(string text)
    {
      if (this.updateTextBoxes)
      {
        ThreadHelper.AddText(this, outputTextBox, text + "\r\n");
        ThreadHelper.ScrollToEnd(this, outputTextBox);
      }
    }
  }
}