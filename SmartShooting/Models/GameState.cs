namespace SmartShooting.Models;

public class GameState
{
    public int DistanceDemandee { get; set; }
    public double DistanceActuelle { get; set; }
    public int DistanceArrondie { get; set; }
    public int Score { get; set; }
    public int Tirs { get; set; }
    public string Message { get; set; } = "En attente...";
    public string Resultat { get; set; } = "--";
}