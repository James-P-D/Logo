using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using LogicalParser;
using LogicalParser.Commands;
using LogicalParser.Objects;
using StringParser;
using Executor;

//TODO;c
// Introduce <Step> for single execution of commands
// Dynamic showing of image     MORE TESTING
// Fix load (disable step/run buttons on textbox changed)
// Initialise canvas and turtle (including textboxes and initial turtle icon) and make it work for restarts
// Fix update textboxes (not working!)
// floats? and ints? bytes? casting?!?
// Fix for error line numbers (are we not working correctly with comments?)
// Check variables (do we need that value in there? Nope! just need to get multiple-inheritance working with interfaces)
// Check if(..) {..} else if {..} !!
// Move error strings to resources file
// Check 'Unable to parse' 'StringParser.StringToken' on e.g. 'setcolora 223 + ;'
// Check 'Unable to parse' in general. 
// Check exceptions (throw new Exception("?!?!?!?");)

namespace Logo
{
  public partial class Form1 : Form
  {
    private Image imageWithoutTurtle;
    private Image imageWithTurtle;
    private Turtle turtle;

    public Form1()
    {
      InitializeComponent();

      wrapBordersCheckBox.Checked = wrapBorders;
      updateUICheckBox.Checked = updateTextBoxes;
      executor = new Executor.Executor();
      executor.AddOutputTextEvent += Executor_AddOutputTextEvent;
      executor.UpdateEvent += Executor_UpdateEvent;
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

      turtle = new Turtle(0, 0, 0);

      imageWithoutTurtle = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);
      using (var grp = Graphics.FromImage(imageWithoutTurtle))
      {
        grp.FillRectangle(Brushes.White, 0, 0, imageWidth, imageHeight);
      }

      DrawTurtle();
      UpdateTextboxes(this.turtle);
      ThreadHelper.SetImage(this, pictureBox1, imageWithTurtle);
    }

    private void UpdatePicture(Turtle turtle, int x1, int y1)
    {
      if (turtle.IsPenDown)
      {
        double TOLERANCE = 0.0001;
        if (Math.Abs(turtle.X - x1) > TOLERANCE || Math.Abs(turtle.Y - y1) > TOLERANCE)
        {
          UpdateImage(turtle, x1, y1);
        }
      }

      if (turtle.IsVisible)
      {
        DrawTurtle();
        //this.pictureBox1.Image = this.imageWithTurtle;
        //this.pictureBox1.Image = DrawTurtle(turtle, image);                
        ThreadHelper.SetImage(this, pictureBox1, imageWithTurtle);
      }
      else
      {
        //this.pictureBox1.Image = (Image)image.Clone();
        ThreadHelper.SetImage(this, pictureBox1, imageWithoutTurtle);
      }
    }


    void Executor_UpdateEvent(Turtle turtle, int x1, int y1)
    {
      //Thread.Sleep(10);
    }

    void Executor_AddOutputTextEvent(string text)
    {
      AddOutputText(text);
    }

    private bool running = false;
    private bool wrapBorders = false;
    private bool updateTextBoxes = true;

    private bool isDirty = false;
    private const string INITIAL_DIRECTORY = @".\Samples";
    private string currentFilename;
    private readonly Executor.Executor executor;

    private void stopButton_Click(object sender, EventArgs e)
    {
      Stop();
    }

    private void Stop()
    {
      running = false;
      executor.Running = false;
    }

    private void LoadButton_Click(object sender, EventArgs e)
    {

    }

    private void StepButton_Click(object sender, EventArgs e)
    {

    }

    private void runButton_Click(object sender, EventArgs e)
    {
      Run();
    }

    private void increaseFontSizeButton_Click(object sender, EventArgs e)
    {
      float fontSize = this.programTextBox.Font.Size;
      fontSize = Math.Max(5F, Math.Min(fontSize + 1F, 20F));
      this.programTextBox.Font = new Font("Courier New",
        fontSize, System.Drawing.FontStyle.Regular,
        System.Drawing.GraphicsUnit.Point,
        0);
    }

    private void decreaseFontSizeButton_Click(object sender, EventArgs e)
    {
      float fontSize = this.programTextBox.Font.Size;
      fontSize = Math.Max(5F, Math.Min(fontSize - 1F, 20F));
      this.programTextBox.Font = new Font("Courier New",
        fontSize, System.Drawing.FontStyle.Regular,
        System.Drawing.GraphicsUnit.Point,
        0);
    }

    private void StringTokeniserAddOutputText(string text)
    {
      AddOutputText(text);
    }

    private void Run()
    {
      ClearOutputText();
      var stringTokeniser = new StringTokeniser();
      InitialiseCanvasAndTurtle();

      try
      {
        stringTokeniser.AddOutputTextEvent += StringTokeniserAddOutputText;
        var allLines = programTextBox.Text.Split('\n');
        var stringTokens = stringTokeniser.Parse(allLines);

        var logicalParser = new Parser();
        logicalParser.Parse(stringTokens, out var commands, out var objects);

        running = true;
        var runThread = new BackgroundWorker();
        runThread.RunWorkerCompleted += runThread_RunWorkerCompleted;
        runThread.DoWork += runThread_DoWork;
        runThread.ProgressChanged += runThread_ProgressChanged;
        runThread.WorkerReportsProgress = true;

        AddOutputText("*** Compile successful ***");

        runThread.RunWorkerAsync(new object[] {commands, objects, turtle});
      }
      catch (Exception ex)
      {
        running = false;
        AddOutputText("ERROR: " + ex.Message);
        AddOutputText("*** Compile failed ***");
      }
      finally
      {
        stringTokeniser.AddOutputTextEvent -= StringTokeniserAddOutputText;
      }
    }



    private void UpdateTextboxes(Turtle turtle)
    {
      ThreadHelper.SetText(this, turtleXTextBox, turtle.X.ToString(CultureInfo.InvariantCulture));
      ThreadHelper.SetText(this, turtleYTextBox, turtle.Y.ToString(CultureInfo.InvariantCulture));
      ThreadHelper.SetText(this, turtleDirectionTextBox, turtle.Direction.ToString(CultureInfo.InvariantCulture));

      ThreadHelper.SetText(this, turtleRColourTextBox, turtle.ColorR.ToString());
      ThreadHelper.SetText(this, turtleGColourTextBox, turtle.ColorG.ToString());
      ThreadHelper.SetText(this, turtleBColourTextBox, turtle.ColorB.ToString());
      ThreadHelper.SetText(this, turtleAColourTextBox, turtle.ColorA.ToString());
    }

    private void UpdateImage(Turtle turtle, int x1, int y1)
    {
      using (var grp = Graphics.FromImage(imageWithoutTurtle))
      {
        var xCenter = imageWithoutTurtle.Width / 2;
        var yCenter = imageWithoutTurtle.Height / 2;

        var startX = x1 + xCenter;
        var startY = y1 + yCenter;
        var endX = (int) turtle.X + xCenter;
        var endY = (int) turtle.Y + yCenter;

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

            endX = (int) turtle.X + xCenter;
            endY = (int) turtle.Y + yCenter;

            grp.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
          }
        }
      }
    }

    private void DrawTurtle()
    {
      var x = turtle.X;
      var y = turtle.Y;
      var bottomLeftX = x;
      var bottomLeftY = y;
      var bottomRightX = x;
      var bottomRightY = y;

      turtle.CalculateNewPosition(turtle.CalculateNewDirection(45), -20, ref bottomLeftX, ref bottomLeftY);
      turtle.CalculateNewPosition(turtle.CalculateNewDirection(-45), -20, ref bottomRightX, ref bottomRightY);

      imageWithTurtle = (Image) imageWithoutTurtle.Clone();
      float xCenter = imageWithTurtle.Width / 2;
      float yCenter = imageWithTurtle.Height / 2;

      using (var grp = Graphics.FromImage(imageWithTurtle))
      {
        var pen = Pens.Black;

        grp.DrawLine(pen, new Point((int) (turtle.X + xCenter), (int) (turtle.Y + yCenter)),
          new Point((int) (bottomLeftX + xCenter), (int) (bottomLeftY + yCenter)));
        grp.DrawLine(pen, new Point((int) (turtle.X + xCenter), (int) (turtle.Y + yCenter)),
          new Point((int) (bottomRightX + xCenter), (int) (bottomRightY + yCenter)));
        grp.DrawLine(pen, new Point((int) (bottomLeftX + xCenter), (int) (bottomLeftY + yCenter)),
          new Point((int) (bottomRightX + xCenter), (int) (bottomRightY + yCenter)));
      }
    }

    void runThread_DoWork(object sender, DoWorkEventArgs e)
    {
      var commands = e.Argument is object[] ? ((object[]) e.Argument)[0] as List<Command> : null;
      if (((object[]) e.Argument).Length > 1)
      {
        var objects = e.Argument is object[] ? ((object[]) e.Argument)[1] as List<LogoObject> : null;
        var turtle = e.Argument is object[] ? ((object[]) e.Argument)[2] as Turtle : null;

        var mainBreak = false;
        var mainContinue = false;
        executor.Running = true;
        if (executor.Execute(sender, commands, objects, turtle, 0, ref mainBreak, ref mainContinue))
        {
          AddOutputText("*** Execution successful ***");
        }
        else
        {
          AddOutputText("*** Execution failed ***");
        }
      }
    }

    readonly object locker = new object();

    void runThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      lock (locker)
      {
        UpdatePicture(
          (e.UserState as object[])?[0] as Turtle,
          (int) (e.UserState as object[])?[1],
          (int) (e.UserState as object[])?[2]);
        UpdateTextboxes((e.UserState as object[])?[0] as Turtle);
      }

      //TODO: Do we still need this?!?
      // Yes! This doesn't seem to cause the same threading issue as firing on events!
      /*
        var x1 = (int)(e.UserState as object[])[0];
        var y1 = (int)(e.UserState as object[])[1];
        var x2 = (int)(e.UserState as object[])[2];
        var y2 = (int)(e.UserState as object[])[3];
        var direction1 = (int)(e.UserState as object[])[4];
        var direction2 = (int)(e.UserState as object[])[5];
        var isPenDown = (bool)(e.UserState as object[])[6];
        var isVisible = (bool)(e.UserState as object[])[7];
        var image = (Image)(e.UserState as object[])[8];
        lock (image)
        {
            var xCenter = image.Width / 2;
            var yCenter = image.Height / 2;
            //Console.WriteLine("{0}, {1} -> {2}, {3}", x1, y1, x2, y2);
            //Console.WriteLine("------------");

            //using (Graphics grp = Graphics.FromImage(image))
            //{
            //  //Console.WriteLine("{0}, {1} -> {2}, {3}", x1, y1, x2, y2);
            //  grp.DrawLine(Pens.Black, new Point(x1 + xCenter, y1 + yCenter), new Point(x2 + xCenter, y2 + yCenter));
            //}
            //this.pictureBox1.Image = (Image)image.Clone();

            ThreadHelper.SetText(this, turtleXTextBox, x2.ToString());
            ThreadHelper.SetText(this, turtleYTextBox, y2.ToString());

            //Thread.Sleep(100);
        }

        ThreadHelper.SetText(this, turtleXTextBox, x2.ToString());
        ThreadHelper.SetText(this, turtleYTextBox, y2.ToString());
        */
    }

    void runThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      running = false;
    }

    private void programTextBox_TextChanged(object sender, EventArgs e)
    {
      isDirty = true;

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
      openFileDialog.InitialDirectory = INITIAL_DIRECTORY;
      openFileDialog.Filter = "logo files (*.lgl)|*.lgl|All files (*.*)|*.*";
      openFileDialog.DefaultExt = "lgl";
      var dialogResult = openFileDialog.ShowDialog();
      if (dialogResult == DialogResult.OK)
      {
        programTextBox.Text = string.Empty;

        var allLines = File.ReadAllLines(openFileDialog.FileName);

        foreach (var line in FormatLines(allLines))
        {
          programTextBox.Text += line + "\r\n";
        }

        isDirty = false;
        saveToolStripMenuItem.Enabled = false;
        saveAsToolStripMenuItem.Enabled = false;

        currentFilename = openFileDialog.FileName;
      }
    }

    private string[] FormatLines(string[] allLines)
    {
      return allLines;
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
      saveFileDialog.InitialDirectory = INITIAL_DIRECTORY;
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
      ThreadHelper.AddText(this, outputTextBox, text + "\r\n");
      ThreadHelper.ScrollToEnd(this, outputTextBox);
    }
  }
}