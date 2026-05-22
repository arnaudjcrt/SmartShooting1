using SmartShooting.Models;

namespace SmartShooting.Services;

public class GameService
{
    private readonly Random random = new();

    private const double Tolerance = 10;

    public GameState Etat { get; } = new();

    public void GenererNouvelleDistance()
    {
        Etat.DistanceDemandee = random.Next(50, 301);
        Etat.Resultat = "--";
        Etat.Message = "En attente...";
    }

    public void TraiterDistance(double distance)
    {
        Etat.DistanceActuelle = distance;
        Etat.DistanceArrondie = (int)Math.Round(distance);

        if (distance < Etat.DistanceDemandee - Tolerance)
            Etat.Message = "RECULEZ";
        else if (distance > Etat.DistanceDemandee + Tolerance)
            Etat.Message = "RAPPROCHEZ-VOUS";
        else
            Etat.Message = "BONNE DISTANCE";
    }

    public void TraiterLuminosite(int luminosite)
    {
        Etat.Luminosite = luminosite;
    }

    public void TraiterResultat(string resultat)
    {
        if (resultat == "HIT")
            resultat = "TOUCHÉ";

        if (resultat == "MISS")
            resultat = "RATÉ";

        Etat.Tirs++;

        if (resultat == "TOUCHÉ")
            Etat.Score += 100;

        Etat.Resultat = resultat;
    }

    public void ResetScore()
    {
        Etat.Score = 0;
        Etat.Tirs = 0;
        Etat.Resultat = "--";
    }
}