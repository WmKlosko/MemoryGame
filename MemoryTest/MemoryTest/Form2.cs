using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using static MemoryTest.Klasy.Scores;

namespace MemoryTest
{
    public partial class Form1 : Form
    {
        Random random = new Random();

        int clicks;
        string playerName = "";

        List<string> icons = new List<string>()
        {
            "M", "M", "P", "P", "J", "J", "I", "I",
            "S", "S", "E", "E", "G", "G", "C", "C"
        };

        Label firstClicked, secondClicked;

        public Form1()
        {
            InitializeComponent();
            AssignIconsToSquares();

            clicks = 0;

        }

        private void label_Click(object sender, EventArgs e)
        {

            addTheClick();

            //blokowanie trzeciego kliknięcia
            if (firstClicked != null && secondClicked != null)
            {
                return;
            }

            Label clickedLabel = sender as Label;

            if (clickedLabel == null)
            {
                return;
            }
            //Blokowanie klikania na tą samą ikonę

            if (clickedLabel.ForeColor == Color.Black)
            { 
            return;
            }

            if (firstClicked == null)
            {
                firstClicked = clickedLabel;
                firstClicked.ForeColor = Color.Black;
                return;
            }

            secondClicked = clickedLabel;
            secondClicked.ForeColor = Color.Black;

            if (firstClicked.Text == secondClicked.Text)
            {
                firstClicked.ForeColor = Color.FromArgb(175, 237, 192);
                secondClicked.ForeColor = Color.FromArgb(175, 237, 192);
                firstClicked = null;
                secondClicked = null;
            }
            else
            {
                timer1.Start();
            }
            
            CheckForWinner();


        }

        private void addTheClick()
        {
            clicks++;
        }

        /// <summary>
        /// funkcja wyciąga wyniki z bazy danych
        /// </summary>
        /// <param name="context">reprezentacja BD</param>
        /// <param name="recordsNumber">liczba rekordów do wyciągnięcia</param>
        /// <param name="sortType">wybieranie wartości według, której będę sortować (clicks/date)</param>
        /// <param name="orderType">wybiaranie kolejnosci rosnacej lub malejacej</param>
        /// <returns></returns>
        private string getHighscores(KloskowindEntities1 context, int recordsNumber, string sortType, string orderType)
        {
            List<Highscores> highscores = context.Highscores.SqlQuery("SELECT TOP " + recordsNumber + " * FROM Highscores ORDER BY " + sortType +" " + orderType).ToList<Highscores>();
            var highscoresToDisplay = "";
            var place = 1;
            foreach (var score in highscores)
            {
                highscoresToDisplay += place + ". " + score.Name + " - " + score.Clicks + " klikniec " + "(" + score.Date + ")\n";
                place++;
            }
            return highscoresToDisplay;
        }

        /// <summary>
        /// Funkcja spardza czy gra powinna zostać zakończona
        /// </summary>
        private void CheckForWinner()
        {

            Label label;
           
            for (int i = 0; i < SiatkaGry.Controls.Count; i++)
            {
                label = SiatkaGry.Controls[i] as Label;

                if (label != null && label.ForeColor == label.BackColor)
                    return;
            }
         

            string name;
            if (playerName.Trim() == "")
            {
                name = "Bezimienny";
            } else
            {
                name = playerName;
            }

            var context = new KloskowindEntities1();

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            if (context.Database.Exists())
            {

                Highscores playersScore = new Highscores { Clicks = clicks, Name = name, Date = DateTime.Now, Machine = Environment.MachineName };
                context.Highscores.Add(playersScore);
                context.SaveChanges();

                var highscoresToDisplay = getHighscores(context, 5, "clicks", "ASC");
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow;

                MessageBox.Show("Udalo Ci sie dopasowac wszystkie ikony, wygrales!\nLiczba klikniec: " + clicks + "\nNazwa gracza: " + name + "\n\nNajlepsi gracze:\n" + highscoresToDisplay);

                Close();
            } else
            {
                MessageBox.Show("Nie można polaczyć z baza danych - wynik nie zostal zapisany", "Blad bazy danych", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Close();
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;

            firstClicked = null;
            secondClicked = null;
        }

        private void SiatkaGry_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            playerName = textBox1.Text;
        }

        private void label17_Click_1(object sender, EventArgs e)
        {
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://icon-icons.com/icon/palm-beach-travel-vacation-leisure/53586");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonHighscores_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            var context = new KloskowindEntities1();
            var highscoresToDisplay = getHighscores(context, 20, "clicks", "ASC");
            MessageBox.Show(highscoresToDisplay);

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow;
        }

        private void buttonLastScores_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            var context = new KloskowindEntities1();
            var highscoresToDisplay = getHighscores(context, 20, "date", "DESC");
            MessageBox.Show(highscoresToDisplay);

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow;
        }

        private void AssignIconsToSquares()
        {
            Label label;
            int randomNumber;

            for (int i = 0; i < SiatkaGry.Controls.Count; i++)
            {
                if (SiatkaGry.Controls[i] is Label)
                    label = (Label)SiatkaGry.Controls[i];
                else
                    continue;

                randomNumber = random.Next(0, icons.Count);
                label.Text = icons[randomNumber];

                icons.RemoveAt(randomNumber);
            }
        }

    }
}
