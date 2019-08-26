using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using LogicalParser;
using LogicalParser.Commands;
using LogicalParser.Objects;
using StringParser;
using Executor;

//TODO;
// Introduce <Step> for single execution of commands
// Dynamic showing of image     MORE TESTING
// Initialise canvas and turtle (including textboxes and initial turtle icon)
// Fix update textboxes (not working!)
// floats? and ints? bytes? casting?!?
// do {} until();/while();?
// Check for additional unneccesary parameters to commands. if you run 'PENUP x' what happens?
// Fix for error line numbers (are we not working correctly with comments?)
// Getting values? GetX, GetY, GetDirection, IsPenUp, IsPenDown?
// Check variables (do we need that value in there?)
// Check if(..) {..} else if {..} !!
// Move error strings to resources file
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

            this.wrapBordersCheckBox.Checked = this.wrapBorders;
            this.updateUICheckBox.Checked = this.updateTextBoxes;
            this.executor = new Executor.Executor();
            this.executor.AddOutputTextEvent += new Executor.Executor.AddOutputTextDelegate(Executor_AddOutputTextEvent);
            this.executor.UpdateEvent += new global::Executor.Executor.UpdateDelegate(Executor_UpdateEvent);
            this.programTextBox.Text = "hideturtle; repeat 36 { repeat 360 { forward 5; rightturn 1; } rightturn 10; }";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.InitialiseCanvasAndTurtle();
        }

        private void InitialiseCanvasAndTurtle()
        {             
            int imageWidth = this.pictureBox1.Width;
            int imageHeight = this.pictureBox1.Height;

            this.turtle = new Turtle(0, 0, 0);

            this.imageWithoutTurtle = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);
            using (Graphics grp = Graphics.FromImage(this.imageWithoutTurtle))
            {
                grp.FillRectangle(Brushes.White, 0, 0, imageWidth, imageHeight);
            }

            DrawTurtle();

            ThreadHelper.SetImage(this, this.pictureBox1, this.imageWithTurtle);
        }

        private void UpdatePicture(Turtle turtle, int x1, int y1)
        {
            if (turtle.IsPenDown)
            {
                if (turtle.X != x1 || turtle.Y != y1)
                {
                    UpdateImage(turtle, x1, y1);
                }
            }

            if (turtle.IsVisible)
            {
                this.DrawTurtle();
                //this.pictureBox1.Image = this.imageWithTurtle;
                //this.pictureBox1.Image = DrawTurtle(turtle, image);                
                ThreadHelper.SetImage(this, this.pictureBox1, this.imageWithTurtle);
            }
            else
            {
                //this.pictureBox1.Image = (Image)image.Clone();
                ThreadHelper.SetImage(this, this.pictureBox1, this.imageWithoutTurtle);                
            }
        }


        void Executor_UpdateEvent(Turtle turtle, int x1, int y1)
        {
            //Thread.Sleep(10);
        }

        void Executor_AddOutputTextEvent(string text)
        {
            this.AddOutputText(text);
        }

        private bool running = false;
        private bool wrapBorders = false;
        private bool updateTextBoxes = true;

        private bool isDirty = false;
        private const string INITIAL_DIRECTORY = @".\Samples";
        private string currentFilename;
        private Executor.Executor executor;
                
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

        private void StringTokeniserAddOutputText(string text)
        {
            AddOutputText(text);
        }

        private void Run()
        {
            ClearOutputText();
            StringTokeniser stringTokeniser = new StringTokeniser();
            this.InitialiseCanvasAndTurtle();

            try
            {
                stringTokeniser.AddOutputTextEvent += new StringTokeniser.AddOutputTextDelegate(StringTokeniserAddOutputText);
                string[] allLines = programTextBox.Text.Split('\n');
                StringToken[] stringTokens = stringTokeniser.Parse(allLines);

                List<Command> commands;
                List<LogoObject> objects;
                Parser logicalParser = new Parser();
                logicalParser.Parse(stringTokens, out commands, out objects);
                                
                running = true;
                BackgroundWorker runThread = new BackgroundWorker();
                runThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(runThread_RunWorkerCompleted);
                runThread.DoWork += new DoWorkEventHandler(runThread_DoWork);
                runThread.ProgressChanged += new ProgressChangedEventHandler(runThread_ProgressChanged);
                runThread.WorkerReportsProgress = true;

                AddOutputText("*** Compile successful ***");

                runThread.RunWorkerAsync(new object[] { commands, objects, turtle});
            }
            catch (Exception ex)
            {
                running = false;
                AddOutputText("ERROR: " + ex.Message);
                AddOutputText("*** Compile failed ***");
            }
            finally
            {
                stringTokeniser.AddOutputTextEvent -= new StringTokeniser.AddOutputTextDelegate(StringTokeniserAddOutputText);
            }
        }
        
        

        private void UpdateTextboxes(Turtle turtle)
        {
            ThreadHelper.SetText(this, turtleXTextBox, turtle.X.ToString());
            ThreadHelper.SetText(this, turtleYTextBox, turtle.Y.ToString());
            ThreadHelper.SetText(this, turtleDirectionTextBox, turtle.Direction.ToString());

            ThreadHelper.SetText(this, turtleRColourTextBox, turtle.ColorR.ToString());
            ThreadHelper.SetText(this, turtleGColourTextBox, turtle.ColorG.ToString());
            ThreadHelper.SetText(this, turtleBColourTextBox, turtle.ColorB.ToString());
            ThreadHelper.SetText(this, turtleAColourTextBox, turtle.ColorA.ToString());
        }

        private void UpdateImage(Turtle turtle, int x1, int y1)
        {
            using (Graphics grp = Graphics.FromImage(this.imageWithoutTurtle))
            {
                int xCenter = this.imageWithoutTurtle.Width / 2;
                int yCenter = this.imageWithoutTurtle.Height / 2;

                int startX = x1 + xCenter;
                int startY = y1 + yCenter;
                int endX = (int)turtle.X + xCenter;
                int endY = (int)turtle.Y + yCenter;

                Pen pen = new Pen(Color.FromArgb(turtle.ColorA, turtle.ColorR, turtle.ColorG, turtle.ColorB));
                grp.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));

                if (this.wrapBorders)
                {
                    if (
                      (endX < 0) || (endX > this.imageWithoutTurtle.Width) ||
                      (endY < 0) || (endY > this.imageWithoutTurtle.Height))
                    {
                        if (endX < 0)
                        {
                            startX = x1 + xCenter + this.imageWithoutTurtle.Width;
                            turtle.SetX(turtle.X + this.imageWithoutTurtle.Width);
                        }
                        else if (endX > this.imageWithoutTurtle.Width)
                        {
                            startX = x1 + xCenter - this.imageWithoutTurtle.Width;
                            turtle.SetX(turtle.X - this.imageWithoutTurtle.Width);
                        }

                        if (endY < 0)
                        {
                            startY = y1 + yCenter + this.imageWithoutTurtle.Height;
                            turtle.SetY(turtle.Y + this.imageWithoutTurtle.Height);
                        }
                        else if (endY > this.imageWithoutTurtle.Height)
                        {
                            startY = y1 + yCenter - this.imageWithoutTurtle.Height;
                            turtle.SetY(turtle.Y - this.imageWithoutTurtle.Height);
                        }

                        endX = (int)turtle.X + xCenter;
                        endY = (int)turtle.Y + yCenter;

                        grp.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
                    }
                }
            }
        }

        private void DrawTurtle()
        {
            float x = this.turtle.X;
            float y = this.turtle.Y;
            float bottomLeftX = x;
            float bottomLeftY = y;
            float bottomRightX = x;
            float bottomRightY = y;

            turtle.CalculateNewPosition(this.turtle.CalculateNewDirection(45), -20, ref bottomLeftX, ref bottomLeftY);
            turtle.CalculateNewPosition(this.turtle.CalculateNewDirection(-45), -20, ref bottomRightX, ref bottomRightY);

            this.imageWithTurtle = (Image)this.imageWithoutTurtle.Clone();
            using (Graphics grp = Graphics.FromImage(this.imageWithTurtle))
            {
                float xCenter = this.imageWithTurtle.Width / 2;
                float yCenter = this.imageWithTurtle.Height / 2;

                Pen pen = Pens.Black;

                grp.DrawLine(pen, new Point((int)(turtle.X + xCenter), (int)(turtle.Y + yCenter)), new Point((int)(bottomLeftX + xCenter), (int)(bottomLeftY + yCenter)));
                grp.DrawLine(pen, new Point((int)(turtle.X + xCenter), (int)(turtle.Y + yCenter)), new Point((int)(bottomRightX + xCenter), (int)(bottomRightY + yCenter)));
                grp.DrawLine(pen, new Point((int)(bottomLeftX + xCenter), (int)(bottomLeftY + yCenter)), new Point((int)(bottomRightX + xCenter), (int)(bottomRightY + yCenter)));
            }
        }

        void runThread_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Command> commands = ((e.Argument as object[])[0]) as List<Command>;
            List<LogoObject> objects = ((e.Argument as object[])[1]) as List<LogoObject>;
            Turtle turtle = ((e.Argument as object[])[2]) as Turtle;
            
            bool mainBreak = false;
            bool mainContinue = false;
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

        object locker = new object();
        void runThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (locker)
            {
                this.UpdatePicture(
                    (e.UserState as object[])[0] as Turtle,
                    (int)(e.UserState as object[])[1],
                    (int)(e.UserState as object[])[2]);
                this.UpdateTextboxes((e.UserState as object[])[0] as Turtle);
            }
          //TODO: Do we still need this?!?
          // Yes! This doesn't seem to cause the same threading issue as firing on events!
            return;
            int x1 = (int)((e.UserState as object[])[0]);
            int y1 = (int)((e.UserState as object[])[1]);
            int x2 = (int)((e.UserState as object[])[2]);
            int y2 = (int)((e.UserState as object[])[3]);
            int direction1 = (int)((e.UserState as object[])[4]);
            int direction2 = (int)((e.UserState as object[])[5]);
            bool isPenDown = (bool)((e.UserState as object[])[6]);
            bool isVisible = (bool)((e.UserState as object[])[7]);
            Image image = (Image)((e.UserState as object[])[8]);
            lock (image)
            {
                int xCenter = image.Width / 2;
                int yCenter = image.Height / 2;
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
        }

        void runThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            running = false;
        }

        private void programTextBox_TextChanged(object sender, EventArgs e)
        {
            this.isDirty = true;

            if (!string.IsNullOrEmpty(this.currentFilename))
            {
                saveToolStripMenuItem.Enabled = true;
            }
            saveAsToolStripMenuItem.Enabled = true;
        }

        #region New/Open/Save/SaveAs

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = INITIAL_DIRECTORY;
            openFileDialog.Filter = "logo files (*.lgl)|*.lgl|All files (*.*)|*.*";
            openFileDialog.DefaultExt = "lgl";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                programTextBox.Text = string.Empty;

                string[] allLines = File.ReadAllLines(openFileDialog.FileName);

                foreach (string line in FormatLines(allLines))
                {
                    programTextBox.Text += line + "\r\n";
                }

                this.isDirty = false;
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;

                this.currentFilename = openFileDialog.FileName;
            }
        }

        private string[] FormatLines(string[] allLines)
        {
            return allLines;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.isDirty)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to save your changes?", "New", MessageBoxButtons.YesNoCancel);

                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    if (string.IsNullOrEmpty(currentFilename))
                    {
                        if (SaveAs())
                        {
                            this.isDirty = false;
                            saveToolStripMenuItem.Enabled = false;
                            saveAsToolStripMenuItem.Enabled = false;
                            this.currentFilename = string.Empty;
                            this.programTextBox.Text = string.Empty;
                        }
                    }
                    else
                    {
                        if (Save())
                        {
                            this.isDirty = false;
                            saveToolStripMenuItem.Enabled = false;
                            saveAsToolStripMenuItem.Enabled = false;
                            this.currentFilename = string.Empty;
                            this.programTextBox.Text = string.Empty;
                        }
                    }
                }
            }
            else
            {
                this.isDirty = false;
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                this.currentFilename = string.Empty;
                this.programTextBox.Text = string.Empty;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.currentFilename))
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
                this.isDirty = false;
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save to file {this.currentFilename}\nException: {ex.Message}");
                return false;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private bool SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = INITIAL_DIRECTORY;
            saveFileDialog.Filter = "logo files (*.lgl)|*.lgl|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "lgl";
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, programTextBox.Text);

                    this.isDirty = false;
                    this.currentFilename = saveFileDialog.FileName;
                    saveToolStripMenuItem.Enabled = false;
                    saveAsToolStripMenuItem.Enabled = false;

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to save to file {this.currentFilename}\nException: {ex.Message}");
                }
            }

            return false;
        }

        #endregion

        private void wrapBordersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox)
            {
                this.wrapBorders = (sender as CheckBox).Checked;
            }
        }

        private void updateUICheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox)
            {
                this.updateTextBoxes = (sender as CheckBox).Checked;
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