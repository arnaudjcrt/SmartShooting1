using System.IO.Ports;

namespace SmartShooting.Services;

public class ArduinoService
{
    private SerialPort? _portSerie;

    public event Action<string>? LigneRecue;

    public string[] ObtenirPorts()
    {
        return SerialPort.GetPortNames();
    }

    public bool EstConnecte()
    {
        return _portSerie != null && _portSerie.IsOpen;
    }

    public void Connecter(string nomPort)
    {
        Deconnecter();

        _portSerie = new SerialPort(nomPort, 9600);
        _portSerie.NewLine = "\n";
        _portSerie.DataReceived += PortSerie_DataReceived;
        _portSerie.Open();
    }

    public void Deconnecter()
    {
        if (_portSerie != null)
        {
            if (_portSerie.IsOpen)
                _portSerie.Close();

            _portSerie.Dispose();
            _portSerie = null;
        }
    }

    private void PortSerie_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string ligne = _portSerie!.ReadLine().Trim();
            LigneRecue?.Invoke(ligne);
        }
        catch
        {
        }
    }
}