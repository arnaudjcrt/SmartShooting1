using SmartShooting.Models;

namespace SmartShooting.Services;

public class GameService
{
    private readonly Random _random = new();
    private const double Tolerance = 0.5;

    public GameState Etat { get; } = new();

    public void GenererNouvelleDistance()
    {
        Etat.DistanceDemandee = _random.Next(1, 4);
        Etat.Resultat = "--";
        Etat.Message = "En attente...";
    }

    public void TraiterDistance(double distance)
    {
        Etat.DistanceActuelle = distance;
        Etat.DistanceArrondie = (int)Math.Round(distance, MidpointRounding.AwayFromZero);

        if (distance < Etat.DistanceDemandee - Tolerance)
            Etat.Message = "RECULEZ";
        else if (distance > Etat.DistanceDemandee + Tolerance)
            Etat.Message = "RAPPROCHEZ-VOUS";
        else
            Etat.Message = "BONNE DISTANCE";
    }

    public void TraiterResultat(string resultat)
    {
        if (resultat == "HIT") resultat = "TOUCHÉ";
        if (resultat == "MISS") resultat = "RATÉ";

        Etat.Tirs++;

        if (resultat == "TOUCHÉ")
            Etat.Score++;

        Etat.Resultat = resultat;
    }

    public void ResetScore()
    {
        Etat.Score = 0;
        Etat.Tirs = 0;
        Etat.Resultat = "--";
    }
}