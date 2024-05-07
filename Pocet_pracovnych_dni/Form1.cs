using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Pocet_pracovnych_dni
{
    public partial class Form1 : Form
    {
        private readonly List<string> pravidelneSviatky;

        public Form1()
        {
            InitializeComponent();
            pravidelneSviatky = new List<string>()
            {
                 "06.01",
                 "01.05",
                 "08.05",
                 "15.09",
                 "01.11",
                 "24.12",
                 "25.12",
                 "26.12"
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int rok;
            int mesiac;

            if (int.TryParse(textBox2.Text, out rok) && int.TryParse(textBox1.Text, out mesiac) && mesiac <= 12 && mesiac >= 1 && rok >= 1)
            {
                int pracovneDni = PocitajPracovneDni(rok, mesiac);

                label1.Text = $" {pracovneDni}";
            }
            else
            {
                MessageBox.Show("Prosím zadajte mesiac a rok v správnom číselnom formáte.");
            }
        }

        private int PocitajPracovneDni(int rok, int mesiac)
        {
            int pracovneDni = 0;
            DateTime velkonocnyPiatok = VelkonocnyPiatok(rok, mesiac);
            DateTime velkonocnyPondelok = VelkonocnyPondelok(rok, mesiac);

            for (int den = 1; den <= DateTime.DaysInMonth(rok, mesiac); den++)
            {
                DateTime date = new DateTime(rok, mesiac, den);
                if (JePracovnyDen(date, velkonocnyPiatok, velkonocnyPondelok))
                {
                    pracovneDni++;
                }
            }
            return pracovneDni;
        }

        private bool JePracovnyDen(DateTime datum, DateTime velkonocnyPiatok, DateTime velkonocnyPondelok)
        {
            return datum.DayOfWeek != DayOfWeek.Saturday &&
                   datum.DayOfWeek != DayOfWeek.Sunday &&
                   datum != velkonocnyPiatok &&
                   datum != velkonocnyPondelok &&
                   !pravidelneSviatky.Contains(datum.ToString("dd.MM"));
        }

        private DateTime VelkonocnyPiatok(int rok, int mesiac)
        {
            // Algoritmus na výpočet dátumu Veľkonocneho piatka podľa Gaussovho vzorca
            int a = rok % 19;
            int b = rok % 4;
            int c = rok % 7;
            int m = 24;
            int n = 5;
            int d = (19 * a + m) % 30;
            int e = (n + 2 * b + 4 * c + 6 * d) % 7;
            int nedelaMarec = 22 + d + e;
            int nedelaApril = d + e - 9;

            if (nedelaMarec >= 0 && nedelaMarec <= 31)
            {
                return new DateTime(rok, 3, nedelaMarec).AddDays(-2);
            }
            else if (nedelaMarec == 26 && d == 29 && (11 * (rok % 19) + 11) % 30 < 19)
            {
                return new DateTime(rok, 3, 25).AddDays(-2);
            }
            else if (nedelaApril > 0 && nedelaApril <= 31)
            {
                return new DateTime(rok, 4, nedelaApril).AddDays(-2);
            }
            else
            {
                return DateTime.MinValue;// Vráť minimálnu hodnotu dátumu v prípade chyby
            }
        }

        private DateTime VelkonocnyPondelok(int rok, int mesiac)
        {
            // Algoritmus na výpočet dátumu Veľkonocneho pondelka podľa Gaussovho vzorca
            int a = rok % 19;
            int b = rok % 4;
            int c = rok % 7;
            int m = 24;
            int n = 5;
            int d = (19 * a + m) % 30;
            int e = (n + 2 * b + 4 * c + 6 * d) % 7;
            int nedelaMarec = 22 + d + e;
            int nedelaApril = d + e - 9;

            if (nedelaMarec >= 0 && nedelaMarec <= 31)
            {
                return new DateTime(rok, 3, nedelaMarec).AddDays(1);
            }
            else if (nedelaMarec == 26 && d == 29 && (11 * (rok % 19) + 11) % 30 < 19)
            {
                return new DateTime(rok, 3, 25).AddDays(1);
            }
            else if (nedelaApril > 0 && nedelaApril <= 31)
            {
                return new DateTime(rok, 4, nedelaApril).AddDays(1);
            }
            else
            {
                return DateTime.MinValue;// Vráť minimálnu hodnotu dátumu v prípade chyby
            }
        }
    }
}