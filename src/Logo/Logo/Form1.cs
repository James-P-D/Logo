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

            wrapBordersCheckBox.Checked = wrapBorders;
            updateUICheckBox.Checked = updateTextBoxes;
            executor = new Executor.Executor();
            executor.AddOutputTextEvent += new Executor.Executor.AddOutputTextDelegate(Executor_AddOutputTextEvent);
            executor.UpdateEvent += new global::Executor.Executor.UpdateDelegate(Executor_UpdateEvent);
            programTextBox.Text = "hideturtle; repeat 36 { repeat 360 { forward 5; rightturn 1; } rightturn 10; }";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitialiseCanvasAndTurtle();
        }

        private void InitialiseCanvasAndTurtle()
        {             
            int imageWidth = pictureBox1.Width;
            int imageHeight = pictureBox1.Height;

            turtle = new Turtle(0, 0, 0);

            imageWithoutTurtle = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);
            using (Graphics grp = Graphics.FromImage(imageWithoutTurtle))
            {
                grp.FillRectangle(Brushes.White, 0, 0, imageWidth, imageHeight);
            }

            DrawTurtle();

            ThreadHelper.SetImage(this, pictureBox1, imageWithTurtle);
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

        private void StringTokeniserAddOutputText(string text)
        {
            AddOutputText(text);
        }

        private void Run()
        {
            ClearOutputText();
            StringTokeniser stringTokeniser = new StringTokeniser();
            InitialiseCanvasAndTurtle();

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
            using (Graphics grp = Graphics.FromImage(imageWithoutTurtle))
            {
                int xCenter = imageWithoutTurtle.Width / 2;
                int yCenter = imageWithoutTurtle.Height / 2;

                int startX = x1 + xCenter;
                int startY = y1 + yCenter;
                int endX = (int)turtle.X + xCenter;
                int endY = (int)turtle.Y + yCenter;

                Pen pen = new Pen(Color.FromArgb(turtle.ColorA, turtle.ColorR, turtle.ColorG, turtle.ColorB));
                grp.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));

                if (wrapBorders)
                {
                    if (
                      (endX < 0) || (endX > imageWithoutTurtle.Width) ||
                      (endY < 0) || (endY > imageWithoutTurtle.Height))
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

        private void DrawTurtle()
        {
            float x = turtle.X;
            float y = turtle.Y;
            float bottomLeftX = x;
            float bottomLeftY = y;
            float bottomRightX = x;
            float bottomRightY = y;

            turtle.CalculateNewPosition(turtle.CalculateNewDirection(45), -20, ref bottomLeftX, ref bottomLeftY);
            turtle.CalculateNewPosition(turtle.CalculateNewDirection(-45), -20, ref bottomRightX, ref bottomRightY);

            imageWithTurtle = (Image)imageWithoutTurtle.Clone();
            using (Graphics grp = Graphics.FromImage(imageWithTurtle))
            {
                float xCenter = imageWithTurtle.Width / 2;
                float yCenter = imageWithTurtle.Height / 2;

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

        readonly object locker = new object();
        void runThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (locker)
            {
                UpdatePicture(
                    (e.UserState as object[])[0] as Turtle,
                    (int)(e.UserState as object[])[1],
                    (int)(e.UserState as object[])[2]);
                UpdateTextboxes((e.UserState as object[])[0] as Turtle);
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = INITIAL_DIRECTORY;
            openFileDialog.Filter = "logo files (*.lgl)|*.lgl|All files (*.*)|*.*";
            openFileDialog.DefaultExt = "lgl";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                programTextBox.Text = string.Empty;

                string[] allLines = File.ReadAllLines(openFileDialog.FileName);

                foreach (string line in FormatLines(allLines))
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
                DialogResult dialogResult = MessageBox.Show("Do you want to save your changes?", "New", MessageBoxButtons.YesNoCancel);

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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = INITIAL_DIRECTORY;
            saveFileDialog.Filter = "logo files (*.lgl)|*.lgl|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "lgl";
            DialogResult dialogResult = saveFileDialog.ShowDialog();
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