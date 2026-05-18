using SmartShooting.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SmartShooting.Views;

public partial class MainWindow : Window
{
    private readonly ArduinoService arduinoService = new();
    private readonly GameService gameService = new();

    private readonly DispatcherTimer timerTest = new();
    private readonly Random hasard = new();

    private double distanceSimulee = 3.0;

    public MainWindow()
    {
        InitializeComponent();

        arduinoService.LigneRecue += ArduinoService_LigneRecue;

        timerTest.Interval = TimeSpan.FromMilliseconds(300);
        timerTest.Tick += TimerTest_Tick;

        ActualiserPorts();
        NouvelleDistance();
    }

    private void ActualiserPorts_Click(object sender, RoutedEventArgs e)
    {
        ActualiserPorts();
    }

    private void Connexion_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (arduinoService.EstConnecte())
            {
                arduinoService.Deconnecter();
                BoutonConnexion.Content = "Connecter";
                AjouterLog("Déconnecté");
                return;
            }

            if (PortComboBox.SelectedItem == null)
            {
                MessageBox.Show("Choisis un port COM.");
                return;
            }

            arduinoService.Connecter(PortComboBox.SelectedItem.ToString()!);
            BoutonConnexion.Content = "Déconnecter";
            AjouterLog("Arduino connecté");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erreur : " + ex.Message);
        }
    }

    private void ModeTest_Click(object sender, RoutedEventArgs e)
    {
        if (timerTest.IsEnabled)
        {
            timerTest.Stop();
            AjouterLog("Mode test désactivé");
        }
        else
        {
            timerTest.Start();
            AjouterLog("Mode test activé");
        }
    }

    private void NouvelleDistance_Click(object sender, RoutedEventArgs e)
    {
        NouvelleDistance();
    }

    private void ResetScore_Click(object sender, RoutedEventArgs e)
    {
        gameService.ResetScore();
        MettreAJourAffichage();
        AjouterLog("Score réinitialisé");
    }

    private void ActualiserPorts()
    {
        PortComboBox.ItemsSource = arduinoService.ObtenirPorts();

        if (PortComboBox.Items.Count > 0)
            PortComboBox.SelectedIndex = 0;
    }

    private void NouvelleDistance()
    {
        gameService.GenererNouvelleDistance();
        MettreAJourAffichage();
        AjouterLog("Nouvelle distance : " + gameService.Etat.DistanceDemandee + " m");
    }

    private void ArduinoService_LigneRecue(string ligne)
    {
        Dispatcher.Invoke(() =>
        {
            AjouterLog(ligne);

            if (ligne.StartsWith("DIST:"))
            {
                string valeur = ligne.Replace("DIST:", "").Replace(",", ".");

                if (double.TryParse(valeur, NumberStyles.Float, CultureInfo.InvariantCulture, out double distance))
                {
                    gameService.TraiterDistance(distance);
                    MettreAJourAffichage();
                }
            }

            if (ligne.StartsWith("RESULT:"))
            {
                string resultat = ligne.Replace("RESULT:", "").Trim();
                gameService.TraiterResultat(resultat);
                MettreAJourAffichage();
            }
        });
    }

    private void TimerTest_Tick(object? sender, EventArgs e)
    {
        distanceSimulee += (gameService.Etat.DistanceDemandee - distanceSimulee) * 0.15;
        distanceSimulee += (hasard.NextDouble() - 0.5) * 0.4;
        distanceSimulee = Math.Clamp(distanceSimulee, 0.5, 10);

        gameService.TraiterDistance(distanceSimulee);
        MettreAJourAffichage();
    }

    private void MettreAJourAffichage()
    {
        var etat = gameService.Etat;

        TexteDistanceDemandee.Text = etat.DistanceDemandee + " m";
        TexteDistanceActuelle.Text = etat.DistanceActuelle.ToString("0.00") + " m";
        TexteDistanceArrondie.Text = "Arrondi : " + etat.DistanceArrondie + " m";

        BarreDistance.Value = Math.Clamp(etat.DistanceActuelle, 0, 10);

        TexteMessage.Text = etat.Message;
        TexteResultat.Text = etat.Resultat;
        TexteScore.Text = "Score : " + etat.Score;
        TexteTirs.Text = "Tirs : " + etat.Tirs;

        if (etat.Message == "BONNE DISTANCE")
            BlocMessage.Background = new SolidColorBrush(Color.FromRgb(22, 163, 74));
        else if (etat.Message == "RECULEZ")
            BlocMessage.Background = new SolidColorBrush(Color.FromRgb(234, 88, 12));
        else if (etat.Message == "RAPPROCHEZ-VOUS")
            BlocMessage.Background = new SolidColorBrush(Color.FromRgb(37, 99, 235));
        else
            BlocMessage.Background = new SolidColorBrush(Color.FromRgb(51, 65, 85));

        if (etat.Resultat == "TOUCHÉ")
            BlocResultat.Background = new SolidColorBrush(Color.FromRgb(22, 163, 74));
        else if (etat.Resultat == "RATÉ")
            BlocResultat.Background = new SolidColorBrush(Color.FromRgb(220, 38, 38));
        else
            BlocResultat.Background = new SolidColorBrush(Color.FromRgb(15, 23, 42));
    }

    private void AjouterLog(string texte)
    {
        ZoneLogs.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + texte + Environment.NewLine);
        ZoneLogs.ScrollToEnd();
    }

    protected override void OnClosed(EventArgs e)
    {
        arduinoService.Deconnecter();
        base.OnClosed(e);
    }
}