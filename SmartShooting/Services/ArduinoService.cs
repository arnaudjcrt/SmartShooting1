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

        _portSerie = new SerialPort(nomPort, 9600)
        {
            NewLine = "\n",
            ReadTimeout = 1000,
            DtrEnable = true,
            RtsEnable = true
        };

        _portSerie.DataReceived += PortSerie_DataReceived;
        _portSerie.Open();

        Thread.Sleep(1500); // laisse le temps à l'Arduino de redémarrer
        _portSerie.DiscardInBuffer();
    }

    public void Deconnecter()
    {
        if (_portSerie != null)
        {
            try
            {
                _portSerie.DataReceived -= PortSerie_DataReceived;

                if (_portSerie.IsOpen)
                    _portSerie.Close();

                _portSerie.Dispose();
            }
            catch
            {
            }

            _portSerie = null;
        }
    }

    private void PortSerie_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (_portSerie == null || !_portSerie.IsOpen)
                return;

            string ligne = _portSerie.ReadLine().Trim();

            if (!string.IsNullOrWhiteSpace(ligne))
                LigneRecue?.Invoke(ligne);
        }
        catch
        {
        }
    }
}