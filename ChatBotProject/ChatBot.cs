using System;
using System.Windows.Forms;
using System.Media;
using System.IO; // needed for filing
using System.Speech.Synthesis;

namespace ChatBotProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static ChatBot bot;
        static ChatBot user;
        SpeechSynthesizer reader = new SpeechSynthesizer();
        bool textToSpeech = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            init();

            // Sets Position for the first bubble on the top
            bbl_old.Top = 0 - bbl_old.Height;

            /*
            // Load Chat from the log file
            if (File.Exists("chat.log"))
            {
                using (StreamReader sr = File.OpenText("chat.log"))
                {
                    int i = 0; // to count lines
                    while (sr.Peek() >= 0) // loop till the file ends
                    {
                        if (i % 2 == 0) // check if line is even
                        {
                            addInMessage(sr.ReadLine());
                        }
                        else
                        {
                            addOutMessage(sr.ReadLine());
                        }
                        i++;
                    }
                    // scroll to the bottom once finished loading.
                    panel2.VerticalScroll.Value = panel2.VerticalScroll.Maximum;
                    panel2.PerformLayout();
                }
            }
            */
        }

        void init()
        {
            bot = new ChatBot();
            user = new ChatBot("Andrew");

            bot.Initialize();
            user.Initialize();

        }

        private void showOutput()
        {
            if (!(string.IsNullOrWhiteSpace(InputTxt.Text))) // Make sure the textbox isnt empty
            {
                SoundPlayer Send = new SoundPlayer("SOUND1.wav"); // Send Sound Effect
                SoundPlayer Rcv = new SoundPlayer("SOUND2.wav"); // Recieve Sound Effect


                InputTxt.Text = InputTxt.Text.Replace("I am not smart enough to understand what the heck you just said, sorry.", "");
                InputTxt.Text = InputTxt.Text.Replace("I am not smart enough to understand what the heck you just said, sorry", "");
                InputTxt.Text = InputTxt.Text.Replace("I am not smart enough to understand what the heck you just said", "");
                InputTxt.Text = InputTxt.Text.Replace("\n", "");

                if (InputTxt.Text == "" || string.IsNullOrEmpty(InputTxt.Text) || InputTxt.Text.ToLower() == InputTxt.Text.ToUpper())
                {

                }
                else
                {
                    // Show the user message and play the sound

                    addInMessage(InputTxt.Text);
                    Send.Play();
                }

                // Store the Bot's Output by giving it our input.
                string outtt = bot.getOutput(InputTxt.Text);

                if (outtt.Length == 0)
                {
                    outtt = "I don't understand.";
                }


                // Hide the "Bot is typing.." text
                txtTyping.Hide();

                outtt = outtt.Replace("I am not smart enough to understand what the heck you just said, sorry.", "");
                outtt = outtt.Replace("I am not smart enough to understand what the heck you just said, sorry", "");
                outtt = outtt.Replace("I am not smart enough to understand what the heck you just said", "");
                outtt = outtt.Replace("\n", "");

                if (outtt == "" || string.IsNullOrEmpty(outtt) || outtt.ToLower() == outtt.ToUpper())
                {
                    addOutMessage("Bye");
                }


                if (outtt == "")
                {
                    //init();
                    WriteToLog();
                    return;
                }
                else
                {
                    lock (this)
                    {
                        WriteToLog("[Bot:" + outtt + ']');
                    }
                }

                // disable the chat box white the bot is typing to prevent user spam.
                InputTxt.Enabled = false;

                // Once the timer ends

                InputTxt.Enabled = true; // Enable Chat box

                // Show the bot message and play the sound
                addOutMessage(outtt);
                //Rcv.Play();

                // Text to Speech if enabled
                if (textToSpeech)
                {
                    reader.SpeakAsync(outtt);
                }

                InputTxt.Focus(); // Put the cursor back on the textbox
                /*
                outtt = user.getOutput(outtt);
                InputTxt.Text = outtt; //User types the text
                */
                

                if (outtt == "" || string.IsNullOrEmpty(outtt) || outtt.ToLower() == outtt.ToUpper())
                {
                    //init();
                    WriteToLog();
                    return;
                }
                outtt = user.getOutput(outtt);
                InputTxt.Text = outtt;
                outtt = outtt.Replace("I am not smart enough to understand what the heck you just said, sorry.", "");
                outtt = outtt.Replace("I am not smart enough to understand what the heck you just said, sorry", "");
                outtt = outtt.Replace("I am not smart enough to understand what the heck you just said", "");
                if (outtt.ToLower().Contains("I am not smart enough to understand what the heck you just said, sorry".ToLower()))
                {
                    var v = 0;
                }

                lock (this)
                {
                    WriteToLog("[User:" + outtt + ']');
                }
                button1_Click(this.button1, new EventArgs());

            }
        }

        private void WriteToLog(string outtt)
        {
            //=========== Creates backup of chat from user and bot to the given location ============
            FileStream fs = new FileStream(@"chat.log", FileMode.Append, FileAccess.Write);
            if (fs.CanWrite)
            {
                byte[] write = System.Text.Encoding.ASCII.GetBytes(outtt + Environment.NewLine);
                fs.Write(write, 0, write.Length);
            }
            fs.Flush();
            fs.Close();
        }
        private void WriteToLog()
        {
            //=========== Creates backup of chat from user and bot to the given location ============
            FileStream fs = new FileStream(@"chat.log", FileMode.Append, FileAccess.Write);
            if (fs.CanWrite)
            {
                byte[] write = System.Text.Encoding.ASCII.GetBytes(Environment.NewLine);
                fs.Write(write, 0, write.Length);
            }
            fs.Flush();
            fs.Close();
        }

        // Call the Output method when the send button is clicked.
        private void button1_Click(object sender, EventArgs e)
        {
            showOutput();
        }

        // Call the Output method when the enter key is pressed.
        private void InputTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                showOutput();
                e.SuppressKeyPress = true; // Disable windows error sound
            }
        }
        
        // Dummy Bubble created to store the previous bubble data.
        bubble bbl_old = new bubble();

        // User Message Bubble Creation
        public void addInMessage(string message)
        {
            if (message == "I am not smart enough to understand what the heck you just said, sorry.")
                return;

            // Create new chat bubble
                bubble bbl = new bubble(message, msgtype.In);
            bbl.Location = bubble1.Location; // Set the new bubble location from the bubble sample.
            bbl.Left += 50; // Indent the bubble to the right side.
            bbl.Size = bubble1.Size; // Set the new bubble size from the bubble sample.
            bbl.Top = bbl_old.Bottom + 10; // Position the bubble below the previous one with some extra space.
            
            // Add the new bubble to the panel.
            panel2.Controls.Add(bbl);

            // Force Scroll to the latest bubble
            bbl.Focus();

            // save the last added object to the dummy bubble
            bbl_old = bbl;
        }

        // Bot Message Bubble Creation
        public void addOutMessage(string message)
        {
            // Create new chat bubble
            bubble bbl = new bubble(message, msgtype.Out);
            bbl.Location = bubble1.Location; // Set the new bubble location from the bubble sample.
            bbl.Size = bubble1.Size; // Set the new bubble size from the bubble sample.
            bbl.Top = bbl_old.Bottom + 10; // Position the bubble below the previous one with some extra space.
            
            // Add the new bubble to the panel.
            panel2.Controls.Add(bbl);

            // Force Scroll to the latest bubble
            bbl.Focus();

            // save the last added object to the dummy bubble
            bbl_old = bbl;
        }

        // Custom close button to close the program when clicked.
        private void close_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        
        // Clear all the bubbles and chat.log
        private void clearChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Delete the log file
            File.Delete(@"chat.log");

            // Clear the chat Bubbles
            panel2.Controls.Clear();

            // This reset the position for the next bubble to come back to the top.
            bbl_old.Top = 0 - bbl_old.Height;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(menuButton, new System.Drawing.Point(0, -contextMenuStrip1.Size.Height));
        }

        private void toggleVoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // whenever the toggle is clicked, true is set to false visa versa.
            textToSpeech = !textToSpeech;
        }
    }
}